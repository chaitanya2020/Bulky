using Bulky.Models;
using Bulky.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork db,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            
            return View(objProductList);
        }
        public IActionResult Upsert(int ? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            
            };
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            
            if (ModelState.IsValid)
            {
                string wwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string filename=Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string productPath=Path.Combine(wwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldPath=Path.Combine(wwRootPath,productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    using(var fileStream=new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"images\product\" + filename;
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["Success"] = "Product Created Successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["Success"] = "Product updated Successfully";
                }
               
                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }


        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productfromDB = _unitOfWork.Product.Get(u => u.Id == id);
            if (productfromDB == null)
            {
                return NotFound();
            }
            return View(productfromDB);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        //public IActionResult Delete(int id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? productfromDB = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (productfromDB == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(productfromDB);
        //}
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Product productfromDB = _unitOfWork.Product.Get(u => u.Id == id);
        //    if (productfromDB == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Remove(productfromDB);
        //    _unitOfWork.Save();
        //    TempData["Success"] = "Category Deleted Successfully";
        //    return RedirectToAction("Index");
        //}

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data=objProductList});
        }

        [HttpDelete]
        public IActionResult Delete(int ?id)
        {
            var productTobeDeleted= _unitOfWork.Product.Get(u=>u.Id == id);
            if (productTobeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, 
                productTobeDeleted.ImageUrl.TrimStart('\\'));
            
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _unitOfWork.Product.Remove(productTobeDeleted);
            _unitOfWork.Save();

            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            TempData["Success"] = "Category Deleted Successfully";
            return Json(new { success = true,message="Deleted Successfully" });
        }

        #endregion
    }
}
