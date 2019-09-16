using Newtonsoft.Json;

namespace garda.Controllers.User
{
    public class UserAuthRequest
    {
        [JsonProperty("password")]
        // mot de passe de l'utilisateur qui veut se connecter
        public string Password {get;set;}
        
        [JsonProperty("client_id")]
        [JsonRequired]
        // Client_id de l'application utilisee pour se connecter
        public string ClientId {get;set;}
        
        [JsonProperty("client_secret")]
        [JsonRequired]
        // Client_secret de l'application utilisee pour se connecter
        public string ClientSecret {get;set;}

        [JsonProperty("grant_type")]
        [JsonRequired]
        // Grant_type demandé
        public string GrantType {get;set;}
    }
}