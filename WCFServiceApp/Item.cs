using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFServiceApp
{
    public class Item
    {
        [DataMember(Name = "UserID")]
        public decimal UserID
        {
            get { return _userID; }
            set { _userID = value; }
        }
        private decimal _userID;


        [DataMember(Name = "ItemID")]
        public string ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }
        private string _itemID;

        [DataMember(Name = "Quantity")]
        public decimal Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        private decimal _quantity;
    }
}