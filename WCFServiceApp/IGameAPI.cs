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
    public interface IGameAPI
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "gameapi/getallavailablecharacternames")]
        List<string> GetAllAvailableCharacterNames();


        [OperationContract]
        [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "gameapi/getcharacter/{charName}")]
        Character GetCharacter(string charName);

        [OperationContract]
        [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "gameapi/getitem/{itemID}")]
        Item GetItem(string itemID);



        [OperationContract]
        [WebInvoke(Method = "GET",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "gameapi/getallavailableitems")]
        List<string> GetAllItemsIDBelongToUser();

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "gameapi/getallitemidbyusername")]
        List<string> GetAllItemIdByUsername();

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "gameapi/saveallitemidbyusername")]
        bool SaveAllItemIdByUsername(Dictionary<string, decimal> itemIdList);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "gameapi/getstagebycharname/{charname}")]
        Byte[] GetStageByCharName(string charName);

        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "gameapi/savebalancebyuserid")]
        bool SaveBalanceByUserId(decimal balance);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "gameapi/searchcharacterbyusernamecharname/{username}/{charname}")]
        List<SearchCharacter> SearchCharacterByUserNameCharName(string username, string charname);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "gameapi/searchcharacter")]
        List<SearchCharacter> SearchCharacter();

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "gameapi/searchcharacterbyusername/{username}")]
        List<SearchCharacter> SearchCharacterByUserName(string userName);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "gameapi/searchcharacterbycharname/{charname}")]
        List<SearchCharacter> SearchCharacterByCharName(string charName);
       
        [OperationContract]
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "gameapi/createnewcharacter")]
        bool CreateNewCharacter(string charName, string character, string stage, decimal stageExp);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "gameapi/getallcharacter")]
        List<SCharacter> GetAllCharacter();
        
    }
}
