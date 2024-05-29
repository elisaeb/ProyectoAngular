using MongoDB.Driver;
using Tecnocasa2024.Server.Models.Interfaces;
using Tecnocasa2024.Shared;
using BCrypt.Net;
using MongoDB.Bson;
using System.Linq;

namespace Tecnocasa2024.Server.Models
{
    public class MongoDBAccess : IDBAccess
    {
        #region ...propiedades de clase servicio acceso a MongoDB ....
        private IConfiguration _configuration;
        private MongoClient _mongoClient; //<--- objeto para conectarnos al servidor mongodb 
        private IMongoDatabase _mongoDatabase; //<----objeto para encapsular acceso a bd "Tecnocasa2024" de mongodb
        #endregion

        public MongoDBAccess(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._mongoClient = new MongoClient(this._configuration["MongoDB:ConnectionString"]);
            this._mongoDatabase = this._mongoClient.GetDatabase(this._configuration["MongoDB:DataBase"]);
        }

        #region ....metodos de clase servicio de acceso a MongoDB...
        public async Task<Cliente> ComprobarCredenciales(string email, string password)
        {
            try
            {
                //1º paso: intentar recuperar de la coleccion "clientes" el documento asociado a ese email, sino existe...al registro...
                // usando linq (intentarlo hacer con metodo .FindAsync()
                Cliente _clienteLogged = this._mongoDatabase.GetCollection<Cliente>("clientes")
                                            .AsQueryable<Cliente>() //<---- metodo para hacer q cursor recuperado por metodo .GetCollection() sea recorrido por LINQ
                                            .Where((Cliente c) => c.Email == email)
                                            .Single<Cliente>();

                //2º paso: si existe, comprobar el hash de la password almacenada en mongo, con el hash calculado para la password pasado en 
                //el parametro, si no coincide...devolvemos fallo en login, si coincide:
                if (BCrypt.Net.BCrypt.Verify(password,_clienteLogged.Password))
                {
                    //3º paso: recuperamos objeto Cliente expandido (misInmuebles, listaFavoritos) y se lo devolvemos <== OJO!!! PENDIENTE!!!
                    //al controlador para q se lo devuelva al cliente blazor 

                    var inmuebleCollection = this._mongoDatabase.GetCollection<Inmueble>("inmuebles");

                    // Convertir los ObjectId a strings
                    var favoritosIdsString = _clienteLogged.ListaFavoritos.Select(id => id.ToString()).ToList();

                    // Filtrar los inmuebles favoritos por los IDs
                    var filter = Builders<Inmueble>.Filter.In(inmueble => inmueble.IdInmueble, favoritosIdsString);
                    var favoritosInmuebles = await inmuebleCollection.Find(filter).ToListAsync();

                    // Asignar los inmuebles favoritos expandidos al cliente
                    _clienteLogged.FavoritosExpandidos = favoritosInmuebles;

                    var misInmueblesIdsString = _clienteLogged.MisInmuebles.Select(id => id.ToString()).ToList();

                    // Filtrar los inmuebles por los IDs de "misInmuebles"
                    var misInmueblesFilter = Builders<Inmueble>.Filter.In(inmueble => inmueble.IdInmueble, misInmueblesIdsString);
                    var misInmueblesInmuebles = await inmuebleCollection.Find(misInmueblesFilter).ToListAsync();

                    // Asignar los inmuebles expandidos al cliente
                    _clienteLogged.InmueblesExpandidos = misInmueblesInmuebles;

                    return _clienteLogged;

                    //var inmuebleCollection = this._mongoDatabase.GetCollection<Inmueble>("inmuebles");
                    //var favoritosIds = _clienteLogged.ListaFavoritos;
                    //var favoritosIdsString = favoritosIds.Select(id => id.ToString()).ToList();
                    //var filter = Builders<Inmueble>.Filter.In(inmueble => inmueble.IdInmueble, favoritosIdsString);
                    //var favoritosInmuebles = await inmuebleCollection.Find(filter).ToListAsync();
                    //_clienteLogged.FavoritosExpandidos = favoritosInmuebles;


                    //var misInmueblesIds = _clienteLogged.MisInmuebles;
                    //var misInmueblesIdsString = misInmueblesIds.Select(id => id.ToString()).ToList();
                    //var misInmueblesFilter = Builders<Inmueble>.Filter.In(inmueble => inmueble.IdInmueble, misInmueblesIdsString);
                    //var misInmueblesInmuebles = await inmuebleCollection.Find(misInmueblesFilter).ToListAsync();
                    //_clienteLogged.InmueblesExpandidos = misInmueblesInmuebles;

                    //return _clienteLogged;

                    //var inmuebleCollection = this._mongoDatabase.GetCollection<Inmueble>("inmuebles");
                    //var agenteCollection = this._mongoDatabase.GetCollection<Agente>("agentes");
                    //var oficinaCollection = this._mongoDatabase.GetCollection<Oficina>("oficinas");

                    //var favoritosIds = _clienteLogged.ListaFavoritos;
                    //var favoritosIdsString = favoritosIds.Select(id => id.ToString()).ToList();
                    //var favoritosFilter = Builders<Inmueble>.Filter.In(inmueble => inmueble.IdInmueble, favoritosIdsString);
                    //var favoritosInmuebles = await inmuebleCollection.Find(favoritosFilter).ToListAsync();

                    //var misInmueblesIds = _clienteLogged.MisInmuebles;
                    //var misInmueblesIdsString = misInmueblesIds.Select(id => id.ToString()).ToList();
                    //var misInmueblesFilter = Builders<Inmueble>.Filter.In(inmueble => inmueble.IdInmueble, misInmueblesIdsString);
                    //var misInmueblesInmuebles = await inmuebleCollection.Find(misInmueblesFilter).ToListAsync();

                    //foreach (var inmueble in misInmueblesInmuebles)
                    //{
                    //    if (!string.IsNullOrEmpty(inmueble.IdAgente))
                    //    {
                    //        inmueble.Agente = await agenteCollection
                    //                             .Find(Builders<Agente>.Filter.Eq(agente => agente.IdAgente, inmueble.IdAgente))
                    //                             .FirstOrDefaultAsync();
                    //    }

                    //    if (!string.IsNullOrEmpty(inmueble.IdOficina))
                    //    {
                    //        inmueble.Oficina = await oficinaCollection
                    //                              .Find(Builders<Oficina>.Filter.Eq(oficina => oficina.IdOficina, inmueble.IdOficina))
                    //                              .FirstOrDefaultAsync();
                    //    }
                    //}

                    //_clienteLogged.FavoritosExpandidos = favoritosInmuebles;
                    //_clienteLogged.InmueblesExpandidos = misInmueblesInmuebles;

                    //return _clienteLogged;


                }
                else
                {
                    throw new Exception("Password invalida para ese email...");
                }


            }
            catch (InvalidOperationException ex1) {
                //fallo en query linq, no existe el email...lo registramos el cliente
                if (ex1.Message.Contains("Sequence contains no elements"))
                {
                    String _hash=BCrypt.Net.BCrypt.HashPassword(password);
                    Cliente _newCliente = new Cliente {
                        Email= email,
                        Password=_hash
                    };
                    await this._mongoDatabase.GetCollection<Cliente>("clientes").InsertOneAsync(_newCliente);
                    //mandar email activacion...lo ideal en controlador, pero en controlador no sabemos si estamos en login o en un registro...


                    return _newCliente;
                } else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                //fallo en login...
                return null;
            }
        }

