using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Tecnocasa2024.Server.Models.Interfaces;
using Tecnocasa2024.Shared;

namespace Tecnocasa2024.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PortalController : ControllerBase
    {
        #region ...propiedades de la clase PortalController controlador  servicio REST ....
        private IConfiguration _configuration;
        private HttpClient _http = new HttpClient();
        private string _keyGeoApi;
        private IDBAccess _accesoDB;
        #endregion

        public PortalController(IConfiguration accesoAppSettingsDI, IDBAccess accesoDB)
        {
                this._accesoDB = accesoDB;
                this._configuration = accesoAppSettingsDI;
                this._keyGeoApi = this._configuration["GeoApi:key"];
        }

        #region ...metodos de la clase PortalController controlador  servicio REST ....
        [HttpGet]
        public async Task<ResultadoBusquedasGEOAPI> BuscarGeoApi([FromQuery] string texto) {
            try
            {
                //string _keyGeoApi = this._configuration["GeoApi:key"];

                //consultar a geoapi:

                #region .... - consulta provincias que contengan ese texto....
                // - provincias q contengan ese texto
                // https://apiv1.geoapi.es/provincias ? CCOM=... & type=JSON & key= & sandbox=1 <=== si pasas clave de geoapi, poner sandbox=0
                // seria recuperar todas las provs, y una recup.todas buscar en su nombre PRO si empieza con texto
                HttpRequestMessage _requestProvs = new HttpRequestMessage(HttpMethod.Get, $"https://apiv1.geoapi.es/provincias?&type=JSON&key={_keyGeoApi}&sandbox=0");
                HttpResponseMessage _respProvs = await this._http.SendAsync(_requestProvs);

                if (!_respProvs.IsSuccessStatusCode) { throw new Exception($"fallo al intentar recuperar provincias q contienen el texto...{texto}"); }

                string _datosProvsSERIALIZADOS = await _respProvs.Content.ReadAsStringAsync();

                #region...leyendo respuesta del servicio geoapi sin clase especifica de por medio q mapee la respuesta: RespuestaGEOAPI.cs ....

                //JsonNode _datosProvsJSON=JsonNode.Parse(_datosProvsSERIALIZADOS);
                //List<Provincia> _listaProvs = _datosProvsJSON["data"].GetValue<List<Provincia>>();

                //List<Provincia> _provs = _listaProvs.Where((Provincia prov) => new Regex(texto, RegexOptions.IgnoreCase).IsMatch(prov.PRO))
                //                                    .ToList<Provincia>();


                #endregion

                RespuestasGEOAPI<Provincia> _datosProvs = JsonSerializer.Deserialize<RespuestasGEOAPI<Provincia>>(_datosProvsSERIALIZADOS);

                List<Provincia> _provs = _datosProvs.data
                                                  .Where((Provincia prov) => new Regex(texto, RegexOptions.IgnoreCase).IsMatch(prov.PRO))
                                                  .ToList<Provincia>();

                #endregion


                #region .... - consulta ciudades que contengan ese texto ....
                // - ciudades q contengan ese texto
                // https://apiv1.geoapi.es/municipios ? CPRO=... & type=JSON & key= & sandbox=1 <=== si pasas clave de geoapi, poner sandbox=0
                // ¿¿¿ q CPRO meto?? tengo texto busqueda...no CPRO tengo q lanzar 52 pet. por cada prov. para recup. sus municipios y una vez
                //obtenidas buscar el texto1..
                List<Task<HttpResponseMessage>> _tareasMunis = new List<Task<HttpResponseMessage>>();
                for (int i = 1; i < 53; i++)
                {
                    string _cpro=i < 10 ? "0" + i.ToString() : i.ToString();
                    string _url = $"https://apiv1.geoapi.es/municipios?CPRO={_cpro}&type=JSON&key={_keyGeoApi}&sandbox=0";

                    HttpRequestMessage _requestMuni = new HttpRequestMessage(HttpMethod.Get, _url);
                    _tareasMunis.Add(_http.SendAsync(_requestMuni));
                    Task.Delay(1000);
                }

                HttpResponseMessage[] _resultados = await Task.WhenAll(_tareasMunis);
                
                List<Municipio> _municipios=new List<Municipio>();
                foreach (HttpResponseMessage resp in _resultados)
                {
                    string _bodyMuni = await resp.Content.ReadAsStringAsync();
                    List<Municipio> _datosMuni = (JsonSerializer.Deserialize<RespuestasGEOAPI<Municipio>>(_bodyMuni))
                                                    .data
                                                    .Where((Municipio muni)=> new Regex(texto, RegexOptions.IgnoreCase).IsMatch(muni.DMUN50))
                                                    .ToList<Municipio>();
                     
                    _datosMuni.ForEach((Municipio muni)=> _municipios.Add(muni));   
                }

                //List<Municipio> _munis= _resultados.ToList<HttpResponseMessage>()
                //                            .Select( async (HttpResponseMessage resp)=> await resp.Content.ReadAsStringAsync() )
                //                            .Select( (string body) => (JsonSerializer.Deserialize<RespuestasGEOAPI<Municipio>>(body)).data )
                //                            .ToList<Municipio>();


                #endregion


                #region .... - calles/barrios que contengan ese texto ....
                // - calles/barrios q contengan ese texto...
                // https://apiv1.geoapi.es/qcalles ? QUERY=..texto... & type=JSON & key= & sandbox=1 <=== si pasas clave de geoapi, poner sandbox=0
                HttpRequestMessage _requestCalles = new HttpRequestMessage(HttpMethod.Get, $"https://apiv1.geoapi.es/qcalles?QUERY={texto}&type=JSON&key={_keyGeoApi}&sandbox=0");
                HttpResponseMessage _respCalles=await this._http.SendAsync(_requestCalles);

                if (!_respCalles.IsSuccessStatusCode) { throw new Exception($"fallo al intentar recuperar calles q contienen el texto...{texto}"); }

                string _datosCallesSERIALIZADOS=await _respCalles.Content.ReadAsStringAsync();

                RespuestasGEOAPI<Barrio> _datosCalles = JsonSerializer.Deserialize<RespuestasGEOAPI<Barrio>>(_datosCallesSERIALIZADOS);
                List<Barrio> _barrios = _datosCalles.data;

                #endregion


                return new ResultadoBusquedasGEOAPI { provincias = _provs, ciudades = _municipios, barrios = _barrios };
            }
            catch (Exception ex)
            {

                return new ResultadoBusquedasGEOAPI { provincias = null, ciudades = null, barrios = null }; ;
            }

        }

        [HttpGet]
        public async Task<RESTMessage> BuscarInmuebles( [FromQuery] string cpro,
                                                        [FromQuery] string? nompro,      
                                                        [FromQuery] string? cmum,
                                                        [FromQuery] string? nommun,
                                                        [FromQuery] string? cpos,
                                                        [FromQuery] string? nombar
                                                        )
        {
            try
            {
                //buscar en mongodb inmuebles con esos criterios de cpro,...
                List<Inmueble> _listaADevolver = await this._accesoDB.RecuperarInmuebles(cpro,nompro,cmum,nommun,cpos,nombar);
                if (_listaADevolver == null) throw new Exception("error en BD de mongo a la hora de recuperar inmuebles");

                //anulamos conversion de nombres de props. cuando serialzamos los datos de la direccion de cada inmueble, pq provincia,ciudad y barrio tienen todas MAYS...
                JsonSerializerOptions options = new JsonSerializerOptions { PropertyNamingPolicy = null };
                string _listaSerializada = JsonSerializer.Serialize<List<Inmueble>>(_listaADevolver, options);

                return new RESTMessage
                {
                    Codigo = 0,
                    Mensaje = $"Inmuebles recuperados OK para ...cod.provincia: {cpro}, cod.municipio: {cmum}, cod.postal: {cpos}",
                    Error = "",
                    TokenSesion = "",
                    DatosCliente = null,
                    OtrosDatos = _listaSerializada
                };

            }
            catch (Exception ex)
            {

                return new RESTMessage
                {
                    Codigo=1, 
                    Mensaje="Error al intentar recuperar inmuebles", 
                    Error=ex.Message,
                    TokenSesion="",
                    DatosCliente=null,
                    OtrosDatos=null
                };
            }
        }
        [HttpGet]
        public async Task<Inmueble> BuscarInmueble([FromQuery] string idinmu)
        {
            try
            {
                Inmueble _inmueble = await this._accesoDB.RecuperarInmueble(idinmu);
                return _inmueble;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public async Task<string> BuscarProvincia([FromQuery] string cpro) {
            //string _keyGeoApi = this._configuration["GeoApi:key"];
            try
            {
                HttpRequestMessage _requestProvs = new HttpRequestMessage(HttpMethod.Get, $"https://apiv1.geoapi.es/provincias?&type=JSON&key={this._keyGeoApi}&sandbox=0");
                HttpResponseMessage _respProvs = await this._http.SendAsync(_requestProvs);

                if (!_respProvs.IsSuccessStatusCode) { throw new Exception($"fallo al intentar recuperar provincias con CPRO...{cpro}"); }

                string _datosProvsSERIALIZADOS = await _respProvs.Content.ReadAsStringAsync();
                RespuestasGEOAPI<Provincia> _datosProvs = JsonSerializer.Deserialize<RespuestasGEOAPI<Provincia>>(_datosProvsSERIALIZADOS);

                string _nombreProv = _datosProvs.data
                                                .Where((Provincia prov) => prov.CPRO==cpro)
                                                .Select( (Provincia prov)=> prov.PRO)
                                                .Single<String>();

                return _nombreProv;

            }
            catch (Exception ex)
            {

                return "";
            }

        }

        [HttpGet]
        public async Task<string> BuscarMunicipio([FromQuery] string cpro, [FromQuery] string cmum)
        {
            try
            {
                HttpRequestMessage _requestMunis = new HttpRequestMessage(HttpMethod.Get, $"https://apiv1.geoapi.es/municipios?CPRO={cpro}&type=JSON&key={this._keyGeoApi}&sandbox=0");
                HttpResponseMessage _respMunis = await this._http.SendAsync(_requestMunis);

                if (!_respMunis.IsSuccessStatusCode) { throw new Exception($"fallo al intentar recuperar municipios con CPRO y CMUM ...{cpro}, {cmum}"); }

                string _datosMunisSERIALIZADOS = await _respMunis.Content.ReadAsStringAsync();
                RespuestasGEOAPI<Municipio> _datosMunis = JsonSerializer.Deserialize<RespuestasGEOAPI<Municipio>>(_datosMunisSERIALIZADOS);

                string _nombreMuni = _datosMunis.data
                                                .Where((Municipio muni) => muni.CPRO == cpro && muni.CMUM == cmum)
                                                .Select((Municipio muni) => muni.DMUN50)
                                                .Single<String>();

                return _nombreMuni;

            }
            catch (Exception ex)
            {

                return "";
            }
        }


        #endregion


    }
}
