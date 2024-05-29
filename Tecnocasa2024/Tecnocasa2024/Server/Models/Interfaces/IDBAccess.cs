using Tecnocasa2024.Shared;
using MongoDB.Bson;
namespace Tecnocasa2024.Server.Models.Interfaces
{
    public interface IDBAccess
    {
        //metodos interface q tienen q cumplir los servicios de acceso a BD (ya sea sqlserver, mongodb, oracle, mysql,...)
        Task<Cliente>ComprobarCredenciales(string email, string password);
        Task<bool>RegistrarCliente(string email, string password);
        Task<List<Inmueble>> RecuperarInmuebles(string cpro, string nompro, string cmum, string nommun, string cpos, string nombar);
        Task<Inmueble> RecuperarInmueble(string idinmu);
        Task<bool> GuardarListaFavs(List<string> lista, string idcliente);
    }
}
