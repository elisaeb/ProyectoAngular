using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnocasa2024.Shared
{
    public class Oficina
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string IdOficina { get; set; } = "";

        [BsonElement("idAgenteEncargado")]
        public string IdAgenteEncargado { get; set; } = "";

        [BsonIgnore]
        public Agente AgenteEncargado { get; set; } = new Agente();

        [BsonElement("telefonos")]
        public List<string> Telefonos { get; set; } = new List<string>();

        [BsonElement("emails")]
        public List<string> Emails { get; set; } = new List<string>();

        [BsonElement("direccion")]
        public Direccion DirecOficina { get; set; } = new Direccion();

        [BsonElement("listaAgentes")]
        public List<ObjectId> ListaAgentes { get; set; } = new List<ObjectId>();

        [BsonIgnore]
        public List<Agente> AgentesExpandidos { get; set; } = new List<Agente>();

        [BsonElement("listaInmuebles")]

        public List<ObjectId> ListaInmuebles { get; set; } = new List<ObjectId>();

    }
}
