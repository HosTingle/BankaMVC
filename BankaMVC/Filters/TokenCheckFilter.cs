using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BankaMVC.Filters
{
    public class TokenCheckFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Cookies["AuthToken"];

            var routeData = context.RouteData;
            var controller = routeData.Values["controller"]?.ToString();
            var action = routeData.Values["action"]?.ToString();

            // Giriş veya kayıt sayfasındaysa filtreyi atla
            if (controller == "Giris" && (action == "Index" || action == "Giris") ||
                controller == "Kayit" && (action == "Index" || action == "Kayit"))
            {
                base.OnActionExecuting(context);
                return;
            }

            // Token yoksa giriş sayfasına yönlendir
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Giris", action = "Index" })
                );
            }

            base.OnActionExecuting(context);
        }

    }
}
