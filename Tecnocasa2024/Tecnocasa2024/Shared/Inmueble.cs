using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnocasa2024.Shared
{
    public class Inmueble
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string IdInmueble { get; set; } = "";

        [BsonElement("idAgente")]
        public string IdAgente { get; set; } = "";

        [BsonIgnore]
        public Agente Agente { get; set; } = new Agente();

        [BsonElement("idOficina")]
        public string IdOficina { get; set; } = "";

        [BsonIgnore]
        public Oficina Oficina { get; set; } = new Oficina();

        

        [BsonElement("tipo")]
        public string Tipo { get; set; } = "";

        [BsonElement("anioConstruccion")]
        public int AñoConstruccion { get; set; } = 0;

        [BsonElement("superficieTotal")]
        public Decimal SuperficieTotal{ get; set; } = 0;

        [BsonElement("superficieHabitable")]
        public Decimal SuperficieHabitable { get; set; } = 0;

        [BsonElement("precio")]
        public Decimal Precio { get; set; } = 0;

        [BsonElement("coordenadas")]
        public Coordenadas CoordsInmueble { get; set; }=new Coordenadas();

        [BsonElement("numHabitaciones")]
        public int NumeroHabitaciones { get; set; } = 0;

        [BsonElement("numBanios")]
        public int NumeroBaños { get; set; } = 0;

        [BsonElement("estado")]
        public string Estado { get; set; } = "";

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = "";

        [BsonElement("galeriaFotos")]
        public List<string> GaleriaFotos { get; set; } = new List<string>();

        [BsonElement("direccion")]
        public Direccion DirecInmueble{ get; set; } = new Direccion();

        [BsonElement("aireAcondicionado")]
        public string AireAcondicionado { get; set; } = "";

        [BsonElement("ascensor")]
        public bool Ascensor { get; set; } = true;

        [BsonElement("calefaccion")]
        public string Calefaccion { get; set; } = "";

    }
}
