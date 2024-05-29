using System.Net.Mail;
using System.Net;
using Tecnocasa2024.Server.Models.Servicios.Interfaces;

namespace Tecnocasa2024.Server.Models.Servicios
{

    public class MailJetService : ICorreo
    {
        #region ....propiedades de la clase servicio MailJetService....
        private IConfiguration __accesoAppSettings;
        


        public string UserId { get; set; } 
        public string Key { get; set; } 

        #endregion


        public MailJetService(IConfiguration servicioAccesoAppSettingsDI)
        {
            this.__accesoAppSettings = servicioAccesoAppSettingsDI;

            this.UserId = this.__accesoAppSettings.GetSection("MailJetCredentials:ClaveAPI").Value;
            this.Key = this.__accesoAppSettings.GetSection("MailJetCredentials:ClaveSecreta").Value;


        }




        #region ....metodos de la clase servicio MailJetService....
        public bool EnviarEmail(string emailcliente, string subject, string cuerpoMensaje, string? ficheroAdjunto)
        {
            try
            {
                //1º: abrir socket (conexion) al servidor SMTP de mailjet con las credenciales de la api q te dan 
                //cuando te registras <=== usando la clase SmtpClient
                SmtpClient _clienteSMTP = new SmtpClient("in-v3.mailjet.com");
                _clienteSMTP.Credentials = new NetworkCredential(this.UserId, this.Key);

                //2º: crear el cuerpo del mensaje de correo a mandar al cliente.. <=== clase MailMessage
                MailMessage _mensajeAEnviar = new MailMessage("elisaburiestu@gmail.com", emailcliente);
                _mensajeAEnviar.Subject = subject;
                _mensajeAEnviar.IsBodyHtml = true; //si quieres incrustar en el body del tags html como links, imagenes,....
                _mensajeAEnviar.Body = cuerpoMensaje;

                if (!String.IsNullOrEmpty(ficheroAdjunto))
                {
                    //en el parametro fichero adjunto va la ruta y el nombre de la factura generada en el server...
                    MemoryStream ms = new MemoryStream();
                    using (FileStream fs = new FileStream(ficheroAdjunto, FileMode.Open, FileAccess.Read)) fs.CopyTo(ms);
                    _mensajeAEnviar.Attachments.Add(new Attachment(ms, "application/pdf"));
                }

                //3º: mandar email usando el socket abierto con cliente smtp creado <=== metodo .send de la clase
                //                                                                      SmtpClient
                _clienteSMTP.Send(_mensajeAEnviar);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }



        #endregion
    }

}
