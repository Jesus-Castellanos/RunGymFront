using Newtonsoft.Json;
using RunGymFront.Models;
using RunGymFront.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RunGymFront.Controllers
{
    [Authorize]
    public class RutinasController : Controller
    {
        string apiUrl = ConfigurationManager.AppSettings["Api"].ToString();

        public async Task<ActionResult> Rutinas(int id)
        {
            EjercicioDto ejercicio = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);

                    HttpResponseMessage response = await client.GetAsync($"Ejercicios/GetDetalles/{id}");

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Error en la peticion");
                        TempData["Error"] = "No se pudo obtener el ejercicio.";
                        return View();
                    }

                    var res = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(res);

                    ejercicio = JsonConvert.DeserializeObject<EjercicioDto>(res);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                TempData["Error"] = $"Error en la conexión con la API: {ex.Message}";
            }

            return View(ejercicio);
        }
    }
}
