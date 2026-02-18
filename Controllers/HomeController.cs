using Microsoft.AspNetCore.Mvc;

namespace tp_final_backend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
