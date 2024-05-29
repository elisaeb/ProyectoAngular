using MongoDB.Bson;
using Tecnocasa2024.Shared;

namespace Tecnocasa2024.Client.Models.Interfaces
{
    public interface IAjaxServices
    {
        Task<String> LoginGoogle();
        Task<RESTMessage> LoginRegistro(string email, string password);
        Task<ResultadoBusquedasGEOAPI> BuscarGeoApi(string texto);
        Task<RESTMessage> BuscarInmuebles(string cpro, string nompro, string cmum, string nommun, string cpos, string nombar);
        Task<String> RecuperarProvincia(string cpro);
        Task<String> RecuperarMunicipio(string cpro, string cmum);
        Task<Inmueble> RecuperarInmueble(string idinmu);
        Task<bool> Logout(List<string> lista, string idcliente);
        Task<bool> EmailFormulario(string mensaje,string nombre, string apellido, string tel, string email);
    }
}
