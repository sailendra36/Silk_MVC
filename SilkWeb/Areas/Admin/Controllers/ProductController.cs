using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Silk.DataAccess.Data;
using Silk.DataAccess.Repository;
using Silk.DataAccess.Repository.IRepository;
using Silk.Models;
using Silk.Models.ViewModels;

namespace SilkWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _db;     -- commented bociz of repo implementation
        //private readonly IProductRepository _productRepo;     -- commented bociz of unitofwork implementation
        private readonly IUnitOfWork _unitOfWork;

        //public ProductController(ApplicationDbContext db)
        //public ProductController(IProductRepository productRepo)
        public ProductController(IUnitOfWork unitOfWork)
        {
            //_db = db;
            //_productRepo= productRepo
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //List<Product> objProductList = _db.Products.ToList();
            //List<Product> objProductList = _productRepo.GetAll().ToList();
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
            return View(objProductList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            /*example of projection in ef core - selecting specific column and creating new object out of it*/
            /*IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll()
                               .Select(u => new SelectListItem        
                               {
                                   Text = u.Name,                                                                   --no need now because of viewModel
                                   Value = u.Id.ToString()
                               });
            ViewData["CategoryList"] = CategoryList;
            ViewBag.CategoryList = CategoryList; */
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll() .Select(u => new SelectListItem
                               {
                                   Text = u.Name,
                                   Value = u.Id.ToString()
                               }),
                Product = new Product()
            };

            return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                //_db.Products.Add(obj);
                //_db.SaveChanges();
                /*_productRepo.Add(obj);
                _productRepo.Save();*/
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully!";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Product? productFromDb = _db.Products.Find(id);  --only works with primary Key
            ///*Product? productFromDb = _db.Products.FirstOrDefault(c => c.Id == id);                  -- commented becoz using product repo*/ 
            //////Product? productFromDb = _productRepo.Get(u=> u.Id == id);                             -- commented bociz of unitofwork implementation
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            //Product? productFromDb = _db.Products.Where(c => c.Id == id).FirstOrDefault();  --used when some more filtering required in Where clause
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                /*_db.Products.Update(obj);
                _db.SaveChanges();*/
                /*_productRepo.Update(obj);
                _productRepo.Save();*/
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product Edited Successfully!";
                return RedirectToAction("Index", "Product");
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
            //Product? productFromDb = _db.Products.Find(id);  --only works with primary Key
            //Product? productFromDb = _db.Products.FirstOrDefault(c => c.Id == id);                    -- commented becoz using product repo
            ///Product? productFromDb = _productRepo.Get(u => u.Id == id);                               -- commented bociz of unitofwork implementation
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            //Product? productFromDb = _db.Products.Where(c => c.Id == id).FirstOrDefault();  --used when some more filtering required in Where clause
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            //Product? obj = _db.Products.FirstOrDefault(c => c.Id == id);                               -- commented becoz using product repo
            //Product? obj = _productRepo.Get(u => u.Id == id);                                           -- commented bociz of unitofwork implementation
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            /*_db.Products.Remove(obj);
            _db.SaveChanges();*/
            /*_productRepo.Remove(obj);
            _productRepo.Save();*/
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product Deleted Successfully!";
            return RedirectToAction("Index", "Product");
        }
    }
}
