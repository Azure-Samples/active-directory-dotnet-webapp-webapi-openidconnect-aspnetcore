using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Authentication.Cookies;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoListWebApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public void Login(string returnUrl = null)
        {
            if (HttpContext.User == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                HttpContext.Authentication.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
            }
        }

        // GET: /Account/LogOff
        [HttpGet]
        public void LogOff()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                HttpContext.Authentication.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}
