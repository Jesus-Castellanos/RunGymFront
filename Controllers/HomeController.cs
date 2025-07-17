using Newtonsoft.Json;
using RunGymFront.Models;
using RunGymFront.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TuProyecto.Controllers;

namespace RunGymFront.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private static readonly string apiURL = ConfigurationManager.AppSettings["Api"];

        public ActionResult Index()
        {
            int inactivityLimit = int.Parse(ConfigurationManager.AppSettings["InactivityLimitSeconds"] ?? "300");
            ViewBag.InactivityLimitSeconds = inactivityLimit;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(ErroresReportados model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new
                {
                    success = false,
                    message = "Por favor corrige los errores en el formulario.",
                    errors
                });
            }

            model.FechaReporte = model.FechaReporte == default ? DateTime.Now : model.FechaReporte;

            try
            {
                string apiUrl = ConfigurationManager.AppSettings["Api"];
                if (string.IsNullOrEmpty(apiUrl))
                {
                    return Json(new
                    {
                        success = false,
                        message = "La URL de la API no está configurada correctamente."
                    });
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl.TrimEnd('/'));
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("api/ErroresReportados/PostErroresReportados", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new
                        {
                            success = true,
                            message = "¡Reporte enviado con éxito! Hemos recibido tu reporte y lo revisaremos pronto."
                        });
                    }

                    string apiError = await response.Content.ReadAsStringAsync();
                    return Json(new
                    {
                        success = false,
                        message = "Error al enviar el reporte. Por favor, inténtalo de nuevo más tarde.",
                        apiError
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Ocurrió un error inesperado: {ex.Message}"
                });
            }
        }
    }
}
