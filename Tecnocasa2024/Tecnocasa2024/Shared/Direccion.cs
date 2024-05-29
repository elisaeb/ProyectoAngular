using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnocasa2024.Shared
{
    public class Direccion
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string IdDireccion { get; set; } = "";

        [BsonElement("calle")]
        public string Calle { get; set; } = "";

        [BsonElement("numero")]
        public string Numero { get; set; } = "";

        [BsonElement("planta")]
        public string Planta { get; set; } = "";

        [BsonElement("letra")]
        public string Letra { get; set; } = "";

        [BsonElement("cp")]
        public int CP { get; set; } = 0;

        [BsonElement("provincia")]
        public Provincia ProvDirec { get; set; } = new Provincia();

        [BsonElement("municipio")]
        public Municipio MunDirec { get; set; } = new Municipio();

        [BsonElement("barrio")]
        public Barrio BarrioDirec { get; set; } = new Barrio();
    }
}
