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
        [HttpGet]
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
                TempData["success"] = "Category Created Successfully!";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) 
            {
                return NotFound();
            }
            //Category? categoryFromDb = _db.Categories.Find(id);  --only works with primary Key
            Category? categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);
            //Category? categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();  --used when some more filtering required in Where clause
            if(categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Category Edited Successfully!";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Category? categoryFromDb = _db.Categories.Find(id);  --only works with primary Key
            Category? categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);
            //Category? categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();  --used when some more filtering required in Where clause
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _db.Categories.FirstOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();  
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Category Deleted Successfully!";
            return RedirectToAction("Index", "Category");
        }
    }
}