        public async Task<bool> GuardarListaFavs(List<string> lista, string idcliente)
        {
            try
            {
                var clienteCollection = this._mongoDatabase.GetCollection<Cliente>("clientes");

                // Convertir las strings a ObjectIds
                var listaObjectId = lista.Select(ObjectId.Parse).ToList();

                var filtro = Builders<Cliente>.Filter.Eq(c => c.IdCliente, idcliente);

                // Actualizar la lista de favoritos con los ObjectIds
                var actualizacion = Builders<Cliente>.Update.Set(c => c.ListaFavoritos, listaObjectId);

                var resultado = await clienteCollection.UpdateOneAsync(filtro, actualizacion);

                return resultado.ModifiedCount > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Inmueble>RecuperarInmueble(string idinmu)
        {
            try
            {
                return this._mongoDatabase.GetCollection<Inmueble>("inmuebles")
                                                .AsQueryable<Inmueble>()
                                                .Where((Inmueble inmu)=> inmu.IdInmueble==idinmu)
                                                .Single<Inmueble>();
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async  Task<List<Inmueble>> RecuperarInmuebles(string cpro, string nompro, string cmum, string nommun, string cpos, string nombar)
        {
            try
            {
                Func<Inmueble, Boolean> _filtroFIND = (Inmueble inmu) => { return true;  };
                
                if (!String.IsNullOrEmpty(cpro) && String.IsNullOrEmpty(cmum) && String.IsNullOrEmpty(cpos)) {
                    _filtroFIND = (Inmueble inmu) => { return inmu.DirecInmueble.ProvDirec.CPRO == cpro; };                 
                }

                if (!String.IsNullOrEmpty(cpro) && !String.IsNullOrEmpty(cmum) && String.IsNullOrEmpty(cpos))
                {
                    _filtroFIND = (Inmueble inmu) => { 
                        return inmu.DirecInmueble.ProvDirec.CPRO == cpro && 
                               inmu.DirecInmueble.MunDirec.CMUM == cmum;
                    };
                }

                if (!String.IsNullOrEmpty(cpro) && !String.IsNullOrEmpty(cmum) && !String.IsNullOrEmpty(cpos))
                {
                    _filtroFIND = (Inmueble inmu) => {
                        return inmu.DirecInmueble.ProvDirec.CPRO == cpro &&
                               inmu.DirecInmueble.MunDirec.CMUM == cmum &&
                               inmu.DirecInmueble.BarrioDirec.CPRO == cpos;
                    };
                }


                //List<Inmueble> _inmuebles=  this._mongoDatabase.GetCollection<Inmueble>("inmuebles")
                //                                .AsQueryable<Inmueble>()
                //                                .Where(_filtroFIND)
                //                                .ToList<Inmueble>();

                //He comentado lo de arriba porque no me encontraba nada, asi cojo todos los inmuebles

                var inmueblesCollection = this._mongoDatabase.GetCollection<Inmueble>("inmuebles");
                var agentesCollection = this._mongoDatabase.GetCollection<Agente>("agentes");
                var oficinasCollection = this._mongoDatabase.GetCollection<Oficina>("oficinas");

                // Realiza una búsqueda de todos los inmuebles
                //var inmuebles = await inmueblesCollection.Find(_ => true).ToListAsync();
                List<Inmueble> inmuebles = this._mongoDatabase.GetCollection<Inmueble>("inmuebles")
                                                .AsQueryable<Inmueble>()
                                                .ToList<Inmueble>();

                var idsAgentes = inmuebles.Select(i => i.IdAgente).Distinct().ToList();
                var idsOficinas = inmuebles.Select(i => i.IdOficina).Distinct().ToList();

                var oficinas = await oficinasCollection.Find(o => idsOficinas.Contains(o.IdOficina)).ToListAsync();


                var idsAgentesEnOficinas = oficinas.SelectMany(o => o.ListaAgentes).Distinct().ToList();
                idsAgentes.AddRange(idsAgentesEnOficinas.Select(id => id.ToString()));
                idsAgentes = idsAgentes.Distinct().ToList();

                var agentes = await agentesCollection.Find(a => idsAgentes.Contains(a.IdAgente)).ToListAsync();

                foreach (var inmueble in inmuebles)
                {
                    inmueble.Agente = agentes.FirstOrDefault(a => a.IdAgente == inmueble.IdAgente);
                    var oficina = oficinas.FirstOrDefault(o => o.IdOficina == inmueble.IdOficina);
                    if (oficina != null)
                    {
                        oficina.AgenteEncargado = agentesCollection.Find(a => a.IdAgente == oficina.IdAgenteEncargado).FirstOrDefault();
                        oficina.AgentesExpandidos = agentes.Where(a => oficina.ListaAgentes.Select(id => id.ToString()).Contains(a.IdAgente)).ToList();
                    }
                    inmueble.Oficina = oficina;
                }

                return inmuebles;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public Task<bool> RegistrarCliente(string email, string password)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
