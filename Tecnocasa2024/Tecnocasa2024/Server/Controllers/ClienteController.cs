using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Tecnocasa2024.Server.Models;
using Tecnocasa2024.Server.Models.Interfaces;
using Tecnocasa2024.Shared;
using MongoDB.Bson;
using System;
using System.Text;
using Tecnocasa2024.Server.Models.Servicios.Interfaces;


namespace Tecnocasa2024.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        #region ...... propiedades clase controlador rest-clienteController ...
        private IDBAccess _accesoDB;
        private IConfiguration _config;
        private ICorreo _correoService;
        #endregion

        public ClienteController(IConfiguration accesoAppSettingsDI, IDBAccess accesoDBDI, ICorreo correoServiceDI)
        {
                this._config = accesoAppSettingsDI;
                this._accesoDB = accesoDBDI;
                this._correoService = correoServiceDI;
        }


        #region ..... metodos clase controlador rest-clienteController....
        private string __GenerarJWT(Cliente datoscliente) {

            string _jwt = "";
            SecurityKey _claveFirmaJWT = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(this._config["JWT:clavefirma"]));

            //generamos token de sesion con claims personalizados del email...
            JwtSecurityToken _jwtObject = new JwtSecurityToken(
                    issuer: this._config["JWT:issuer"],
                    audience:null,
                    claims:new List<Claim> { new Claim("email", datoscliente.Email) },
                    notBefore:null,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: new SigningCredentials(_claveFirmaJWT,SecurityAlgorithms.HmacSha256)
                );

            //serializamos objeto para mandarlo en resp.al cliente blazor
            _jwt = new JwtSecurityTokenHandler().WriteToken(_jwtObject);            
            return _jwt;
        }

        [HttpPost]
        public async Task<bool> ActualizarListaFavs([FromBody] Dictionary<string, string> datos)
        {
            try
            {
                List<string> lista = JsonSerializer.Deserialize<List<string>>(datos["lista"]);
                bool resultado = await this._accesoDB.GuardarListaFavs(lista, datos["idcliente"]);

                // Si el resultado es verdadero, la actualización fue exitosa
                if (resultado)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier error y retornar un error HTTP 500
                return false;
            }
        } 


        [HttpPost]
        public async Task<RESTMessage> Login([FromBody] Dictionary<string,string> datos)
        {
            Cliente _cliente = await this._accesoDB.ComprobarCredenciales(datos["email"], datos["password"]);
            if(_cliente!=null)
            {
                //genero _jwt y devuelvo datos del cliente
                return new RESTMessage {
                    Codigo = 0,
                    Mensaje = "Login completado OK!!!!",
                    Error = "",
                    DatosCliente = _cliente,
                    TokenSesion = this.__GenerarJWT(_cliente),
                    OtrosDatos=null,
                };

            } else
            {
                //devolver respuesta con fallo
                return new RESTMessage
                {
                    Codigo = 1,
                    Mensaje = "Password incorrecta!!!!",
                    Error = "Password incorrecta, si el email q has introducido no existe se acaba de registrar",
                    DatosCliente = null,
                    TokenSesion = "",
                    OtrosDatos = null,
                };
            }
        }


        [HttpGet]
        public string LoginGoogle()
        {
            //codigo acceso mediante oauth2 a servicio externo autentificacion de google, para q nos genere url acceso y devolverla al cliente
            //tengo q generar peticion de esta forma:
            //https://accounts.google.com/o/oauth2/v2/auth ? 
            //      response_type = code &
            //      access_type = offline & 
            //      client_id = ..... & <----- extraido de la cuenta de servicio creada para el proyecto usando oauth en la consola de google-cloud
            //      redirect_uri = ...callback .... & <---- url de vuelta de google cuando usuario se autentifica bien en google y genera jwt-google
            //      scope =  valor1 valor2 valor3... & <---- de las APIS de google habilitadas para la cuenta del proyecto, en nuestro caso PEOPLE API
            //                                              q info quieres recuperar, ej:  email  o  email profile o  email profile addresses ....
            //      state = ...codigo random seguridad servicio aspnetcore ...  <--- codigo de seguridad que despues google metera en la respuesta
            //                                                                  y q usaremos en el callback para comprobar si es para mi servicio o no    
            string _clientId = this._config["Google:client_id"];
            string _redirectUri = Url.Action(nameof(LoginCallbackGoogle), "Cliente", null, Request.Scheme); //"https://localhost:7056/api/Cliente/LoginCallbackGoogle";
            string _scope = "profile email";
            string _state = "53583458034583485385035"; //<---no hacerlo asi!!!! sustituirlo por codigo hash generado por server...
            string _accestype = "offline";

            return $"https://accounts.google.com/o/oauth2/v2/auth?client_id={_clientId}&redirect_uri={_redirectUri}&response_type=code&scope={_scope}&access_type={_accestype}&state={_state}";


        }

        [HttpGet]
        public async Task<IActionResult> LoginCallbackGoogle([FromQuery]string code, [FromQuery]string state, [FromQuery]string scope)
             
        {
            //puedes recupoerar los parametros del querystring q manda google en la url una vez q el usuario se ha autentificado de dos formas:
            // - en parametros del metodo usando el decorador [FromQuery] tipo nombreParametro, ....
            // - del objeto Request propiedad .Query <--- diccionario clave-valor, donde aparacen parametros querystring
            // https://localhost:7056/api/Cliente/LoginCallbackGoogle ? state=53583458034583485385035 &
            //                                                          code=4%2F0AeaYSHDm5u2FQG0p5BNVzUY3SYPCCYrTERjWaYHvZmFKZQ--Wwbfvvg9hBLE7TD-M-8Y4A &
            //                                                          scope=email+profile+openid+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.profile+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.email &
            //                                                          authuser=0 &
            //                                                          prompt=consent


            //Podemos hacer la peticion con el CODIGO q nos devuelve GOOGLE para q nos genere JWT de sesion de dos formas:
            // - 1º de forma manual: generar peticion como se indica en oauth2: https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow/add-login-auth-code-flow#request-tokens
            //          peticion POST a url: https://www.googleapis.com/oauth2/v4/token
            //          pasando en el body no un json sino un diccionario clave-valor simulando un formulario, con estas variables:
            //          client_id, client_secret, code, redirect_uri, grant_type

            string _url = "https://www.googleapis.com/oauth2/v4/token";
            string _code = code; //this.Request.Query["code"].ToString();
            string _grantType = "authorization_code";
            string _clientId = this._config["Google:client_id"];
            string _clientSecret = this._config["Google:client_secret"];
            string _redirectUri = Url.Action(nameof(LoginCallbackGoogle), "Cliente", null, Request.Scheme); //"https://localhost:7056/api/Cliente/LoginCallbackGoogle";

            Dictionary<string, string> _content = new Dictionary<string, string> {
                { "grant_type", _grantType },
                {  "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "code", _code },
                { "redirect_uri", _redirectUri }
            };

            HttpClient _httpClient = new HttpClient();
            HttpResponseMessage _respuestaGoogle=await _httpClient.PostAsync(_url, new FormUrlEncodedContent(_content));
            string _contenidoRespuesta = await _respuestaGoogle.Content.ReadAsStringAsync();
            //en la respuesta debe estar serializado un json con este formato: { "access_token": "....", "refresh_token": "...", "id_token": "...", "token_type": "Bearer" }

            GoogleCodeResponseJWT _contenido = JsonSerializer.Deserialize<GoogleCodeResponseJWT>(_contenidoRespuesta);

            //tendria q comprobar q existe o no en la coleccion mongodb "clientes" un documento con el email de GMAIL con el que el
            //usuario se ha autentificado...si no existe dicho email, habria q crear perfil para ese usuario
            //despues almacenar tokens de sesion de google en coleccion: googleSessions <--- si ya existe algun token anterior de sesion
            //almacenado para ese usurio, borrarlo (deberia hacerse en el LOGOUT)

            string _urlPeopleApi = "https://people.googleapis.com/v1/people/me?personFields=emailAddresses%2Cphotos%2Cnames";
            string _accessTokenApi = _contenido.access_token; //<--- recuperado de la deserializacion de _contenidoRespuesta
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessTokenApi);
            
            HttpResponseMessage _respuestaPeopleAPIGoogle = await _httpClient.GetAsync(_urlPeopleApi);
            string _infoClienteGmail=await _respuestaPeopleAPIGoogle.Content.ReadAsStringAsync();

            JsonNode _infoJSONClienteGmail=JsonNode.Parse(_infoClienteGmail);
            string _nombreCliente = _infoJSONClienteGmail["names"][0]!["givenName"]!.GetValue<string>();
            string _apellidosCliente= _infoJSONClienteGmail["names"][0]!["familyName"]!.GetValue<string>();
            string _emailCliente= _infoJSONClienteGmail["emailAddresses"][0]!["value"]!.GetValue<string>();

            //para obtener el email del cliente logueado y info de su perfil necesito acceder a la api de google: PEOPLE-API
            //https://developers.google.com/people/api/rest/v1/people/get?hl=es-419
            /*
                peticion por GET: parametros a añadir en url: personFields, key ... en cabecera meter access_token recuperado de pet.anterior
                    curl \
                      'https://people.googleapis.com/v1/people/me?personFields=emailAddresses%2Cphotos%2Cnames&key=[YOUR_API_KEY]' \
                      --header 'Authorization: Bearer [YOUR_ACCESS_TOKEN]' \
                      --header 'Accept: application/json' \
                      --compressed

                respuesta:
                        {
                          "resourceName": "people/109398914241914367428",
                          "etag": "%EggBAgMJLjc9PhoEAQIFByIMb3I2TlJjbDhBT3c9",
                          "names": [
                            {
                              "metadata": {
                                "primary": true,
                                "source": {
                                  "type": "PROFILE",
                                  "id": "109398914241914367428"
                                },
                                "sourcePrimary": true
                              },
                              "displayName": "pmr aiki", 
                              "familyName": "aiki", <--------------------  //a recuperar, apellidos del cliente
                              "givenName": "pmr",  <--------------------  //a recuperar, nombre del cliente
                              "displayNameLastFirst": "aiki, pmr",
                              "unstructuredName": "pmr aiki"
                            }
                          ],
                          "photos": [
                            {
                              "metadata": {
                                "primary": true,
                                "source": {
                                  "type": "PROFILE",
                                  "id": "109398914241914367428"
                                }
                              },
                              "url": "https://lh3.googleusercontent.com/a/ACg8ocI7ibXwSedcXJys8QJQp7Rdn3xqi18xIcx51Qa_4msYqc--_W8i=s100"
                            }
                          ],
                          "emailAddresses": [
                            {
                              "metadata": {
                                "primary": true,
                                "verified": true,
                                "source": {
                                  "type": "ACCOUNT",
                                  "id": "109398914241914367428"
                                },
                                "sourcePrimary": true
                              },
                              "value": "pmr.aiki@gmail.com" <--------------------  //a recuperar, email del cliente
                            }
                          ]
                        }
             */

            //
            // - 2º instalando y usando paquete Google.Apis: clases a usar...GoogleAuthorizationCodeFlow y metodo: ExchangeCodeForTokenAsync
            //     crear credenciales para hacer uso de la api con tokens recup: UserCredential  
            //    para obtener info de google-apis con esas credenciales:  OAuth2Service 
            return Redirect($"https://localhost:7056/Cliente/Panel/InicioPanel?email={_emailCliente}");


        }

        public bool EmailFormulario([FromBody] Dictionary<string, string> datos)
        {
            try
            {
                string mensajeEmailStr = $"Nombre: {datos["nombre"]} \n Email: {datos["email"]} \n Telefono: {datos["telefono"]} \n Mensaje: {datos["mensaje"]}";

                this._correoService.EnviarEmail("elisaburiestu@gmail.com","Envio de email desde fromcontacto",  mensajeEmailStr, "");
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        #endregion



    }
}
