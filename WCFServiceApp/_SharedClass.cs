using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace WCFServiceApp
{
    public class _SharedClass
    {
        public static SqlConnection ObtainConnectionString()
        {
            return new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["WCFService.PythonWarriors.SQLConnectionString"].ToString());
        }
        public static Player LoginValidation(string username, string passwordHash)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string loginQuery = @"SELECT Count(*) FROM Account 
                     WHERE (userName = @Username OR email = @Username) AND password = @Password";
            //            string emailQuery = @"SELECT userID FROM Account 
            //                     WHERE (userName = @Username OR email = @Username) AND password = @Password";
            try
            {
                SqlCommand loginCommand = new SqlCommand(loginQuery, sqlConn);
                loginCommand.Parameters.AddWithValue("@Username", username);
                loginCommand.Parameters.AddWithValue("@Password", passwordHash);
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                if ((int)loginCommand.ExecuteScalar() > 0)
                {
                    //SqlCommand emailCommand =  new SqlCommand(emailQuery, sqlConn);
                    //emailCommand.Parameters.AddWithValue("@Username", username);
                    //emailCommand.Parameters.AddWithValue("@Password", passwordHash);
                    //SqlDataReader sqlDataReader = emailCommand.ExecuteReader();
                    //decimal _email = 0;
                    //while (sqlDataReader.Read())
                    //    _email = sqlDataReader.GetDecimal(0);
                    return new Player()
                    {
                        Id = GetDecimalFromAccountTable("userID", username, passwordHash),
                        Username = GetStringFromAccountTable("userName", username, passwordHash),
                        Email = GetStringFromAccountTable("email", username, passwordHash),
                        PasswordHash = GetStringFromAccountTable("password", username, passwordHash),
                        Balance = GetDecimalFromAccountTable("balance", username, passwordHash)
                    };
                }
                return new Player() { Username = "INVALID" };

            }
            catch (Exception)
            {
                return new Player() { Username = "INVALID" };
            }
            finally
            {
                sqlConn.Close();
            }

        }

        public static bool RegisterNewUser(string username, string passwordHash, string email)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string registerQuery = "INSERT INTO Account (userName, email, password)";
            registerQuery += " VALUES (@Username, @Email, @Password)";
            try
            {
                SqlCommand registerCommand = new SqlCommand(registerQuery, sqlConn);
                registerCommand.Parameters.AddWithValue("@Username", username);
                registerCommand.Parameters.AddWithValue("@Email", email);
                registerCommand.Parameters.AddWithValue("@Password", passwordHash);
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                return (registerCommand.ExecuteNonQuery() != 0);

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static string HashPassword(string password)
        {
            return sha256(password);
        }
        /// <summary>
        /// Secure password with SHA256 standard (UTF8-encoded)
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string sha256(string password)
        {
            SHA256Managed crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte bit in crypto)
            {
                hash += bit.ToString("x2");
            }
            return hash;
        }
        private static string GetStringFromAccountTable(string columnName, string username, string passwordHash)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string emailQuery = @"SELECT " + columnName + " FROM Account WHERE (userName = @Username OR email = @Username) AND password = @Password";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand emailCommand = new SqlCommand(emailQuery, sqlConn);
                emailCommand.Parameters.AddWithValue("@Username", username);
                emailCommand.Parameters.AddWithValue("@Password", passwordHash);
                SqlDataReader sqlDataReader = emailCommand.ExecuteReader();
                string _email = "";
                while (sqlDataReader.Read())
                    _email = sqlDataReader.GetString(0);
                return _email;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                sqlConn.Close();
            }
        }
        private static decimal GetDecimalFromAccountTable(string columnName, string username, string passwordHash)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT " + columnName + " FROM Account WHERE (userName = @Username OR email = @Username) AND password = @Password";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", passwordHash);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                decimal _decimal = -1;
                while (sqlDataReader.Read())
                    _decimal = sqlDataReader.GetDecimal(0);
                return _decimal;
            }
            catch (Exception)
            {
                return 0;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static string GenerateAuthToken(string username, string password)
        {
            decimal userID = GetDecimalFromAccountTable("userID", username, HashPassword(password));
            if (UserIDExistsInAuthToken(userID))
                return UpdateAndReturnAuthToken(userID); //Update and return token

            //return the new token
            return NewAuthToken(userID);
        }


        public static string UpdateAndReturnAuthToken(decimal userID)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string query = "UPDATE AuthToken SET auth_token = @Auth_Token, expire = @Expire WHERE userID = @UserID";
            try
            {
                string newAuthToken = sha256(DateTime.Now.AddHours(24).ToString() + userID);
                DateTime newExpire = DateTime.Now.AddHours(24);
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@Auth_Token", newAuthToken);
                command.Parameters.AddWithValue("@Expire", newExpire);
                command.Parameters.AddWithValue("@UserID", userID);

                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                if (command.ExecuteNonQuery() != 0)
                    return newAuthToken;
                return null;
            }
            catch (Exception ex)
            {
                return "Exception: UpdateAndReturnAuthToken: " + ex.Message.ToString();
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static string NewAuthToken(decimal userID)
        {
            DateTime expire = DateTime.Today.AddHours(24);
            string auth_token = sha256(DateTime.Now.AddHours(24).ToString() + userID);
            SqlConnection sqlConn = ObtainConnectionString();
            string query = "INSERT INTO AuthToken (userID, auth_token, expire)";
            query += " VALUES (@userID, @Auth_Token, @Expire)";
            try
            {
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userID", userID);
                command.Parameters.AddWithValue("@Auth_Token", auth_token);
                command.Parameters.AddWithValue("@Expire", expire);
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                if (command.ExecuteNonQuery() != 0)
                {
                    return auth_token;
                }
                return null;

            }
            catch (Exception ex)
            {
                return "Exception: NewAuthToken: " + ex.Message.ToString();
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static bool DeleteAuthToken(string auth_token)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string query = "DELETE FROM AuthToken WHERE auth_token = @Auth_Token";
            try
            {
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@Auth_Token", auth_token);
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                return (command.ExecuteNonQuery() != 0);

            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                sqlConn.Close();
            }
        }


        public static bool UserIDExistsInAuthToken(decimal userID)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT userID FROM AuthToken WHERE userID = @UserID";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                decimal _existUserID = -1;
                while (sqlDataReader.Read())
                    _existUserID = sqlDataReader.GetDecimal(0);
                return (Decimal.Compare(userID, _existUserID) == 0);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static decimal GetLoggedInUserID(string auth_token)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT userID FROM AuthToken WHERE auth_token = @Auth_Token";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@Auth_Token", auth_token);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                decimal userID = -1;
                while (sqlDataReader.Read())
                    userID = sqlDataReader.GetDecimal(0);
                return userID;
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        /// <summary>
        /// GAME API
        /// </summary>
        /// <param name="auth_token"></param>
        /// <returns></returns>
        public static List<string> GetAllAvailableCharacterNames(string auth_token)
        {
            decimal userID = GetUserIDInAuthToken(auth_token);
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT charName FROM Character WHERE userID = @UserID";
            List<string> allCharacterNames = new List<string>();
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader sqlDataReader = command.ExecuteReader();

                while (sqlDataReader.Read())
                    allCharacterNames.Add(sqlDataReader.GetString(0));
                return allCharacterNames;
            }
            catch (Exception ex)
            {

                allCharacterNames.Add("Exception: GetAllAvailableCharacterNames: " + ex.Message.ToString());
                return allCharacterNames;
            }
            finally
            {
                sqlConn.Close();
            }
        }


        public static List<string> GetAllItemsIDBelongToUser(string auth_token)
        {
            decimal userID = GetUserIDInAuthToken(auth_token);
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT itemID FROM Item WHERE userID = @UserID";
            List<string> allItemIDs = new List<string>();
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader sqlDataReader = command.ExecuteReader();

                while (sqlDataReader.Read())
                    allItemIDs.Add(sqlDataReader.GetString(0));
                return allItemIDs;
            }
            catch (Exception ex)
            {

                allItemIDs.Add("Exception: GetAllItemsIDBelongToUser: " + ex.Message.ToString());
                return allItemIDs;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static decimal GetUserIDInAuthToken(string auth_token)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT userID FROM AuthToken WHERE auth_token = @Auth_Token";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@Auth_Token", auth_token);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                decimal userID = -1;
                while (sqlDataReader.Read())
                    userID = sqlDataReader.GetDecimal(0);
                return userID;
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static Character GetCharacter(string charName, string auth_token)
        {
            decimal userID = GetLoggedInUserID(auth_token);
            // Check if the requested character name is belong to the userID laa
            // Using Linq ^^
            if (!charName.Equals(GetAllAvailableCharacterNames(auth_token).Where(x => x.Contains(charName)).FirstOrDefault()))
                return null;

            return new Character()
            {
                CharName = charName,
                Id = userID,
                CharacterObj = GetCharacterObj("character", charName, userID),
                Stage = GetCharacterObj("stage", charName, userID),
                StageExp = GetStageExp(charName, userID),
                Updated = GetUpdated(charName, userID)
            };
        }


        public static string GetCharacterObj(string objColumn, string charName, decimal userID)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            object characterObj = null;
            string query = @"SELECT " + objColumn + " FROM Character WHERE userID = @UserID AND charName = @CharName";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@CharName", charName);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                    characterObj = sqlDataReader.GetValue(0);
                return JsonConvert.SerializeObject(characterObj);
                /*
                This web service:
                    +Retrieves a dataset from our database
                    +Uses JsonConvert.SerializeObject() to serialize the object and returns that data to a user's local application as a string
                Then the client's local application:
                    +Uses JsonConvert.DeserializeObject<DataSet>() to convert the object back into a System.Data.DataSet
                 */
            }
            catch (Exception ex)
            {
                characterObj = ex.Message;
                return JsonConvert.SerializeObject(characterObj);
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static decimal GetStageExp(string charName, decimal userID)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT stageExp FROM Character WHERE charName = @CharName AND userID = @UserID";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@CharName", charName);
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                decimal stageExp = -1;
                while (sqlDataReader.Read())
                    stageExp = sqlDataReader.GetDecimal(0);
                return stageExp;
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static string GetUpdated(string charName, decimal userID)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            DateTime? updated = null;
            string query = @"SELECT updated FROM Character WHERE charName = @CharName AND userID = @UserID";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@CharName", charName);
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                    updated = sqlDataReader.GetDateTime(0);
                return JsonConvert.SerializeObject(updated);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(updated + ex.Message);
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static decimal GetUserIdbyUserName(string userName)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT userId FROM Account WHERE userName = @userName";
            
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userName", userName);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                decimal userId = -1;
                while (sqlDataReader.Read())
                    userId = sqlDataReader.GetDecimal(0);
                return userId;
            }
            catch (Exception)
            {
                return -2;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static List<string> GetAllItemIdByUsername(string auth_token)
        {            
            // Get userId from the userName
            decimal userId = GetUserIDInAuthToken(auth_token);
            // Get all item Id from Item table by userId
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT itemId FROM Item WHERE userId = @userId";
            List<string> allItemIds = new List<string>();
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userId", userId);
                SqlDataReader sqlDataReader = command.ExecuteReader();

                while (sqlDataReader.Read())
                    allItemIds.Add(sqlDataReader.GetString(0));
                return allItemIds;
            }
            catch (Exception ex)
            {

                allItemIds.Add("Exception: GetAllItemIdByUsername: " + ex.Message.ToString());
                return allItemIds;
            }
            finally
            {
                sqlConn.Close();
            }
        }

 
        public static Item GetItem(string itemID, string auth_token)
        {
            decimal userID = GetLoggedInUserID(auth_token);
            //check if the itemID is belong to the userID
            if (!itemID.Equals(GetAllItemsIDBelongToUser(auth_token).Where(x => x.Contains(itemID)).FirstOrDefault()))
                return null;

            return new Item()
            {
                UserID = userID,
                ItemID = itemID,
                Quantity = GetItemQuantity(userID, itemID)
            };

        }

        public static decimal GetItemQuantity(decimal userID, string itemID)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            decimal quantity = -1;
            string query = @"SELECT quantity FROM Item WHERE itemID = @ItemID AND userID = @UserID";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@ItemID", itemID);
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                    quantity = sqlDataReader.GetDecimal(0);
                return quantity;
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static bool IsItemIdExistInUserId(string auth_token, string itemId)
        {
            decimal userId = GetUserIDInAuthToken(auth_token);
            SqlConnection sqlConn = ObtainConnectionString();
            bool isExist = false;
            string query = @"SELECT itemId FROM Item WHERE userID = @userId";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userId", userId);               
                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read()) {
                    if (itemId == sqlDataReader.GetString(0))
                        isExist = true;
                }
                return isExist;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static bool UpdateItemQuantityByAuthToken(string auth_token, string itemId, decimal quantity)
        {
            decimal userId = GetUserIDInAuthToken(auth_token); 
            SqlConnection sqlConn = ObtainConnectionString();            
            string query = @"UPDATE Item SET quantity= @quantity WHERE userId = @userId AND itemId = @itemId";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@itemId", itemId);
                command.Parameters.AddWithValue("@quantity", quantity);

                return (command.ExecuteNonQuery() != 0);               
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static bool SaveAllItemIdbyUsername(string auth_token, Dictionary<string, decimal> itemIdList)
        {
            // Get userId from the userName
            decimal userId = GetUserIDInAuthToken(auth_token);
            bool sucess;
            // Get all item Id from Item table by userId
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"INSERT INTO Item VALUES (@userId, @itemId, @quantity)";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();     
                }
                SqlCommand command = new SqlCommand(query, sqlConn);                
                command.Parameters.Add("@userId", SqlDbType.Decimal);
                command.Parameters.Add("@itemId", SqlDbType.VarChar);
                command.Parameters.Add("@quantity", SqlDbType.Decimal);
                foreach (KeyValuePair<string, decimal> pair in itemIdList)
                {
                    if (IsItemIdExistInUserId(auth_token, pair.Key))
                    {
                        if (!UpdateItemQuantityByAuthToken(auth_token, pair.Key, pair.Value))
                            return false;
                        continue;
                    }                   
                    command.Parameters["@userId"].Value = userId;
                    command.Parameters["@itemId"].Value = pair.Key;
                    command.Parameters["@quantity"].Value = pair.Value;
                    
                    sucess = command.ExecuteNonQuery() != 0;
                    if (!sucess)
                        return false;
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static Byte[] GetStageByCharName(string auth_token, string charName)
        {
            decimal userId = GetUserIDInAuthToken(auth_token);
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT stage FROM Character WHERE userId = @userId AND charName = @charName";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@charName", charName);
                Byte[] stage = new Byte[UInt16.MaxValue];
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    stage = reader.GetSqlBinary(0).Value;
                return stage;
            }
            catch (Exception)
            {
                
                return new Byte[5];
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static bool SaveBalanceByUserId(string auth_token, decimal balance)
        {
            decimal userId = GetUserIDInAuthToken(auth_token);
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"UPDATE Account SET balance = @balance WHERE userId = @userId";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@balance", balance);


                return (command.ExecuteNonQuery() != 0);
            }
            catch (Exception)
            {

                return false;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static List<string> SearchByCharName(string charName)
        {            
            SqlConnection sqlConn = ObtainConnectionString();
            List<string> name = new List<string>();
            string query = @"SELECT charName FROM Character WHERE charName LIKE '%' + @charName + '%' ";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@charName", charName);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    name.Add(reader.GetString(0));                
                return name;                
            }
            catch (Exception ex)
            {
                name.Add(ex.Message);
                return name;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static List<string> SearchByUserName(string userName)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            List<string> name = new List<string>();
            string query = @"SELECT userName FROM Account WHERE userName LIKE '%' + @userName + '%' ";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userName", userName);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    name.Add(reader.GetString(0));
                return name;
            }
            catch (Exception ex)
            {
                name.Add(ex.Message);
                return name;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public static decimal SearchByUserNameCharName(string userName, string charName)
        {
            decimal userId = GetUserIdbyUserName(userName);
            SqlConnection sqlConn = ObtainConnectionString();
            decimal exp = -1;
            string query = @"SELECT stageExp FROM Character WHERE userId = @userId AND charName = @charName";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@charName", charName);
                SqlDataReader reader = command.ExecuteReader();                
                while (reader.Read())
                    exp = reader.GetDecimal(0);
                return exp;
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        //public static List<SearchCharacter> SearchCharacterByUserNameCharName(string userName, string charName)
        //{
        //    List<string> user = SearchByUserName(userName);
        //    List<string> character = SearchByCharName(charName);
        //    SqlConnection sqlConn = ObtainConnectionString();
        //    List<SearchCharacter> searchCharacter = new List<SearchCharacter>();

        //    foreach (string nameUser in user)
        //    {
        //        foreach (string nameChar in character)
        //        {
        //            if (SearchByUserNameCharName(nameUser, nameChar) != -1)
        //            {
        //                searchCharacter.Add(new SearchCharacter()
        //                {
        //                    UserName = nameUser,
        //                    CharName = nameChar,
        //                    Exp = SearchByUserNameCharName(nameUser, nameChar)
        //                });
        //            }
        //        }
        //    }
        //    return searchCharacter;
        //}

        //public static List<SearchCharacter> SearchCharacterByUserNameCharName()
        //{
        //    string query = "SELECT (userId, charName, stageExp) FROM Character";
        //    List<SearchCharacter> searchCharacters = new List<SearchCharacter>();       
        //    using (SqlConnection conn = ObtainConnectionString())
        //    {
        //        SqlCommand command = new SqlCommand(query, conn);
        //        SqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            searchCharacters.Add(new SearchCharacter()
        //            {
        //                UserName = reader.GetDecimal(0).ToString(),
        //                CharName = reader.GetString(1),
        //                Exp = reader.GetDecimal(2)
        //            });
        //        }
        //    }
        //    return searchCharacters;
        //}

        //public static List<SearchCharacter> SearchCharacterByUserNameCharName(string userName, string charName)
        //{
        //    string query;
        //    if (string.IsNullOrEmpty(userName))
        //    {
        //        query = "SELECT (userId, charName, stageExp) FROM Character WHERE charName LIKE %"
        //    }
        //}

    }
}