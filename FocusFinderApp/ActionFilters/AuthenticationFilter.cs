using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FocusFinderApp.ActionFilters
{
    public class AuthenticationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                // User is logged in, set ViewBag for the views
                context.HttpContext.Items["IsLoggedIn"] = true;
                context.HttpContext.Items["Username"] = context.HttpContext.Session.GetString("Username");
            }
            else
            {
                // User is NOT logged in
                context.HttpContext.Items["IsLoggedIn"] = false;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after execution
        }
    }
}
