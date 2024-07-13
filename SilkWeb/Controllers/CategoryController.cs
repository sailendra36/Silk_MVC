using Microsoft.AspNetCore.Mvc;

namespace SilkWeb.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
