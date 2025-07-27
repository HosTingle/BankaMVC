using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankaMVC.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Linq;

    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _allowedRoles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                if (context.HttpContext.Request.Cookies.ContainsKey("AuthToken"))
                {
                    context.HttpContext.Response.Cookies.Delete("AuthToken");
                }
            }
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Giris", action = "Index" })
                );
                return;
            }
            if (!_allowedRoles.Any(role => user.IsInRole(role)))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Hesaplar", action = "Yetkisiz" }) 
                );
            }
        }
    }

}
