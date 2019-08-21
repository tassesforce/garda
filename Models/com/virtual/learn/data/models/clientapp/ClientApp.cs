
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace garda.Models.data.ClientAppData
{
    [Table("client_app")]
    public class ClientApp
    {

        [Key]
        [Column("client_id")]
        public string ClientId {get;set;}

        [Key]
        [Column("client_secret")]
        public string ClientSecret {get;set;}

    }
}