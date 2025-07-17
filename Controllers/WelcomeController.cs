using System.Web.Mvc;

namespace RunGymFront.Controllers
{
    public class WelcomeController : Controller
    {
        public ActionResult Welcome()
        {
            return View();
        }
    }
}
