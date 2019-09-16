namespace garda.Services.Historisation
{
    /// <summary>Interface to historise traffic on the application</summary>
    public interface IHistoService
    {
        /// <summary>Action to historize the traffic</summary>
        /// <param name="histoUserAuth">Datas to save for the historization</param>
        void Historize(HistoUserAuth histoUserAuth);
    }
}