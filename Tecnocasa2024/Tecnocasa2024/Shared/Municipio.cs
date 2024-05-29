using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tecnocasa2024.Shared
{
    public class Municipio
    {
        [BsonElement("CMUM")]
        public String CMUM { get; set; } = "";

        [BsonElement("CPRO")]
        public String CPRO { get; set; } = "";

        [BsonElement("CUN")]
        public String CUN { get; set; } = "";

        [BsonElement("DMUN50")]
        public String DMUN50 { get; set; } = "";
    }
}
