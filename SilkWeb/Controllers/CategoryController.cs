using Microsoft.AspNetCore.Mvc;
using SilkWeb.Data;
using SilkWeb.Models;

namespace SilkWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString()) 
            {
                ModelState.AddModelError("name", "The Display Order cannot exactly match the Name.");
            }
            if (obj.Name!=null && obj.Name.ToString() == "test")
            {
                ModelState.AddModelError("", "test is invalid Name");       //asp-validation-summary="ModelOnly" will display this but not above error bind with property name 
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
    }
}
