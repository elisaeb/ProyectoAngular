using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tecnocasa2024.Shared
{
    public class Barrio
    {
        #region ....propiedades clase Barrio....

        [BsonElement("CCOM")]
        public string CCOM { get; set; } = "";
        
        [BsonElement("NENTSI50")]
        public string NENTSI50 { get; set; } = "";

        [BsonElement("NNUCLE50")]
        public string NNUCLE50 { get; set; } = "";

        [BsonElement("CPRO")]
        public string CPRO { get; set; } = "";

        [BsonElement("CMUM")]
        public string CMUM { get; set; } = "";

        [BsonElement("CUN")]
        public string CUN { get; set; } = "";

        [BsonElement("CPOS")]
        public string CPOS { get; set; } = ""; //<--- codigo postal al q pertenece el barrio

        [BsonElement("CVIA")]
        public string CVIA { get; set; } = ""; //<--- codigo id del barrio

        [BsonElement("NVIAC")]
        public string NVIAC { get; set; } = ""; //<-- nombre del barrio

        [BsonElement("TVIA")]
        public string TVIA { get; set; } = ""; //<---- tipo de barrio (calle, paseo, avenida, plaza,...)


        #endregion

        #region ...metodos clase Barrio....

        #endregion
    }
}
