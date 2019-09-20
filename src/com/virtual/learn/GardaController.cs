using System;
using garda.Models.Context.ClientContext;
using garda.Models.Context.RoleContext;
using garda.Models.data.ClientAppData;
using garda.Query;
using garda.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SerilogTimings;
using garda.Services.Historisation;

namespace garda.Controllers
{
    public abstract class GardaController : ControllerBase
    {
        /// <summary>Logger par defaut</summary>
        protected ILogger logger;
        /// <summary>Lien vers le fichier de configuration</summary>
        protected IConfiguration configuration;
        /// <summary>Context des applications consommatrice</summary>
        protected readonly ClientAppDbContext clientAppContext;
        /// <summary>Context des roles</summary>
        protected readonly RoleDbContext roleContext;

        protected GardaController (ClientAppDbContext clientAppContext, RoleDbContext roleContext)
        {
            this.clientAppContext = clientAppContext;
            this.roleContext = roleContext;
        }

        /*
         * Verification de la presence et de la validite de l'application consommatrice
         */
        protected void CheckClientApp(ClientAppDbContext clientAppDbContext, string clientId, string clientSecret)
        {
            ClientApp clientApp = null;
            GardaQueryHandler handler = null;
            using (var opRole = Operation.Begin("Récupération de l'application consommatrice {client_id}", clientId)) 
            {
                handler = new GardaQueryHandler(logger, configuration);
                try 
                {
                    clientApp = handler.GetClientApp(clientAppDbContext, clientId, clientSecret);
                    opRole.Complete();
                } catch (InvalidOperationException e) {
                    throw new UnknownClientAppException("Votre application " + clientId + " ne dispose pas des droits d'accès suffisants", e);
                }   
            }
            if (clientApp == null)    
            {     
                throw new UnknownClientAppException("Votre application " + clientId + " ne dispose pas des droits d'accès suffisants");
            }     
        }
    }
}