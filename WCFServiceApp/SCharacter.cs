using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace WCFServiceApp
{
    public class SCharacter
    {        
        [DataMember(Name = "CharName")]
        public string CharName
        {
            get { return _charName; }
            set { _charName = value; }
        }
        private string _charName;

        [DataMember(Name = "Character")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "Character")]
        public string CharacterObj
        {
            get { return _character; }
            set { _character = value; }
        }
        private string _character;

        [DataMember(Name = "Stage")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "Stage")]
        public string Stage
        {
            get { return _stage; }
            set { _stage = value; }
        }
        private string _stage;

        [DataMember(Name = "Stage Exp")]
        public decimal StageExp
        {
            get { return _stageExp; }
            set { _stageExp = value; }
        }
        private decimal _stageExp;        
    }
}