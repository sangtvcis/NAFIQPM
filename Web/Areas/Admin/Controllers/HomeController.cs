using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Helpers;

namespace Web.Areas.Admin.Controllers
{  
    public class HomeController : ModuleController
    {
        [Authorize(Roles = "Home")]
        public IActionResult Index()
        { 
            return View();
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
