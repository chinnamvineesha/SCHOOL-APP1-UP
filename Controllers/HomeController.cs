using Microsoft.AspNetCore.Mvc;

namespace SchoolApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
