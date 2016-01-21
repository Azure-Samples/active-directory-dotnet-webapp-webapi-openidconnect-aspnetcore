using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoListWebApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (HttpContext.User == null || !HttpContext.User.Identity.IsAuthenticated)
                return new ChallengeResult(OpenIdConnectAuthenticationDefaults.AuthenticationType, new AuthenticationProperties { RedirectUri = "/" });
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/LogOff
        [HttpGet]
        public IActionResult LogOff()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                {
                    OpenIdConnectAuthenticationDefaults.AuthenticationType,
                    CookieAuthenticationDefaults.AuthenticationType
                });
            return RedirectToAction("Index", "Home");
        }
    }
}
