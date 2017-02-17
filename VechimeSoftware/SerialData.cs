using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VechimeSoftware
{
    public class SerialData
    {
        public ObjectId _id { get; set; }
        public string Serial { get; set; }
    }
}
