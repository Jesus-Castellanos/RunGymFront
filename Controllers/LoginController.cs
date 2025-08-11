using Newtonsoft.Json;
using RunGymFront.Models;
using RunGymFront.Services;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RunGymFront.Controllers
{
    public class LoginController : Controller
    {
        string apiUrl = ConfigurationManager.AppSettings["Api"]?.ToString();
        string contoller = "Auth/Login";

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Login model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                Token token = await Microservices.PostWithoutToken<Token>(model, contoller);
                var jwtHelper = new JwtHelper();
                var principal = jwtHelper.ValidateToken((Token)token);

                if (principal != null)
                {
                    SessionHelper.BearerToken = token.token;
                    SessionHelper.UserName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    SessionHelper.Rol = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    SessionHelper.Usuario = JsonConvert.DeserializeObject<Usuarios>(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value);
                    CookieUpdate(SessionHelper.Usuario);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Token inválido o expirado.");
                    return Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ha ocurrido un error inesperado: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var expiredCookie = new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true // Mantener HttpOnly si estaba configurado
                };
                Response.Cookies.Add(expiredCookie);
            }
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var expiredSessionCookie = new HttpCookie("ASP.NET_SessionId")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true
                };
                Response.Cookies.Add(expiredSessionCookie);
            }

            return RedirectToAction("Login", "Login");
        }

        private void CookieUpdate(Usuarios usuario)
        {
            var ticket = new FormsAuthenticationTicket(2,
                usuario.Nombre,
                DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                false,
                JsonConvert.SerializeObject(usuario)
            );
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket)) { };
            Response.AppendCookie(cookie);
        }
    }
}