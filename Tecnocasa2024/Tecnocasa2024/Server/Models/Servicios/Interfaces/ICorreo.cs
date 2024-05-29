namespace Tecnocasa2024.Server.Models.Servicios.Interfaces
{
    public interface ICorreo
    {
      
        public String UserId { get; set; }
        public String Key { get; set; }
        public bool EnviarEmail(String emailcliente, String subject, String cuerpoMensaje, String? ficheroAdjunto);
    }
}
