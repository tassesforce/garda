
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace garda.Models.Data.UserAuthData
{
    [Table("user_auth")]
    public class UserAuth
    {

        [Key]
        [Column("login")]
        public string Login {get;set;}

        [Column("pwd")]
        [Required]
        public string Pwd {get;set;}

        [Column("account_type")]
        [Required]
        public string AccountType {get;set;}

        [Column("user_id")]
        [Required]
        public string UserId {get;set;}
        
        [Column("date_c")]
        [DataType( DataType.Date )]   
        public DateTime DateC {get;set;}

        [Column("date_l_c")]
        [DataType( DataType.Date )]   
        public DateTime DateLC {get;set;}

        [Column("revoked_user")]
        public bool RevokedUser {get;set;}

        [Column("phone_number")]
        public string PhoneNumber {get;set;}

        [Column("mail_adress")]
        public string MailAdress {get;set;}
        
        public UserAuth() {
            
        }
    }
}