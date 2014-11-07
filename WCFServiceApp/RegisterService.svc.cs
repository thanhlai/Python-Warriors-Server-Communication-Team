using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServiceApp
{
    public class RegisterService : IRegisterService
    {
        /// <summary>
        /// Returns true if the new user is added successfully. Otherwise, returns false
        /// to the client
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        bool IRegisterService.RegisterNewUser(string username, string password, string email)
        {
            return _SharedClass.RegisterNewUser(username, _SharedClass.HashPassword(password), email);
        }
        
    }
}
