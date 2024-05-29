namespace Tecnocasa2024.Server.Models
{
    public class GoogleCodeResponseJWT
    {
        public string access_token { get; set; } = "";
        public int expires_in { get; set; } = 0;
        public string id_token { get; set; } = "";
        public string scope { get; set; } = "";
        public string token_type { get; set; } = "";

    }
}
