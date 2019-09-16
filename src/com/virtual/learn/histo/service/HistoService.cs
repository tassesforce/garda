using lug.String.Encrypt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SerilogTimings;

namespace garda.Services.Historisation
{
    /// <summary>Implementation to historise traffic on the application</summary>
    public class HistoService : IHistoService
    {

        /// <summary>Used to logs informations and other stuff</summary>
        private ILogger Logger;

        /// <summary>Configuration to get all parameters </summary>
        private IConfiguration configuration;

        /// <summary> Context to store historization datas </summary>
        private HistoUserAuthDbContext histoUserAuthContext;

        public HistoService(ILogger logger, IConfiguration configuration, HistoUserAuthDbContext histoUserAuthContext)
        {
            this.Logger = logger;
            this.configuration = configuration;
        }

        /// <summary>Action to historize the traffic</summary>
        /// <param name="histoUserAuth">Datas to save for the historization</param>
        public void Historize(HistoUserAuth histoUserAuth)
        {
            try
            {
                histoUserAuthContext.HistoUserAuths.Add(histoUserAuth);
                histoUserAuthContext.SaveChanges();
            } catch (System.Exception e)
            {
                Logger.LogError("An error occured during historization : {error}", e.Message);
            }
        }
    }
}