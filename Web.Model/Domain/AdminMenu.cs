using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Web.Model.BaseModel;

namespace Web.Model 
{
    [Table("AdminMenu")]
    public partial class AdminMenu : BaseEntity
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public int ParentID { get; set; }

        [Required]
        public string Url { get; set; }
        public string? Controller { get; set; }
        public bool Active { get; set; }
        public string? Icon { get; set; }
        public string? LangCode { get; set; }
        public int Level { get; set; }
    }
}
