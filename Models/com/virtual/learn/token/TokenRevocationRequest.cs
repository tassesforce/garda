using Newtonsoft.Json;

namespace garda.Controllers.Token
{
    public class TokenRevocationRequest
    {
        [JsonProperty("refresh_token")]
        [JsonRequired]
        /// <summary> refresh token qui va servir a generer un nouvel access token</summary> 
        public string RefreshToken {get;set;}
        [JsonProperty("client_id")]
        [JsonRequired]
        /// <summary> Identifiant de l'application consommatrice</summary> 
        public string ClientId {get;set;}
        [JsonProperty("client_secret")]
        [JsonRequired]
        /// <summary> Secret de l'application consommatrice</summary> 
        public string ClientSecret {get;set;}

    }
}