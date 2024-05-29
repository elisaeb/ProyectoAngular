using MongoDB.Bson;
using System.Net.Http.Json;
using System.Text.Json;
using Tecnocasa2024.Client.Models.Interfaces;
using Tecnocasa2024.Client.Pages.zonaCliente.PanelCliente;
using Tecnocasa2024.Shared;

namespace Tecnocasa2024.Client.Models
{
    public class AjaxService : IAjaxServices
    {
        #region ....propiedades clase servicio para hacer pet.ajax a servicio REST de asp.net core....
        private HttpClient _httpClient;
        #endregion

        public AjaxService(HttpClient httpClientDI)
        {
            this._httpClient = httpClientDI;
        }

        #region ....metodos clase servicio para hacer pet.ajax a servicio REST de asp.net core....
        public async Task<string> LoginGoogle()
        {
            return await this._httpClient.GetStringAsync("api/Cliente/LoginGoogle");
        }

        public async Task<RESTMessage> LoginRegistro(string email, string password)
        {
            //nos creamos diccionario de datos para mandar al servicio REST de asp.net core...con props: "email" y "password"
            //¿¿ seria posible mandarlo como variables sueltas sin diccionario como si de un formulario se tratase??
            Dictionary<string, string> _datos = new Dictionary<string, string> {
                {  "email", email },
                { "password", password }
            };
            HttpResponseMessage _resp = await this._httpClient.PostAsJsonAsync<Dictionary<string, string>>("/api/Cliente/Login", _datos);
            RESTMessage _restMessage = await _resp.Content.ReadFromJsonAsync<RESTMessage>();
            return _restMessage;
        }

        public async Task<ResultadoBusquedasGEOAPI> BuscarGeoApi(string texto)
        {
            return await this._httpClient
                             .GetFromJsonAsync<ResultadoBusquedasGEOAPI>($"/api/Portal/BuscarGeoApi?texto={texto}");
        }

        public async Task<RESTMessage> BuscarInmuebles(string cpro,
                                                        string nompro,
                                                        string cmum,
                                                        string nommun,
                                                        string cpos,
                                                        string nombar)
        {
            string _paramsUrl = $"cpro={cpro}&nompro={nompro}&cmum={cmum}&nommun={nommun}&cpos={cpos}&nombar={nombar}";

            return await this._httpClient
                 .GetFromJsonAsync<RESTMessage>($"/api/Portal/BuscarInmuebles?{_paramsUrl}");

        }

        public async Task<string> RecuperarProvincia(string cpro)
        {
            return await this._httpClient
                             .GetStringAsync($"/api/Portal/BuscarProvincia?cpro={cpro}");
        }

        public async Task<string> RecuperarMunicipio(string cpro, string cmum)
        {
            return await this._httpClient
                             .GetStringAsync($"/api/Portal/BuscarMunicipio?cpro={cpro}&cmum={cmum}");
        }
        public async Task<Inmueble> RecuperarInmueble(string idinmu)
        {
            return await this._httpClient.GetFromJsonAsync<Inmueble>($"/api/Portal/BuscarInmueble?idinmu={idinmu}");
        }

        public async Task<bool> Logout(List<string> lista, string idcliente)
        {
            try
            {
                Dictionary<string, string> _datos = new Dictionary<string, string> {
                    { "lista", JsonSerializer.Serialize(lista) },
                    { "idcliente", idcliente }
                };
                HttpResponseMessage respuesta = await this._httpClient.PostAsJsonAsync<Dictionary<string, string>>($"api/Cliente/ActualizarListaFavs", _datos);
                respuesta.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> EmailFormulario(string mensaje, string nombre, string apellido, string tel, string email)
        {
            try
            {
                Dictionary<string, string> _datos = new Dictionary<string, string> {
                    { "mensaje", mensaje},
                    { "nombre", nombre },
                    { "apellido", apellido },
                    { "tel", tel },
                    { "email", email }
                    //{ "emailAgente", emailAgente }  
                };
                HttpResponseMessage respuesta = await this._httpClient.PostAsJsonAsync<Dictionary<string, string>>($"api/Cliente/EmailFormulario", _datos);
                respuesta.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
  #endregion
}

