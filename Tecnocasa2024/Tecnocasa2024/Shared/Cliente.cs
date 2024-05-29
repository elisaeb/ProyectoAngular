using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tecnocasa2024.Shared
{
    public class Cliente
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string IdCliente { get; set; }


        [BsonElement("nombre")]
        public string Nombre { get; set; } = "";

        [BsonElement("apellidos")]
        public string Apellidos { get; set; } = "";

        [BsonElement("email")]
        public string Email { get; set; } = "";

        [BsonElement("password")]
        public string Password { get; set; } = "";

        [BsonElement("cuentaActiva")]
        public Boolean CuentaActiva { get; set; } = false;

        [BsonElement("telefono")]
        public string Telefono { get; set; } = "000 11 22 33";

        [BsonElement("genero")]
        public string Genero { get; set; } = "Indefinido";

        //[BsonIgnore] <---- atributo si no quieres mapear una prop. de la clase contra una prop.de algun docum.de coleccion "clientes"
        [BsonElement("direcciones")]
        public List<Direccion> Direcciones { get; set; } = new List<Direccion>();

        [BsonElement("listaFavoritos")]
        public List<ObjectId> ListaFavoritos { get; set; } = new List<ObjectId>();

        [BsonIgnore]
        public List<Inmueble> FavoritosExpandidos { get; set; } = new List<Inmueble>();

        [BsonElement("misInmuebles")]
        public List<ObjectId> MisInmuebles{ get; set; } = new List<ObjectId>();

        [BsonIgnore]
        public List<Inmueble> InmueblesExpandidos { get; set; } = new List<Inmueble>();

    }
}
