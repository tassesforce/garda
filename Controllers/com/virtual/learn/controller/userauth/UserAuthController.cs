using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using garda.Exceptions;
using garda.Models.Context.ClientContext;
using garda.Models.Context.RoleContext;
using garda.Models.Context.UserAuthContext;
using garda.Models.data.ClientAppData;
using garda.Models.Data.RoleData;
using garda.Models.Data.UserAuthData;
using garda.Query;
using lug.Handler.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SerilogTimings;

namespace garda.Controllers.User
{

    /*
     * Controller de gestion des utilisateurs 
     */
    [Route("/")]
    [ApiController]
    public class UserAuthController : GardaController
    {
        private readonly UserAuthDbContext userAuthContext;

        public UserAuthController(IConfiguration configuration, UserAuthDbContext userAuthContext, ClientAppDbContext clientAppContext, 
                    RoleDbContext roleContext, ILogger<UserAuthController> logger) : base(clientAppContext, roleContext)
        {
            this.configuration = configuration;
            this.userAuthContext = userAuthContext;
            this.logger = logger;
        }

        /*
         * Methode POST de recuperation du jeton JWT pour un utilisateur
         */
        [Route("auth/{login}")]
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(UserAuthResponse))]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesResponseType(401)]
        public IActionResult AuthenticateUser( [FromBody] UserAuthRequest userRequest, [FromRoute] string login)
        {
            using (Operation.Time("Demande d'authentification")) 
            {
                GardaQueryHandler handler = new GardaQueryHandler(logger, configuration);
                // Verification des appli conso
                CheckClientApp(clientAppContext, userRequest.ClientId, userRequest.ClientSecret);
                
                UserAuth userAuth = handler.GetUserAuth(userAuthContext, login);
                if (userAuth == null) {
                    throw new UnknownUserException("L'utilisateur " + login + " n'a pas été trouvé dans notre référentiel");
                }
                CheckUserAccount(userAuth, userRequest.Password);

                // Recuperation des roles de l'utilisateur
                List<string> roles = handler.GetRoles(roleContext, userAuth.UserId);
                
                using (Operation.Time("Génération des tokens")) 
                {
                    UserAuthResponse response = GenerateToken(userAuth, userRequest.ClientId, roles);
                    
                    return Ok(response);
                }
            }
        }

        /*
         * Methode POST de recuperation du jeton JWT pour un utilisateur
         */
        [Route("auth/")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(UserAuthResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public IActionResult AddUser( [FromBody] AddUserAuthRequest UserAddRequest)
        {
            using (Operation.Time("Ajout de l'utilisateur {login}", UserAddRequest.Login)) 
            {
                GardaQueryHandler handler = new GardaQueryHandler(logger, configuration);
                // Verification des appli conso
                CheckClientApp(clientAppContext, UserAddRequest.ClientId, UserAddRequest.ClientSecret);
                
                // Verification de l'absence du compte utilisateur
                UserAuth userAuth = handler.GetUserAuth(userAuthContext, UserAddRequest.Login);
                
                if (userAuth != null)
                {
                    throw new KnownUserException("Utilisateur " + UserAddRequest.Login + " connu de notre référentiel");
                }
                UserAuth user = handler.AddUserAuth(userAuthContext, UserAddRequest);
                if (UserAddRequest.Roles.Any())
                {
                    handler.AddRolesToUser(roleContext, user.UserId, UserAddRequest.Roles);
                }

                using (Operation.Time("Génération des tokens")) 
                {
                    UserAuthResponse response = GenerateToken(user, UserAddRequest.ClientId, UserAddRequest.Roles.Split(";").ToList());
                    
                    return Created(configuration["API:Urls:Garda:Auth"], response);
                }
            }
        }

        /// <summary> Methode DELETE de suppression de compte utilisateur</summary>
        [Route("auth")]
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteUser( [FromBody] DeleteUserAuthRequest userRequest, [FromQuery] string login, [FromQuery] string userId)
        {
            using (Operation.Time("Suppression de l'utilisateur {login}", string.IsNullOrEmpty(login) ? login : userId)) 
            {
                GardaQueryHandler handler = new GardaQueryHandler(logger, configuration);
                // Verification des appli conso
                CheckClientApp(clientAppContext, userRequest.ClientId, userRequest.ClientSecret);
                
                // See if the userAuth exists
                UserAuth userAuth = null;
                if (!string.IsNullOrEmpty(login)) 
                {
                    logger.LogInformation("Récupération du compte {id} par login", login);
                    userAuth = handler.GetUserAuth(userAuthContext, login);
                } else 
                {
                    logger.LogInformation("Récupération du compte {id} par userId", userId);
                    userAuth = handler.GetUserAuthByUserId(userAuthContext, userId);
                }
                
                if (userAuth == null)
                {
                    throw new UnknownUserException("Utilisateur " + login + " est inconnu dans notre référentiel");
                }
                handler.DeleteUserAuth(roleContext, userAuthContext, userAuth);

                return Ok();
            }
        }

        /// <summary>Verifie la validite du compte</summary>
        /// <param name="password">Mot de passe a verifier</param>
        /// <param name="userAuth">Compte a verifier</param>
        private void CheckUserAccount(UserAuth userAuth, string password)
        {
            // Verification du compte utilisateur
            if (!BCrypt.Net.BCrypt.EnhancedVerify(password, userAuth.Pwd)) {
                throw new UnauthorizedUserException("L'utilisateur " + userAuth.Login + " n'est pas autorisé");
            }
        }

        /// <summary>Generation du token a renvoyer pour l'utilisateur</summary>
        /// <param name="roles">Liste des roles de l'utilisateur</param>
        /// <param name="userAuth">Contient les informations du compte</param>
        /// <param name="userRequest">Requete de base</param>
        /// <returns>UserAuthResponse : contient les donnees du token</returns>
        private UserAuthResponse GenerateToken(UserAuth userAuth, string clientId, List<string> roles)
        {
            lug.Handler.Token.TokenHandler tokenHandler = new lug.Handler.Token.TokenHandler(logger);
            TokenCriterias criterias = new TokenCriterias
            {
                Audience = configuration["JWT:Audience"],
                Issuer = configuration["JWT:Issuer"],
                Key = configuration["JWT:Key"],
                Login = userAuth.Login,
                AccountType = userAuth.AccountType,
                Roles = roles,
                TokenDurability = int.Parse(configuration["JWT:Durability"]),
                UserId = userAuth.UserId,
                ClientId = clientId
            };
            string accessToken = tokenHandler.GenerateAccessToken(criterias);
            // validite du refresh token => 8h
            criterias.TokenDurability = int.Parse(configuration["JWT:RefreshDurability"]);
            string refreshToken = tokenHandler.GenerateRefreshToken(criterias);
            UserAuthResponse response = new UserAuthResponse();
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;
            response.TokenType = configuration["JWT:TokenType"];
            return response;
        }

    }
}
