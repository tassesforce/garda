
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace garda.Models.Data.UserRoleData
{

    [Table("user_roles")]
    public class UserRole
    {

        [Column("id_role")]
        [Key]
        public string IdRole {get;set;}

        [Column("user_id")]
        [Key]
        public string UserId {get;set;}

    }
}