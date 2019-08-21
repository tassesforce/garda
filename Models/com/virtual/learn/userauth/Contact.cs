using Newtonsoft.Json;

namespace garda.Controllers.User
{
    public class Contact
    {
        [JsonProperty("phoneNumber")]
        /// <summary>Numero de telephone du compte</summary>
        public string PhoneNumber {get; set;}
        [JsonProperty("mailAdress")]
        /// <summary>Adresse mail du compte</summary>
        public string MailAdress {get; set;}
    }
}