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
    //[Authorize(Roles = SD.Role_Admin)]               //so that by just having the url,no one should be able to access the controllers except if user is admin type
    public class CompanyController : Controller
    {
        //private readonly ApplicationDbContext _db;     -- commented bociz of repo implementation
        //private readonly ICompanyRepository _companyRepo;     -- commented bociz of unitofwork implementation
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        //public CompanyController(ApplicationDbContext db)
        //public CompanyController(ICompanyRepository companyRepo)
        public CompanyController(IUnitOfWork unitOfWork)
        {
            //_db = db;
            //_companyRepo= companyRepo
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //List<Company> objCompanyList = _db.companies.ToList();
            //List<Company> objCompanyList = _companyRepo.GetAll().ToList();
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }
        [HttpGet]
        //public IActionResult Create()
        public IActionResult Upsert(int? id)                           //update(id!=null/0) and insert(id==null/0) on single view page
        {
            
            if(id==null || id == 0)
            {
                //Insert
                return View(new Company());
            }
            else
            {
                //Update
                Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }
            //return View(companyVM);
        }
        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {
                
                if (companyObj.Id == 0) 
                {
                    //Add
                    _unitOfWork.Company.Add(companyObj);
                    TempData["success"] = "Company Created Successfully!";

                }
                else
                {
                    //Update
                    _unitOfWork.Company.Update(companyObj);
                    TempData["success"] = "Company Edited Successfully!";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index", "Company");
            }
            else
            {
                return View(companyObj);
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
        //    //Company? companyFromDb = _db.companies.Find(id);  --only works with primary Key
        //    ///*Company? companyFromDb = _db.companies.FirstOrDefault(c => c.Id == id);                  -- commented becoz using company repo*/ 
        //    //////Company? companyFromDb = _companyRepo.Get(u=> u.Id == id);                             -- commented bociz of unitofwork implementation
        //    Company? companyFromDb = _unitOfWork.Company.Get(u => u.Id == id);
        //    //Company? companyFromDb = _db.companies.Where(c => c.Id == id).FirstOrDefault();  --used when some more filtering required in Where clause
        //    if (companyFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(companyFromDb);
        //}
        //[HttpPost]
        //public IActionResult Edit(Company obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        /*_db.companies.Update(obj);
        //        _db.SaveChanges();*/
        //        /*_companyRepo.Update(obj);
        //        _companyRepo.Save();*/
        //        _unitOfWork.Company.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Company Edited Successfully!";
        //        return RedirectToAction("Index", "Company");
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
        //    //Company? companyFromDb = _db.companies.Find(id);  --only works with primary Key
        //    //Company? companyFromDb = _db.companies.FirstOrDefault(c => c.Id == id);                    -- commented becoz using company repo
        //    ///Company? companyFromDb = _companyRepo.Get(u => u.Id == id);                               -- commented bociz of unitofwork implementation
        //    Company? companyFromDb = _unitOfWork.Company.Get(u => u.Id == id);
        //    //Company? companyFromDb = _db.companies.Where(c => c.Id == id).FirstOrDefault();  --used when some more filtering required in Where clause
        //    if (companyFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(companyFromDb);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    //Company? obj = _db.companies.FirstOrDefault(c => c.Id == id);                               -- commented becoz using company repo
        //    //Company? obj = _companyRepo.Get(u => u.Id == id);                                           -- commented bociz of unitofwork implementation
        //    Company? obj = _unitOfWork.Company.Get(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }
        //    /*_db.companies.Remove(obj);
        //    _db.SaveChanges();*/
        //    /*_companyRepo.Remove(obj);
        //    _companyRepo.Save();*/
        //    _unitOfWork.Company.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Company Deleted Successfully!";
        //    return RedirectToAction("Index", "Company");
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { objCompanyList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var  companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            
            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successfully" });
        }
        #endregion
    }
}
