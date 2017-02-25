using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VechimeSoftware
{
    public class UserData
    {
        public ObjectId _id { get; set; }
        public string Username { get; set; }
        public string Passhash { get; set; }
        public string Email { get; set; }
        public string HWID { get; set; }
        public string Serial { get; set; }
    }
}
