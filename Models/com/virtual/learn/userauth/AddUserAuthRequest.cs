using Newtonsoft.Json;

namespace garda.Controllers.User
{
    public class AddUserAuthRequest
    {
        [JsonProperty("login")]
        /// <summary> Propriete identifiant de l'utilisateur qui souhaite se connecter</summary>
        public string Login {get; set;}

        [JsonProperty("account_type")]
        /// <summary> Propriete type de compte (agence, entreprise, candidat)</summary>
        public string AccountType {get; set;}
        
        [JsonProperty("password")]
        /// <summary> mot de passe de l'utilisateur qui veut se connecter</summary>
        public string Password {get;set;}
        
        [JsonProperty("client_id")]
        /// <summary> Client_id de l'application utilisee pour se connecter</summary>
        public string ClientId {get;set;}
        
        [JsonProperty("client_secret")]
        /// <summary> Client_secret de l'application utilisee pour se connecter</summary>
        public string ClientSecret {get;set;}

        [JsonProperty("roles")]
        /// <summary> Grant_type demandé</summary>
        public string Roles {get;set;}

        [JsonProperty("contact")]
        /// <summary> Contacts du compte</summary>
        public Contact Contact {get; set;}
    }
}