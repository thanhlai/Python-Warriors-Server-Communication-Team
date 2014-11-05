using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
                return new Player() { Username = "INVALID"};

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
            decimal userID  = GetDecimalFromAccountTable("userID", username, HashPassword(password));
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
                    allCharacterNames.Add(sqlDataReader.GetString(0)) ;
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

    }
}