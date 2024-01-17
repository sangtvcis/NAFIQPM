using System.Text.Json.Serialization;

namespace Web.Areas.Admin.Models
{
    public class LoginModel
    { 
        public string UserName { get; set; } 
        public string Password { get; set; }
    }
}
