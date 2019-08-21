using Newtonsoft.Json;

namespace garda.Controllers.User
{
    public class DeleteUserAuthRequest
    {
        [JsonProperty("client_id")]
        [JsonRequired]
        // Client_id de l'application utilisee pour se connecter
        public string ClientId {get;set;}
        
        [JsonProperty("client_secret")]
        [JsonRequired]
        // Client_secret de l'application utilisee pour se connecter
        public string ClientSecret {get;set;}

    }
}