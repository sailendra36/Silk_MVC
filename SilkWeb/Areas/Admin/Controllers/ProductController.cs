using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Silk.DataAccess.Data;
using Silk.DataAccess.Repository;
using Silk.DataAccess.Repository.IRepository;
using Silk.Models;
using Silk.Models.ViewModels;
using Silk.Utility;

namespace SilkWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]               //so that by just having the url,no one should be able to access the controllers except if user is admin type
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _db;     -- commented bociz of repo implementation
        //private readonly IProductRepository _productRepo;     -- commented bociz of unitofwork implementation
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        //public ProductController(ApplicationDbContext db)
        //public ProductController(IProductRepository productRepo)
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            //_db = db;
            //_productRepo= productRepo
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            //List<Product> objProductList = _db.Products.ToList();
            //List<Product> objProductList = _productRepo.GetAll().ToList();
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties : "Category").ToList();
            return View(objProductList);
        }
        [HttpGet]
        //public IActionResult Create()
        public IActionResult Upsert(int? id)                           //update(id!=null/0) and insert(id==null/0) on single view page
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
            if(id==null || id == 0)
            {
                //Insert
                return View(productVM);
            }
            else
            {
                //Update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
            //return View(productVM);
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                //_db.Products.Add(obj);
                //_db.SaveChanges();
                /*_productRepo.Add(obj);
                _productRepo.Save();*/
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))          /*if update is AbandonedMutexException on image then need to delete old and add new*/
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath)) 
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productVM.Product.Id == 0) 
                {
                    //Add
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product Created Successfully!";

                }
                else
                {
                    //Update
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product Edited Successfully!";
                }
                _unitOfWork.Save();
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
        /*Edit not needed anymore due to upsert functionality*/
        //[HttpGet]
        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    //Product? productFromDb = _db.Products.Find(id);  --only works with primary Key
        //    ///*Product? productFromDb = _db.Products.FirstOrDefault(c => c.Id == id);                  -- commented becoz using product repo*/ 
        //    //////Product? productFromDb = _productRepo.Get(u=> u.Id == id);                             -- commented bociz of unitofwork implementation
        //    Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        //    //Product? productFromDb = _db.Products.Where(c => c.Id == id).FirstOrDefault();  --used when some more filtering required in Where clause
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        /*_db.Products.Update(obj);
        //        _db.SaveChanges();*/
        //        /*_productRepo.Update(obj);
        //        _productRepo.Save();*/
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product Edited Successfully!";
        //        return RedirectToAction("Index", "Product");
        //    }
        //    return View();
        //}

        //[HttpGet]         //removed since not nneded after Delete decvlared below in #region also delete delete view
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    //Product? productFromDb = _db.Products.Find(id);  --only works with primary Key
        //    //Product? productFromDb = _db.Products.FirstOrDefault(c => c.Id == id);                    -- commented becoz using product repo
        //    ///Product? productFromDb = _productRepo.Get(u => u.Id == id);                               -- commented bociz of unitofwork implementation
        //    Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        //    //Product? productFromDb = _db.Products.Where(c => c.Id == id).FirstOrDefault();  --used when some more filtering required in Where clause
        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    //Product? obj = _db.Products.FirstOrDefault(c => c.Id == id);                               -- commented becoz using product repo
        //    //Product? obj = _productRepo.Get(u => u.Id == id);                                           -- commented bociz of unitofwork implementation
        //    Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    /*_db.Products.Remove(obj);
        //    _db.SaveChanges();*/
        //    /*_productRepo.Remove(obj);
        //    _productRepo.Save();*/
        //    _unitOfWork.Product.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product Deleted Successfully!";
        //    return RedirectToAction("Index", "Product");
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var  productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false,message = "Error while Deleting."});
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successfully" });
        }
        #endregion
    }
}
