using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; 
using Web.Model.BaseModel;

namespace Web.Model 
{
    [Table("UserAdmin")]
    public partial class tbl_User : BaseEntity
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Image { get; set; }
        public int Gender { get; set; }
        public string? Address { get; set; }
        public bool Active { get; set; }
        public string? Password { get; set; }
        public string? PasswordQuestion { get; set; }
        public string? Phone { get; set; }
        public string? Description { get; set; }
        public string? GroupUserID { get; set; } 
        public bool isAdmin { get; set; }
    }
}
