using System; 
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; 
using Web.Model.BaseModel;

namespace Web.Model 
{
    [Table("tbl_GroupUser")]
    public class GroupUser : BaseEntity
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string? Name { get; set; }
        public bool Status { get; set; }
        public bool IsAdministrator { get; set; }
    }
}
