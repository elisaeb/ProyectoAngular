using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnocasa2024.Shared
{
    public class Coordenadas
    {
        [BsonElement("latitud")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Latitud { get; set; } = 0;
        
        [BsonElement("longitud")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Longitud { get; set; } = 0;

    }
}
