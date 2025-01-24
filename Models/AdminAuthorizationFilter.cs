using FribergRentalCars.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FribergRentalCars.Models
{
    public class AdminAuthorizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Control User "role" (fetch session value)
            var user = filterContext.HttpContext.Session.GetInt32("isAdmin");
            // User is not an administrator
            if (user == null || user != 1)
            {
                // Redirect to 'Access Denied' page
                filterContext.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
