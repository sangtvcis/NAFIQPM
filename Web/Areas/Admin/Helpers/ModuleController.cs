using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Areas.Admin.Helpers
{
    [Area("Admin")]
    public class ModuleController : AppController
    { 
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context); 
        }
    }
}
