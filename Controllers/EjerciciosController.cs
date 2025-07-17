using Newtonsoft.Json;
using RunGymFront.Models;
using RunGymFront.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RunGymFront.Controllers
{
    public class EjerciciosController : Controller
    {

        public async Task<ActionResult> Ejercicios()
        {
            var ejercicios = await Microservices.GetWithToken<List<Ejercicios>>(null, "Ejercicios/GetEjercicios");
            TempData["Ejercicios"] = JsonConvert.SerializeObject(ejercicios);
            return View(ejercicios);
        }
    }
}
