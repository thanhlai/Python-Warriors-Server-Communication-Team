﻿using Newtonsoft.Json;
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
                return null;

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
        public static bool ChangePassword(string auth_token, string newPassword)
        {
            //Retrieve userId from auth token table
            decimal userID = GetUserIDInAuthToken(auth_token);

            SqlConnection sqlConn = ObtainConnectionString();            
            string query = @"UPDATE Account SET password= @NewPasswordHash WHERE userID = @userId";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userId", userID);
                command.Parameters.AddWithValue("@NewPasswordHash", newPassword);   //password is already hashed

                return (command.ExecuteNonQuery() > 0);               
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
        public static bool RegisterNewUser(string username, string passwordHash, string email)
        {
            if (IsUserNameExistInAccount(username))
                return false;
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

        public static bool IsUserNameExistInAccount(string username)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT userID FROM Account WHERE userName = @Username";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@Username", username);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                decimal _userID = -1;
                while (sqlDataReader.Read())
                    _userID = sqlDataReader.GetDecimal(0);  // different than -1, meaning the user already exists
                return (decimal.Compare(_userID, -1) != 0);
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
                {
                    allItemIDs.Add(sqlDataReader.GetString(0));
                }
                    
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

        public static List<Item> GetAllItemByUserId(string auth_token)
        {            
            // Get userId from the userName
            decimal userId = GetUserIDInAuthToken(auth_token);
            // Get all item Id from Item table by userId
            SqlConnection sqlConn = ObtainConnectionString();
            string query = @"SELECT itemId, quantity FROM Item WHERE userId = @userId";
            List<Item> allItems = new List<Item>();
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
                    allItems.Add(new Item()
                    {
                        UserID = userId,
                        ItemID = sqlDataReader.GetString(0),
                        Quantity = sqlDataReader.GetDecimal(1)
                    });
                return allItems;
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

        public static bool IsItemIdExistInUserId(decimal userId, string itemId)
        {           
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

        public static bool UpdateItemQuantityByAuthToken(decimal userId, string itemId, decimal quantity)
        {           
            SqlConnection sqlConn = ObtainConnectionString();            
            string query = @"UPDATE Item SET quantity= @quantity WHERE userID = @userId AND itemID = @itemId";
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

                return (command.ExecuteNonQuery() > 0);               
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

        public static bool SaveAllItemsByUserId(string auth_token, List<Item> itemList)
        {
            // Get userId from the userName
            decimal userId = GetUserIDInAuthToken(auth_token);
            // Get all item Id from Item table by userId
            SqlConnection sqlConn = ObtainConnectionString();
            bool sucess;
            string query = @"INSERT INTO Item VALUES (@userId, @itemId, @quantity);";
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
                foreach (Item item in itemList)
                {                    
                    if (IsItemIdExistInUserId(userId, item.ItemID))
                    {
                        if (!UpdateItemQuantityByAuthToken(userId, item.ItemID, item.Quantity))
                            return false;
                        continue;
                    }
                    else
                    {
                        command.Parameters["@userId"].Value = userId;
                        command.Parameters["@itemId"].Value = item.ItemID;
                        command.Parameters["@quantity"].Value = item.Quantity;

                        sucess = command.ExecuteNonQuery() != 0;
                        if (!sucess)
                            return false;                        
                    }                    
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

        public static string GetStageByCharName(string auth_token, string charName)
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
                string stage = "";
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    stage = reader.GetString(0);
                return stage;
            }
            catch (Exception ex)
            {                
                return ex.Message;
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

        public static List<SearchCharacter> SearchCharacterByCharName(string charName)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            List<SearchCharacter> resultList = new List<SearchCharacter>();
            string query = @"SELECT DISTINCT Account.userName, Character.charName, Character.stageExp FROM Character INNER JOIN Account ON Character.userId = Account.userId WHERE charName LIKE @charname ";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@charname", "%" + charName + "%");
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    resultList.Add(new SearchCharacter()
                    {
                        UserName = reader.GetString(0),
                        CharName = reader.GetString(1),
                        Exp = reader.GetDecimal(2)
                    });
                }

                return resultList;
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

        public static List<SearchCharacter> SearchCharacterByUserName(string userName)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            List<SearchCharacter> resultList = new List<SearchCharacter>();
            string query = @"SELECT DISTINCT Account.userName, Character.charName, Character.stageExp FROM Character INNER JOIN Account ON Character.userId = Account.userId WHERE userName LIKE @username ";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@username", "%" + userName + "%");                
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    resultList.Add(new SearchCharacter()
                    {
                        UserName = reader.GetString(0),
                        CharName = reader.GetString(1),
                        Exp = reader.GetDecimal(2)
                    });
                }

                return resultList;
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

        public static List<SearchCharacter> SearchCharacterByUserNameCharName(string userName, string charName)
        {            
            SqlConnection sqlConn = ObtainConnectionString();
            List<SearchCharacter> resultList = new List<SearchCharacter>();
            string query = @"SELECT DISTINCT Account.userName, Character.charName, Character.stageExp FROM Character INNER JOIN Account ON Character.userId = Account.userId WHERE userName LIKE @username AND charName LIKE @charname ";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@username","%" + userName + "%");
                command.Parameters.AddWithValue("@charname", "%" + charName + "%");
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    resultList.Add(new SearchCharacter()
                    {
                        UserName = reader.GetString(0),
                        CharName = reader.GetString(1),
                        Exp = reader.GetDecimal(2)
                    });
                }

                return resultList;
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

        public static List<SearchCharacter> SearchCharacter()
        {
            SqlConnection sqlConn = ObtainConnectionString();
            List<SearchCharacter> resultList = new List<SearchCharacter>();
            string query = @"SELECT DISTINCT Account.userName, Character.charName, Character.stageExp FROM Character INNER JOIN Account ON Character.userId = Account.userId ";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);                
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    resultList.Add(new SearchCharacter()
                    {
                        UserName = reader.GetString(0),
                        CharName = reader.GetString(1),
                        Exp = reader.GetDecimal(2)
                    });
                }

                return resultList;
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


        public static bool CreateNewCharacter(string auth_token, string charName, string character, string stage, decimal stageExp)
        {
            decimal userId = GetUserIDInAuthToken(auth_token);
            SqlConnection sqlConn = ObtainConnectionString();
            List<SearchCharacter> resultList = new List<SearchCharacter>();
            string query = @"INSERT INTO Character VALUES (@charName, @userId, @character, @stage, @stageExp, @updated)";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);

                command.Parameters.AddWithValue("@charName", charName);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@character", character);
                command.Parameters.AddWithValue("@stage", stage);
                command.Parameters.AddWithValue("@stageExp", stageExp);
                command.Parameters.AddWithValue("@updated", DateTime.Now);


                return (command.ExecuteNonQuery() != 0) ;
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

        public static List<SCharacter> GetAllCharacter(string authToken)
        {
            decimal userId = GetUserIDInAuthToken(authToken);
            SqlConnection sqlConn = ObtainConnectionString();
            List<SCharacter> resultList = new List<SCharacter>();
            string query = @"SELECT charName, character, stage, stageExp FROM Character WHERE userID = @userId";
            try
            {
                if (sqlConn.State == ConnectionState.Closed)
                {
                    sqlConn.Open();
                }
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userId", userId);
                SqlDataReader reader = command.ExecuteReader();              
                while (reader.Read())
                {                    
                    resultList.Add(new SCharacter()
                    {
                        CharName = reader.GetString(0),
                        CharacterObj = reader.GetString(1),
                        Stage = reader.GetString(2),
                        StageExp = reader.GetDecimal(3)
                    });
                }

                return resultList;
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

        public static bool EditCharacter(string auth_token, string charName, string character, string stage, decimal stageExps)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            decimal userId = GetUserIDInAuthToken(auth_token);
            string query = "UPDATE Character SET character = @character, stage = @stage, stageExp = @stageExp, updated = @updated WHERE userID = @userID AND charName = @charName";
            try
            {
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@charName", charName);
                command.Parameters.AddWithValue("@character", character);
                command.Parameters.AddWithValue("@stage", stage);
                command.Parameters.AddWithValue("@stageExp", stageExps);
                command.Parameters.AddWithValue("@updated", DateTime.Now);
                command.Parameters.AddWithValue("@userID", userId);

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

        public static bool DeleteCharacter(string auth_token, string charName)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            decimal userId = GetUserIDInAuthToken(auth_token);
            string query = "DELETE FROM Character WHERE userID = @userID AND charName = @charName";
            try
            {
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userID", userId);
                command.Parameters.AddWithValue("@charName", charName);
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

        public static bool UpdateCharacterbyCharName(string auth_token, string charName, string character)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            decimal userId = GetUserIDInAuthToken(auth_token);
            string query = "UPDATE Character SET character = @character WHERE userID = @userID AND charName = @charName";
            try
            {               
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@charName", charName);
                command.Parameters.AddWithValue("@character", character);

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

        public static bool UpdateStagebyCharName(string auth_token, string charName, string stage)
        {
            SqlConnection sqlConn = ObtainConnectionString();
            decimal userId = GetUserIDInAuthToken(auth_token);
            string query = "UPDATE Character SET stage = @stage WHERE userID = @userID AND charName = @charName";
            try
            {
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@charName", charName);
                command.Parameters.AddWithValue("@stage", stage);

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

    }
}