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

        /// <summary>
        /// Return all item Id by user name.
        /// </summary>
        /// <param name="username"></param>        
        /// <returns>List of all item Id belong to the user</returns>
        List<string> IRegisterService.GetAllItemIdByUsername(string username)
        {
            return _SharedClass.GetAllItemIdByUsername(username);
        }

        /// <summary>
        /// Save all item ids by user name
        /// </summary>
        /// <param name="username"></param>        
        /// <returns>List of all item Id belong to the user</returns>
        string IRegisterService.SaveAllItemIdByUsername(string username,Dictionary<string, decimal> itemIdList)
        {
            return _SharedClass.SaveAllItemIdbyUsername(username, itemIdList);
        }
    }
}
