using System;
using System.Collections.Generic;
using System.Linq;
using garda.Controllers.User;
using garda.Models.Context.ClientContext;
using garda.Models.Context.RoleContext;
using garda.Models.Context.UserAuthContext;
using garda.Models.data.ClientAppData;
using garda.Models.Data.UserAuthData;
using garda.Models.Data.UserRoleData;
using Microsoft.Extensions.Logging;
using SerilogTimings;
using lug.String.Encrypt;
using Microsoft.Extensions.Configuration;
using garda.Exceptions;
using garda.Models.Context.TokenContext;
using garda.Models.Data.RevokedToken;

namespace garda.Query
{   
    /// <summary>Database requests handler</summary>
    public class GardaQueryHandler
    {
        private ILogger Logger;

        private IConfiguration configuration;

        internal GardaQueryHandler(ILogger logger, IConfiguration configuration)
        {
            this.Logger = logger;
            this.configuration = configuration;
        }


        /// <summary>get the client app</summary>
        /// <param name="clientAppContext">DB context</param>
        /// <param name="clientId">ClientId to get</param>
        /// <param name="clientSecret">ClientSecret to check</param>
        /// <returns>ClientApp : corresponding clientApp</returns>
        internal ClientApp GetClientApp(ClientAppDbContext clientAppContext, string clientId, string clientSecret) 
        {
            using (var opClientApp = Operation.Begin("Récupération de l'application cliente {client_id}}", clientId)) 
            {
                IQueryable<ClientApp> clientApps = from clientAppTable in clientAppContext.ClientApps 
                                where clientAppTable.ClientId == clientId
                                select clientAppTable; 
                if (clientApps.Any())
                {
                    foreach (ClientApp c in clientApps)
                    {
                        if (clientSecret.Equals(c.ClientSecret))
                        {
                            opClientApp.Complete();
                            return c;
                        }
                    }
                }
                throw new UnknownClientAppException("Votre application " + clientId + " ne dispose pas des droits d'accès suffisants");
            }
        }

        /// <summary>get informations about a user</summary>
        /// <param name="identifiant">id to search</param>
        /// <param name="userAuthContext">DB context to search in</param>
        internal UserAuth GetUserAuth(UserAuthDbContext userAuthContext, string identifiant)
        {
            using (var opUser = Operation.Begin("Récupération de l'utilisateur {identifiant}", identifiant)) 
            {
                IQueryable<UserAuth> userAuths = from userTable in userAuthContext.UserAuths 
                                where userTable.Login == identifiant
                                select userTable; 
                opUser.Complete();
                UserAuth user = null;
                if (userAuths.Any())
                {
                    user = userAuths.FirstOrDefault();
                    // decrypt the UserId (anonymate the user)
                    user.UserId = StringEncryptHelper.Decrypt(new StringEncryptCriteria{
                        KeySize = int.Parse(configuration["Encrypt:KeySize"]),
                        DerivationIterations = int.Parse(configuration["Encrypt:DerivationIterations"]),
                        PassPhrase = configuration["Encrypt:PassPhrase"],
                        Text = user.UserId
                    });
                }
                return user;
            } 
        }

        /// <summary>Get an account from the UserId</summary>
        /// <param name="userId">Id to search</param>
        /// <param name="userAuthContext">Context of the datas</param>
        internal UserAuth GetUserAuthByUserId(UserAuthDbContext userAuthContext, string userId)
        {
            using (var opUser = Operation.Begin("Récupération de l'utilisateur {identifiant}", userId)) 
            {
                IQueryable<UserAuth> userAuths = from userTable in userAuthContext.UserAuths select userTable; 
                opUser.Complete();
                UserAuth user = null;
                if (userAuths.Any())
                {
                    foreach(UserAuth userAuth in userAuths)
                    {
                        string decryptedUserId = StringEncryptHelper.Decrypt(new StringEncryptCriteria{
                            KeySize = int.Parse(configuration["Encrypt:KeySize"]),
                            DerivationIterations = int.Parse(configuration["Encrypt:DerivationIterations"]),
                            PassPhrase = configuration["Encrypt:PassPhrase"],
                            Text = userAuth.UserId
                        });
                        if (decryptedUserId.Equals(userId)) 
                        {
                            user = userAuth;
                            user.UserId = decryptedUserId;
                        }
                    }
                }
                return user;
            } 
        }

        /// <summary>get the roles of a user</summary>
        /// <param name="identifiant">Id to search</param>
        /// <param name="roleContext">DB context to search in</param>
        /// <returns>List<string> : roles of the user</returns>
        internal List<string> GetRoles(RoleDbContext roleContext, string identifiant)
        {
            using (var opRole = Operation.Begin("Récupération des rôles de l'utilisateur {identifiant}", identifiant)) 
            {
                List<string> rolesDb = (from roleTable in roleContext.Roles
                                join userRoleTable in roleContext.UserRoles on roleTable.IdRole equals userRoleTable.IdRole  
                                where userRoleTable.UserId == identifiant
                                select roleTable.IdRole).ToList(); 
                opRole.Complete();
                return rolesDb;
            }
        }

