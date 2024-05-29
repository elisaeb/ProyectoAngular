using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tecnocasa2024.Shared
{
    public class Provincia
    {
        [BsonElement("CCOM")]
        public String CCOM { get; set; } = "";

        [BsonElement("CPRO")]
        public String CPRO { get; set; } = "";

        [BsonElement("PRO")]
        public String PRO { get; set; } = "";
    }
}
