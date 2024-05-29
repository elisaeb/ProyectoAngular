using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnocasa2024.Shared
{
    public class Agente
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string IdAgente { get; set; } = "";

        [BsonElement("nombre")]
        public string Nombre { get; set; } = "";

        [BsonElement("apellidos")]
        public string Apellidos { get; set; } = "";

        [BsonElement("tipo")]
        public string Tipo { get; set; } = ""; //franquiciado o colaborador

        [BsonElement("fotoAgente")]
        public string Foto { get; set; } = "";

        [BsonElement("telefonos")]
        public List<string> Telefonos { get; set; } = new List<string>();

        [BsonElement("direccionPersonal")]
        public Direccion DireccionPersonal { get; set; } = new Direccion();

        [BsonElement("idOficina")]
        public ObjectId IdOficina { get; set; }

        [BsonElement("carteraInmuebles")]
        public List<ObjectId> CarteraInmuebles { get; set; } = new List<ObjectId>();
        [BsonElement("email")]
        public string Email { get; set; } = "";

    }
}
