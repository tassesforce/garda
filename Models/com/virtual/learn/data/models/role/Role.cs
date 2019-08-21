
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace garda.Models.Data.RoleData
{

    [Table("role")]
    public class Role
    {

        [Key]
        [Column("id_role")]
        public string IdRole {get;set;}

        [Column("label")]
        [Required]
        public string Label {get;set;}

        [Column("description")]   
        public string Description {get;set;}
    }
}