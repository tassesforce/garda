using Newtonsoft.Json;

namespace garda.Controllers.Token
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        [JsonRequired]
        public string AccessToken {get;set;}
        [JsonProperty("token_type")]
        [JsonRequired]
        public string TokenType {get;set;}
    }
}