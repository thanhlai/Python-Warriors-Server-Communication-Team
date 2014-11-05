using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceApp
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GameAPI" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select GameAPI.svc or GameAPI.svc.cs at the Solution Explorer and start debugging.
    public class GameAPI : IGameAPI
    {

        List<string> IGameAPI.GetAllAvailableCharacterNames()
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;

            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.GetAllAvailableCharacterNames(applicationheader);    //it is auth_token
        }
    }
}
