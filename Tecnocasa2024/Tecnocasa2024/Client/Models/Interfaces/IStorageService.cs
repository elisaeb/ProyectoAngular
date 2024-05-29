using Tecnocasa2024.Shared;

namespace Tecnocasa2024.Client.Models.Interfaces
{
    /* ... usando genericos para evitar repeticion de codigo: ..
        AlmacenarCliente ( Cliente cliente, string clave)
        AlmacenarJWT (     string jwt,      string clave)
        AlmacenarImuebles( List<Inmueble>,  string clave)
        ....
        -------------------------------------------------
        con un metodo generico simplifico todo:

        AlmacenarDatos<T>( T datos, string clave ) <------- AlmacenarDatos<Cliente>(Cliente datos, string clave)
						        |-- AlmacenarDatos<string> (string datos, string clave)
						            AlmacenarDatos<List<Inmueble>>( List<Inmueble> datos, string clave)



        Cliente        RecuperarCliente(string clave)
        string         RecuperarJWT(string clave)
        List<Inmueble> RecuperarInmuebles(string clave)
        ....
        ------------------------------------------------
        T RecuperaDatos(string clave)  <--------------- Cliente RecuperarDatos(string clave)
     */
    public interface IStorageService
    {
        //interface para servicios de almacemiento de info en navegador
        //existe paquete NuGet q te lo hace...https://github.com/Blazored/LocalStorage <--- "Blazored.LocalStorage, Blazored.SessionStorage,..."

        #region ...metodos asincronos....
        Task AlmacenarDatosAsync<T>(T datos, string clave);
        Task<T> RecuperarDatosAsync<T>(string clave);

        #endregion

        #region ...metodos sincronos....
        event EventHandler<Agente> EventAgente;
        void AlmacenarDatos<T>(T datos,string clave);
        T RecuperarDatos<T>(string clave);
        void BorrarDatos(string clave);

        #endregion


    }
}
