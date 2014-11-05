using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFServiceApp
{
    public class Character
    {
        [DataMember(Name = "Id")]
        public decimal Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private decimal _id;

        [DataMember(Name = "CharName")]
        public string CharName 
        {
            get { return _charName; }
            set { _charName = value; }
        }
        private string _charName;

        [DataMember(Name = "Character")]
        public object CharacterObj 
        {
            get { return _character; }
            set { _character = value; }
        }
        private object _character;

        [DataMember(Name = "Stage")]
        public object Stage
        {
            get { return _stage; }
            set { _stage = value; }
        }
        private object _stage;

        [DataMember(Name = "Stage Exp")]
        public decimal StageExp 
        {
            get { return _stageExp; }
            set { _stageExp = value; }
        }
        private decimal _stageExp;

        [DataMember(Name = "Updated")]
        public DateTime Updated 
        {
            get { return _updated; }
            set { _updated = value; }
        }
        private DateTime _updated;
    }
}