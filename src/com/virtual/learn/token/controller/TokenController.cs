using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using garda.Constant.Claim;
using garda.Exceptions;
using garda.Models.Context.ClientContext;
using garda.Models.Context.RoleContext;
using garda.Models.Context.TokenContext;
using garda.Models.Context.UserAuthContext;
using garda.Models.Data.UserAuthData;
using garda.Query;
using lug.Handler.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SerilogTimings;

namespace garda.Controllers.Token
{
    [ApiController]
    public class TokenController : GardaController
    {
        private readonly RevokedTokenDbContext revokedTokenContext;

        public TokenController(IConfiguration configuration, RevokedTokenDbContext revokedTokenContext, ClientAppDbContext clientAppContext, 
                    RoleDbContext roleContext, ILogger<TokenController> logger) : base(clientAppContext, roleContext, null)
        {
            this.configuration = configuration;
            this.revokedTokenContext = revokedTokenContext;
            this.logger = logger;
        }

        /*
         * Methode POST de recuperation du jeton JWT pour un utilisateur
         */
        [Route("/token/")]
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(TokenResponse))]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesResponseType(401)]
        public IActionResult RefreshToken( [FromBody] TokenRequest tokenRequest)
        {
            using (Operation.Time("Demande de rafraichissement de token")) 
            {
                GardaQueryHandler handler = new GardaQueryHandler(logger, configuration);
                // Verification des appli conso
                CheckClientApp(clientAppContext, tokenRequest.ClientId, tokenRequest.ClientSecret);
                
                bool revoked = handler.IsRevokedToken(revokedTokenContext, tokenRequest.RefreshToken);
                if (revoked)
                {
                    throw new RevokedTokenException("Ce jeton a été révoqué, merci de vous authentifier de nouveau");
                }

                // Recuperation des roles de l'utilisateur
                List<string> roles = handler.GetRoles(roleContext, tokenRequest.UserId);
                
        
                using (Operation.Time("Génération du token"))
                {
                    lug.Handler.Token.TokenHandler tokenHandler = new lug.Handler.Token.TokenHandler(logger);
                    JwtSecurityToken token = tokenHandler.ReadToken(tokenRequest.AccessToken);
                    Claim loginClaim = token.Payload.Claims.Single(c => c.Type == ClaimsConstant.USER_ID);
                    Claim accountTypeClaim = token.Payload.Claims.Single(c => c.Type == ClaimsConstant.ACCOUNT_TYPE);

                    TokenCriterias criterias = new TokenCriterias
                    {
                        Audience = configuration["JWT:Audience"],
                        Issuer = configuration["JWT:Issuer"],
                        Key = configuration["JWT:Key"],
                        Roles = roles,
                        TokenDurability = int.Parse(configuration["JWT:Durability"]),
                        UserId = tokenRequest.UserId,
                        ClientId = tokenRequest.ClientId,
                        AccountType = accountTypeClaim.Value,
                        Login = loginClaim.Value
                    };
                    var accessToken = tokenHandler.GenerateAccessToken(criterias);

                    TokenResponse response = new TokenResponse();                
                    response.AccessToken = accessToken;
                    response.TokenType = configuration["JWT:TokenType"];
                    return Ok(response);
                }
                
            }
        }

        /*
         * Methode POST de recuperation du jeton JWT pour un utilisateur
         */
        [Route("/token/")]
        [HttpDelete]
        [ProducesResponseType(200)]
        public IActionResult RevokeToken( [FromBody] TokenRevocationRequest tokenRequest)
        {
            using (Operation.Time("Révocation du token")) 
            {
                GardaQueryHandler handler = new GardaQueryHandler(logger, configuration);
                CheckClientApp(clientAppContext, tokenRequest.ClientId, tokenRequest.ClientSecret);
                handler.RevokeToken(revokedTokenContext, tokenRequest.RefreshToken);
                return Ok();
            }
        }
    }
}