using Newtonsoft.Json;

namespace garda.Controllers.User
{
    public class UserAuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken {get;set;}
        [JsonProperty("token_type")]
        public string TokenType {get;set;}
        [JsonProperty("refresh_token")]
        public string RefreshToken {get;set;}


    }
}