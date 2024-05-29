using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using Tecnocasa2024.Client.Models.Interfaces;
using Tecnocasa2024.Shared;
using Microsoft.JSInterop;

namespace Tecnocasa2024.Client.Models
{
    public class SubjectsStorage : IStorageService
    {
        

        #region ...propiedades clase subject-storage....
        private BehaviorSubject<Dictionary<string, string>> _storageSubject = new BehaviorSubject<Dictionary<string, string>>(new Dictionary<string, string>());
        private Dictionary<string, string> _storage = new Dictionary<string, string>(); //variable para almacenar diccionario q va por el observable del subject

        public event EventHandler<Agente> EventAgente;
        #endregion

        public SubjectsStorage()
        {
            this._storageSubject.Subscribe<Dictionary<string, string>>(
                    (Dictionary<string, string> datos) => this._storage = datos
                );
        }


        #region ...metodos de clase subject-storage....
        public void AlmacenarDatos<T>(T datos, string clave)
        {
            if (this._storage.ContainsKey(clave))
            {
                this._storage[clave] = JsonSerializer.Serialize<T>(datos);
            }
            else
                this._storage.Add(clave, JsonSerializer.Serialize<T>(datos));

            invocarAgente(clave);
            this._storageSubject.OnNext(this._storage);
        }

        public void invocarAgente(string clave)
        {
            if (clave == "agente")
            {
                Agente agente = JsonSerializer.Deserialize<Agente>(this._storage[clave]);
                EventAgente?.Invoke(this, agente);
            }
        }

        public T RecuperarDatos<T>(string clave)
        {
            if (this._storage.TryGetValue(clave, out string _valorSerializado))
            {
                return JsonSerializer.Deserialize<T>(_valorSerializado);
            }
            else
            {
                return default;
            }
        }

        public void BorrarDatos(string clave)
        {
            if (_storage.ContainsKey(clave))
            {
                _storage.Remove(clave);
            }
        }

        #region...metodos asincronos q no nos hacen falta para subjects...
        public Task AlmacenarDatosAsync<T>(T datos, string clave)
        {
            throw new NotImplementedException();
        }
        public Task<T> RecuperarDatosAsync<T>(string clave)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
