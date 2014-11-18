using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceApp
{

    [ServiceContract(Namespace="WCFServiceApp/JSONData")]
    public interface ILoginService
    {



        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle =  WebMessageBodyStyle.Wrapped,
            UriTemplate = "/login")]
        string Login(string username, string password);


        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "player/{username}/{password}")]
        Player SendToLoginValidation(string username, string password);

      }
}