        /// <summary>Add a user in the DB</summary>
        /// <param name="userAuthContext">DB context to insert in</param>
        /// <param name="request">details of the request</param>
        /// <returns>UserAuth : Added userAuth</returns> 
        internal UserAuth AddUserAuth(UserAuthDbContext userAuthContext, AddUserAuthRequest request)
        {
            var userId = Guid.NewGuid().ToString();
            string phone = "";
            if (!string.IsNullOrEmpty(request.Contact.PhoneNumber)) {
                phone = StringEncryptHelper.Encrypt(new StringEncryptCriteria{
                    KeySize = int.Parse(configuration["Encrypt:KeySize"]),
                    DerivationIterations = int.Parse(configuration["Encrypt:DerivationIterations"]),
                    PassPhrase = configuration["Encrypt:PassPhrase"],
                    Text = request.Contact.PhoneNumber
                });
            }
            var user = new UserAuth
            {
                Login = request.Login,
                Pwd = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password),
                DateC = DateTime.Now,
                UserId = StringEncryptHelper.Encrypt(new StringEncryptCriteria{
                    KeySize = int.Parse(configuration["Encrypt:KeySize"]),
                    DerivationIterations = int.Parse(configuration["Encrypt:DerivationIterations"]),
                    PassPhrase = configuration["Encrypt:PassPhrase"],
                    Text = userId
                }),
                AccountType = request.AccountType,
                PhoneNumber = phone,
                MailAdress = StringEncryptHelper.Encrypt(new StringEncryptCriteria{
                    KeySize = int.Parse(configuration["Encrypt:KeySize"]),
                    DerivationIterations = int.Parse(configuration["Encrypt:DerivationIterations"]),
                    PassPhrase = configuration["Encrypt:PassPhrase"],
                    Text = request.Contact.MailAdress
                })
            };

            userAuthContext.UserAuths.Add(user);
            userAuthContext.SaveChanges();
            // we reset the real userid, not the encrypted one
            user.UserId = userId;
            return user;
        }

        /// <summary>Delete an account nd all the roles linked to it</summary>
        /// <param name="user">user to delete</param>
        /// <param name="userAuthContext">DB context (user)</param>
        /// <param name="roleContext">DB context (roles)</param>
        internal void DeleteUserAuth(RoleDbContext roleContext, UserAuthDbContext userAuthContext, UserAuth user)
        {
            using (var op = Operation.Begin("Suppression de l'utilisateur {identifiant}", user.Login)) 
            {
                
                List<UserRole> userRoles = (from userRole in roleContext.UserRoles
                               where userRole.UserId == user.UserId
                               select userRole).ToList();

                roleContext.UserRoles.RemoveRange(userRoles);
                roleContext.SaveChanges();
                userAuthContext.Remove(user);
                userAuthContext.SaveChanges();
                op.Complete();
            }
        }

        /// <summary>Add roles to a user</summary>
        /// <param name="roleContext">DB context to insert in</param>
        /// <param name="roles">Roles to add to the user</param>
        /// <param name="userId">Id of the user</param>
        internal void AddRolesToUser(RoleDbContext roleContext, string userId, string roles)
        {
            // TODO Verifier que le role existe, sinon on leve une erreur
            Logger.LogInformation("Entree dans AddRolesToUser pour {userId} et {roles}", userId, roles);
            string[] rolesSplit = roles.Split(";");
            if (rolesSplit.Any())
            {
                Array.ForEach(rolesSplit, r => {
                    Logger.LogInformation("boucle pour {userId} et {role}", userId, r);
                    roleContext.UserRoles.Add(new UserRole{
                    IdRole = r,
                    UserId = userId
                });});
                roleContext.SaveChanges();
            }

        }

        /// <summary>Revoke the refreshToken of a user, so we an't use it anymore</summary>
        /// <param name="context">DB Context</param>
        /// <param name="refreshToken">token to revoke</param>
        internal void RevokeToken(RevokedTokenDbContext context, string refreshToken)
        {
            // TODO Verifier que le role existe, sinon on leve une erreur
            Logger.LogInformation("Revocation du token {token}", refreshToken);
            
            context.RevokedTokens.Add(new RevokedToken{
                RefreshToken = refreshToken,
                RevocationDate = DateTime.UtcNow
            });
            context.SaveChanges();

        }

        /// <summary>Check if a refreshToken is revoked or not</summary>
        /// <param name="context">Db context</param>
        /// <param name="refreshToken">token to check</param>
        /// <returns>bool : true if the token is revoked, false otherwise</returns>
        internal bool IsRevokedToken(RevokedTokenDbContext context, string refreshToken)
        {
            using (var opRole = Operation.Begin("Verification du token {token}", refreshToken)) 
            {
                IQueryable<string> query = from revokedTokenTable in context.RevokedTokens
                                where revokedTokenTable.RefreshToken == refreshToken
                                select revokedTokenTable.RefreshToken; 
                if (query.Any())
                {
                    return true;
                }
                return false;
            }
        }

    }
}