using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Web.Model 
{
    [Table("Roles")]
    public class Roles
    {
        [Key]
        public int ID { get; set; }
        public string? KeyRole { get; set; }
        public int MenuID { get; set; } 
    }
}
