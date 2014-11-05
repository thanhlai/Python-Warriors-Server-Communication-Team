using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFServiceApp
{
    [DataContract]
    public class Player
    {
        [DataMember(Name = "Id")]
        public decimal Id 
        {
            get { return _id; }
            set { _id = value; }
        }
        private decimal _id;

        [DataMember(Name = "Username")]
        public string Username 
        {
            get { return _username; }
            set { _username = value; }
        }
        private string _username;

        [DataMember(Name = "Email")]
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        private string _email;
       
        [DataMember(Name = "PasswordHash")]
        public string PasswordHash
        {
            get { return _passwordHash; }
            set { _passwordHash = value; }
        }
        private string _passwordHash;

        [DataMember(Name = "Balance")]
        public decimal Balance
        {
            get { return _balance; }
            set { _balance = value; }
        }
        private decimal _balance;
    }
}