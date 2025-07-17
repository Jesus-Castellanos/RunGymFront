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
    public class TipoDietaController : Controller
    {
        private static string apiURL = ConfigurationManager.AppSettings["Api"];

        // GET: TipoDieta
        public async Task<ActionResult> TipoDieta()
        {
            List<TipoDieta> tipos = await Microservices.GetWithToken<List<TipoDieta>>(null, "TipoDieta/Get");
            return View(tipos);
        }

        public ActionResult Create() => View();

        [HttpPost]
        public async Task<ActionResult> Create(TipoDieta model)
        {
            if (!ModelState.IsValid) return View(model);

            HttpResponseMessage resp;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiURL);
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                resp = await client.PostAsync("TipoDieta/Post", content);
            }

            if (resp.IsSuccessStatusCode)
                return RedirectToAction("Index");
            else
                return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            TipoDieta tipo = await Microservices.GetWithToken<TipoDieta>(null, $"TipoDieta/Get/{id}");
            if (tipo == null)
                return HttpNotFound();
            return View(tipo);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(TipoDieta model)
        {
            if (!ModelState.IsValid) return View(model);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiURL);
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                await client.PutAsync($"TipoDieta/Put/{model.Id}", content);
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int id)
        {
            await Microservices.DeleteWithToken($"TipoDieta/Delete/{id}");
            return RedirectToAction("Index");
        }
    }
}
