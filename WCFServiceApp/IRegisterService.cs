using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceApp
{
    [ServiceContract]
    public interface IRegisterService
    {
        /// <summary>
        /// Register a new user with user name, email and password.
        /// </summary>
        /// <param name="username">The name that the user want to register</param>
        /// <param name="password">Password for this user</param>
        /// <param name="email">Email that the user want to register</param>
        /// <returns>True if successfully register a new user.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "/player")]
        bool RegisterNewUser(string username, string password, string email);
        
    }
}
