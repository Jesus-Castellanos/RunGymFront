using Newtonsoft.Json;
using RunGymFront.Models;
using RunGymFront.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RunGymFront.Controllers
{
    [Authorize]
    public class MetasController : Controller
    {
        private static string apiURL = ConfigurationManager.AppSettings["Api"].ToString();
        // GET: Metas/MisMetas
        public async Task<ActionResult> MisMetas()
        {
            try
            {
                // Obtener todas las metas de la API
                List<Metas> metas = await Microservices.GetWithToken<List<Metas>>(null, "Metas/GetMetas");

                // Obtener el Id del usuario logueado, asumiendo que está en SessionHelper.Usuario.Id
                int usuarioId = 0;
                if (SessionHelper.Usuario != null)
                    usuarioId = SessionHelper.Usuario.Id;
                else
                    return RedirectToAction("Login", "Login"); // O maneja el caso de usuario no autenticado

                // Filtrar metas por usuario
                metas = metas.FindAll(m => m.IdUsuario == usuarioId);

                TempData["Metas"] = JsonConvert.SerializeObject(metas);
                return View(metas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al obtener metas: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Metas/Metas
        public ActionResult Metas()
        {
            return View();
        }

        // POST: Metas/MetasCreate
        [HttpPost]
        public async Task<ActionResult> MetasCreate(Metas model)
        {
            if (!ModelState.IsValid)
            {
                // Retornar la vista con el modelo para que se corrijan los errores
                return View("Metas", model);
            }

            try
            {
                // Asignar IdUsuario desde la sesión (usuario autenticado)
                if (SessionHelper.Usuario != null)
                    model.IdUsuario = SessionHelper.Usuario.Id;
                else
                {
                    TempData["Error"] = "Usuario no autenticado.";
                    return RedirectToAction("Login", "Login");
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(apiURL);
                    string json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("Metas/PostMetas", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "No se pudo crear la meta. Intenta nuevamente.";
                        return View("Metas", model);
                    }
                }

                return RedirectToAction("MisMetas");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Hubo un error al procesar la solicitud: {ex.Message}";
                return View("Metas", model);
            }
        }
    }
}
