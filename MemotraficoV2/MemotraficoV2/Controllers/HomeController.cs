using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return PartialView("../Account/Login");
            }
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Sistema para la Atención de Solicitudes";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "ICHIFE - TECNOLOGÍAS DE LA INFORMACIÓN";

            return View();
        }
    }
}
