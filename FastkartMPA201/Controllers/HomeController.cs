using Microsoft.AspNetCore.Mvc;

namespace FastkartMPA201.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
