using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnocasa2024.Shared
{
    public class RespuestasGEOAPI<T>
    {
        #region ....propiedades de clase q encapusal respuesta servicio REST-GEOAPI ...
        public string update_date { get; set; } = "";
        public int size { get; set; } = 0;
        public List<T> data { get; set; }=new List<T>();
        public string warning { get; set; } = "";
        #endregion
    }
}
