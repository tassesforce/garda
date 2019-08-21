
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace garda.Models.Data.RevokedToken
{

    [Table("revoked_token")]
    public class RevokedToken
    {

        [Key]
        [Column("refresh_token")]
        public string RefreshToken {get;set;}
        [Column("revocation_date")]
        public DateTime RevocationDate {get;set;}

    }
}