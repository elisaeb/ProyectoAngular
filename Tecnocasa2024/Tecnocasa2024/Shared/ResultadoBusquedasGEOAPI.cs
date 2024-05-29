using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnocasa2024.Shared
{
    public class ResultadoBusquedasGEOAPI
    {
        #region ...propiedades clase .....

        public List<Provincia> provincias { get; set; } = new List<Provincia>();
        public List<Municipio> ciudades { get; set; } = new List<Municipio>();
        public List<Barrio> barrios { get; set; } = new List<Barrio>();

        #endregion

        #region ...metodos clase...

        #endregion
    }
}
