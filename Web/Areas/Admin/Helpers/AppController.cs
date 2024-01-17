using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Web.Areas.Admin.Models; 
using Web.Model.Constanst;

namespace Web.Areas.Admin.Helpers
{
    public class AppController : Controller
    { 
        protected int UserID; 
        protected AccountProfileModel LoginProfile; 
        protected List<string> UserRoles;

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            LoginProfile = JsonConvert.DeserializeObject<AccountProfileModel>(
              Encoding.UTF8.GetString( Convert.FromBase64String(Request.Cookies[Constants.USER_INFO] ?? string.Empty)));

            UserRoles = JsonConvert.DeserializeObject<List<string>>(
                Encoding.UTF8.GetString(Convert.FromBase64String(Request.Cookies[Constants.ROLES] ?? string.Empty)));  

            UserID = LoginProfile != null ? LoginProfile.ID : 0;
            ViewBag.UserID = UserID;
            ViewBag.FullName = LoginProfile != null ? LoginProfile.FullName : string.Empty;
            ViewBag.UserName = LoginProfile != null ? LoginProfile.UserName : string.Empty;
        }
    }
}
