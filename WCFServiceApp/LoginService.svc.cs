using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;

namespace WCFServiceApp
{
    public class LoginService : ILoginService
    {
        /// <summary>
        /// Receive the raw password from the client, hash it and then compare to the hashed password in the Account table
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Player ILoginService.SendToLoginValidation(string username, string password)
        {
            Player player = LoginValidation(username, _SharedClass.HashPassword(password));
            if (player.PasswordHash.Equals(null))
                return null;
            return player;
        }
        /// <summary>
        /// Access to the database and retrieve the row (all the information) that 
        /// has the provided username with the secure hashed password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        Player LoginValidation(string username, string passwordHash)
        {
            return (_SharedClass.LoginValidation(username, passwordHash));
                //return the valid player info here
                //can be used to handle data glitch, such as when the balance is invalid (for some reasons)
                //returns a new user data with balance of 1 (or any other characteristics)
        }


        string ILoginService.Login(string username, string password)
        {
            string theUsername = _SharedClass.LoginValidation(username, _SharedClass.HashPassword(password)).Username;

            if (theUsername.Equals("INVALID"))
                return null;
            return _SharedClass.GenerateAuthToken(username, password);
        }
    }
}
