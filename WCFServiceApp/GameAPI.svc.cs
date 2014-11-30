using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;

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


        Character IGameAPI.GetCharacter(string charName)
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.GetCharacter(charName, applicationheader);
        }


        Item IGameAPI.GetItem(string itemID)
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.GetItem(itemID, applicationheader);
        }


        List<string> IGameAPI.GetAllItemsIDBelongToUser()
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.GetAllItemsIDBelongToUser(applicationheader);
        }

        /// <summary>
        /// Return all item Id by user name.
        /// </summary>
        /// <param name="username"></param>        
        /// <returns>List of all item Id belong to the user</returns>
        List<Item> IGameAPI.GetAllItembyUserId()
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.GetAllItemByUserId(applicationheader);
        }

        /// <summary>
        /// Save all item ids by user name
        /// </summary>
        /// <param name="username"></param>        
        /// <returns>List of all item Id belong to the user</returns>
        bool IGameAPI.SaveItemByUserId(string itemid, decimal quantity)
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.SaveItemByUserId(applicationheader, itemid, quantity);
        }

        /// <summary>
        /// Get the stage from character name
        /// </summary>
        /// <param name="charname"></param>        
        /// <returns>List of all item Id belong to the user</returns>
        string IGameAPI.GetStageByCharName(string charName)
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.GetStageByCharName(applicationheader, charName);
        }

        bool IGameAPI.SaveBalanceByUserId(decimal balance)
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.SaveBalanceByUserId(applicationheader, balance);
        }
        List<SearchCharacter> IGameAPI.SearchCharacterByUserNameCharName(string username, string charname)
        {
            return _SharedClass.SearchCharacterByUserNameCharName(username, charname);
        }

        List<SearchCharacter> IGameAPI.SearchCharacter()
        {
            return _SharedClass.SearchCharacter();
        }

        List<SearchCharacter> IGameAPI.SearchCharacterByUserName(string userName)
        {
            return _SharedClass.SearchCharacterByUserName(userName);
        }

        List<SearchCharacter> IGameAPI.SearchCharacterByCharName(string charName)
        {
            return _SharedClass.SearchCharacterByCharName(charName);
        }

        bool IGameAPI.CreateNewCharacter(string charName, string character, string stage, decimal stageExp)
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.CreateNewCharacter(applicationheader, charName, character, stage, stageExp);
        }
        
        List<SCharacter> IGameAPI.GetAllCharacter()
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.GetAllCharacter(applicationheader);
        }

        bool IGameAPI.EditCharacter(string charName, string character, string stage, decimal stageExp)
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.EditCharacter(applicationheader, charName, character, stage, stageExp);
        }

        bool IGameAPI.DeleteCharacter(string charName)
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.DeleteCharacter(applicationheader, charName);
        }

        bool IGameAPI.UpdateCharacterByCharName(string charName, string character)
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.UpdateCharacterbyCharName(applicationheader, charName, character);
        }

        bool IGameAPI.UpdateStageByCharName(string charName, string stage)
        {
            IncomingWebRequestContext iwrc = WebOperationContext.Current.IncomingRequest;
            string applicationheader = iwrc.Headers["X-Auth-Token"];
            return _SharedClass.UpdateStagebyCharName(applicationheader, charName, stage);
        }
    }
}
