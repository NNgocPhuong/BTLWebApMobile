using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Central_server.Filters
{
    public class SessionAuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (session.GetString("UserId") == null)
            {
                context.Result = new RedirectToActionResult("Login", "Users", null);
            }
            else
            {
                base.OnActionExecuting(context);
            }
        }
    }

}
