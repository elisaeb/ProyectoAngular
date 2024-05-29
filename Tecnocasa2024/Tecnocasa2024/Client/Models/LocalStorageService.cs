using Microsoft.JSInterop;
using System.Text.Json;
using Tecnocasa2024.Client.Models.Interfaces;
using Tecnocasa2024.Shared;


namespace Tecnocasa2024.Client.Models
{
    public class LocalStorageService: IStorageService
    {
        #region ...props.clase servicio acceso al LOCALSTORAGE....
        private IJSRuntime _jsSvc;

        public event EventHandler<Agente> EventAgente;
        #endregion

        public LocalStorageService(IJSRuntime jSRuntime)
        {
                _jsSvc = jSRuntime;
        }




        #region ....metodos de clase servicio acceso al LOCALSTORAGE...


        public T RecuperarDatos<T>(string clave)
        {
            throw new NotImplementedException();
        }

        public async  Task<T> RecuperarDatosAsync<T>(string clave)
        {
            string _datos = await this._jsSvc.InvokeAsync<string>("localStorage.getItem", clave);
            return JsonSerializer.Deserialize<T>(_datos);
        }

        public void AlmacenarDatos<T>(T datos, string clave)
        {
            throw new NotImplementedException();
        }

        public async Task AlmacenarDatosAsync<T>(T datos, string clave)
        {
            string _datos = JsonSerializer.Serialize(datos);
            await this._jsSvc.InvokeVoidAsync("console.log","datos a alamcenar en localstorage...", _datos, clave);

            await this._jsSvc.InvokeVoidAsync("localStorage.setItem", clave, _datos );
    }

        public void BorrarDatos(string clave)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
