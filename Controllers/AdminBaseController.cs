using FribergRentalCars.Data.Interfaces;
using FribergRentalCars.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FribergRentalCars.Controllers
{
    public class AdminBaseController : Controller
    {
        /*
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var user = HttpContext.Session.Get<User>()
            if()
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
            }
            else
            {

            }

            /*
             // Kontrollera om användaren är inloggad
        if (User.Identity.IsAuthenticated)
        {
            // Kontrollera om användaren har rätt roll (exempel: "Admin")
            if (!User.IsInRole("Admin"))
            {
                // Om användaren inte har rätt behörighet, omdirigera till en annan vy (t.ex. en "Access Denied"-sida)
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" })
                );
            }
        }
        else
        {
            // Om användaren inte är inloggad, omdirigera till inloggningssidan
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "Account", action = "Login" })
            );
        }
            

        }*/
    }
}
