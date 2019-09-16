using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace garda.Services.Historisation
{
    [Table("histo_user_auth")]
    public class HistoUserAuth
    {

        [Column("correlation_id")]
        /// <summary>Correlation_Id of the request (to find it in the logs)</summary>
        public string CorrelationId {get; set;}

        [Column("connecting_ip")]
        /// <summary>IP that's done the action</summary>
        public string ConnectingIp {get; set;}
        
        [Column("action_done")]
        /// <summary>Type of action done</summary>
        public GardaActionEnum ActionDone {get; set;}
        
        [Column("date_action")]
        /// <summary>Date the action was done</summary>
        public DateTime DateAction {get; set;}
        
        [Column("status_action")]
        /// <summary>HTTP status resulting of the action</summary>
        public string StatusAction {get; set;}

        [Column("login")]
        /// <summary> Id of the user to log in</summary>
        public string Login {get; set;}

        [Column("account_type")]
        /// <summary> Account type (agency, company, candidate, etc.)</summary>
        public string AccountType {get; set;}
        
        [Column("password")]
        /// <summary> Password of the user</summary>
        public string Password {get;set;}
        
        [Column("refresh_token")]
        /// <summary> Phone number of the account</summary>
        public string PhoneNumber {get; set;}

        [Column("mail_adress")]
        /// <summary> Mail adress of the account</summary>
        public string MailAdress {get; set;}
    }
}