using RunGymFront.Models;
using RunGymFront.Services;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RunGymFront.Controllers
{
    public class RegistroController : Controller
    {
        string endpoint = "Usuarios/PostUsuarios";

        public ActionResult Registro()
        {
            return View();
        }

        // POST: Registro/Registro
        [HttpPost]
        public async Task<ActionResult> Registro(Usuarios model)
        {
            var response = await Microservices.PostWithoutToken<HttpResponseMessage>(model, endpoint);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                TempData["Error"] = $"Hubo un error al procesar la solicitud: {response.StatusCode}";
                return RedirectToAction("Registro", "Registro");
            }
        }
    }
}