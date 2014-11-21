using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFServiceApp
{
    public class SearchCharacter
    {
        [DataMember(Name = "UserName")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        private string userName;

        [DataMember(Name = "CharName")]
        public string CharName
        {
            get { return charName; }
            set { charName = value; }
        }
        private string charName;

        [DataMember(Name = "Exp")]
        public decimal Exp
        {
            get { return exp; }
            set { exp = value; }
        }
        private decimal exp;
    }
}