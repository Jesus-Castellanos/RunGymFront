using RunGymFront.Models;
using RunGymFront.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RunGymFront.Controllers
{
    [Authorize]
    public class DietaController : Controller
    {
        public ActionResult Dieta()
        {
            if (SessionHelper.Rol == "1")
                return RedirectToAction("AdminDietas");

            var usuario = SessionHelper.Usuario;
            if (usuario == null)
                return RedirectToAction("Error", "Home");

            // muestra la vista por el peso //
            float peso = usuario.Peso;
            int IdTipoDieta;
            if (peso < 50)
                
                return RedirectToAction("Dieta1");
            
            else if (peso >= 50 && peso <= 80)
            
                return RedirectToAction("Dieta2");
            
            else
            
                return RedirectToAction("Dieta3");
            
        }
        public async Task<ActionResult> AdminDietas()
        {
            try
            {
                if (SessionHelper.Usuario == null || string.IsNullOrEmpty(SessionHelper.BearerToken))
                {
                    TempData["Error"] = "Debe iniciar sesión.";
                    return RedirectToAction("Login", "Login");
                }

                // ✅ Llama al microservicio para obtener las dietas
                var dietas = await Microservices.GetWithToken<List<Dieta>>(null, "Dieta/GetDieta");

                // ✅ Pasa la lista a la vista sin tocar el HTML existente
                return View(dietas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las dietas: {ex.Message}";
                return View(new List<Dieta>()); // Vista vacía pero funcional
            }
        }

        [HttpGet]
        public async Task<ActionResult> Dieta1()
        {
            var response = await Microservices.GetWithToken<List<Dieta>>(null, "Dieta/GetTipoDieta/1");
            return View(response);
        }

        [HttpGet]
        public async Task<ActionResult> Dieta2()
        {
            var response = await Microservices.GetWithToken<List<Dieta>>(null, "Dieta/GetTipoDieta/2");
            return View(response);
        }

        [HttpGet]

        public async Task<ActionResult> Dieta3()
        {
            var response = await Microservices.GetWithToken<List<Dieta>>(null, "Dieta/GetTipoDieta/3");
            return View(response);
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            try
            {
                if (SessionHelper.Usuario == null)
                {
                    TempData["Error"] = "Debe iniciar sesión.";
                    return RedirectToAction("Login", "Login");
                }

                // ✅ Llamada correcta al endpoint con interpolación y tipo correcto
                var dieta = await Microservices.GetWithToken<Dieta>(null, $"Dieta/GetDieta/{id}");

                if (dieta == null)
                {
                    TempData["Error"] = $"No se encontró la dieta con ID {id}.";
                    return RedirectToAction("AdminDietas");
                }

                return View(dieta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la dieta: {ex.Message}";
                return RedirectToAction("AdminDietas");
            }
        }


        // Recibir cambios del formulario y actualizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarDietas(Dieta model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var updatedDieta = await Microservices.PutWithToken<Dieta>(model, $"Dieta/PutDieta/{model.Id}");

                if (updatedDieta == null)
                {
                    ModelState.AddModelError("", "Error al actualizar la dieta.");
                    return View(model);
                }

                TempData["Success"] = "Dieta actualizada correctamente.";
                return RedirectToAction("AdminDietas");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar: {ex.Message}");
                return View(model);
            }
        }

        // POST: EliminarConfirmado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarConfirmado(int id)
        {
                var success = await Microservices.DeleteWithToken($"Dieta/DeleteDieta/{id}");

                if (!success)
                    TempData["Error"] = "No se pudo eliminar la dieta.";
                else
                    TempData["Success"] = "Dieta eliminada correctamente.";

                return RedirectToAction("Dieta", "Dieta");
        }
    }
}