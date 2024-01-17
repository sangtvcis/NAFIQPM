using System; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Web.Model
{
    [Table("Permission")]
    public partial class Permissions
    {
        [Key]
        public int GroupID { get; set; }
        [Key]
        public int RoleID { get; set; }
        [Key]
        public string Action { get; set; }
    }
}
