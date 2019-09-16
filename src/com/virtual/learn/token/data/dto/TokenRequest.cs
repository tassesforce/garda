using Newtonsoft.Json;

namespace garda.Controllers.Token
{
    public class TokenRequest
    {
        [JsonProperty("refresh_token")]
        /// <summary> refresh token qui va servir a generer un nouvel access token</summary> 
        public string RefreshToken {get;set;}
        
        [JsonProperty("access_token")]
        /// <summary> Ancien access_token qui va permettre de récupérer des informations à valider</summary> 
        public string AccessToken {get;set;}

        [JsonProperty("userId")]
        /// <summary> identifiant anonymise de l'utilisateur qui veut se connecter</summary> 
        public string UserId {get;set;}

        [JsonProperty("client_id")]
        /// <summary> Client_id de l'application utilisee pour se connecter</summary>
        public string ClientId {get;set;}
        
        [JsonProperty("client_secret")]
        /// <summary> Client_secret de l'application utilisee pour se connecter</summary>
        public string ClientSecret {get;set;}

        [JsonProperty("grant_type")]
        /// <summary> Grant_type demandé</summary>
        public string GrantType {get;set;}
    }
}