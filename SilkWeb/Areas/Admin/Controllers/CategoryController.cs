using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Silk.DataAccess.Data;
using Silk.DataAccess.Repository;
using Silk.DataAccess.Repository.IRepository;
using Silk.Models;
using Silk.Utility;

namespace SilkWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]               //so that by just having the url,no one should be able to access the controllers except if user is admin type
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _db;     -- commented bociz of repo implementation
        //private readonly ICategoryRepository _categoryRepo;     -- commented bociz of unitofwork implementation
        private readonly IUnitOfWork _unitOfWork;

        //public CategoryController(ApplicationDbContext db)
        //public CategoryController(ICategoryRepository categoryRepo)
        public CategoryController(IUnitOfWork unitOfWork)
        {
            //_db = db;
            //_categoryRepo= categoryRepo
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //List<Category> objCategoryList = _db.Categories.ToList();
            //List<Category> objCategoryList = _categoryRepo.GetAll().ToList();
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
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
            if (obj.Name != null && obj.Name.ToString() == "test")
            {
                ModelState.AddModelError("", "test is invalid Name");       //asp-validation-summary="ModelOnly" will display this but not above error bind with property name 
            }
            if (ModelState.IsValid)
            {
                //_db.Categories.Add(obj);
                //_db.SaveChanges();
                /*_categoryRepo.Add(obj);
                _categoryRepo.Save();*/
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
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
            ///*Category? categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);                  -- commented becoz using category repo*/ 
            //////Category? categoryFromDb = _categoryRepo.Get(u=> u.Id == id);                             -- commented bociz of unitofwork implementation
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();  --used when some more filtering required in Where clause
            if (categoryFromDb == null)
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
                /*_db.Categories.Update(obj);
                _db.SaveChanges();*/
                /*_categoryRepo.Update(obj);
                _categoryRepo.Save();*/
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
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
            //Category? categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);                    -- commented becoz using category repo
            ///Category? categoryFromDb = _categoryRepo.Get(u => u.Id == id);                               -- commented bociz of unitofwork implementation
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();  --used when some more filtering required in Where clause
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            //Category? obj = _db.Categories.FirstOrDefault(c => c.Id == id);                               -- commented becoz using category repo
            //Category? obj = _categoryRepo.Get(u => u.Id == id);                                           -- commented bociz of unitofwork implementation
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            /*_db.Categories.Remove(obj);
            _db.SaveChanges();*/
            /*_categoryRepo.Remove(obj);
            _categoryRepo.Save();*/
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully!";
            return RedirectToAction("Index", "Category");
        }
    }
}
