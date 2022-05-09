using EcomFurniture.Data;
using EcomFurniture.Models;
using EcomFurniture.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EcomFurniture.Controllers
{
    public class ProductController : Controller
    {
        private readonly FurnitureContext _context;
        public readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(FurnitureContext context , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _context.Product.Include(u=>u.Category);  //Eager loading
            //foreach (var obj in objList)
            //{
            //    obj.Category = _context.Category.FirstOrDefault(u=>u.Id == obj.CategoryId);
            //};
            return View(objList);
        }

        //GET-UPSERT
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryDropDown = _context.Category.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value= i.Id.ToString()
            //});

            //ViewBag.CategoryDropDown = CategoryDropDown;
            //ViewData["CategoryDropDown"] = CategoryDropDown;

            // Product product = new Product();

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _context.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null)
            {
                //this is for create
                return View(productVM);
            }
            else
            {
                productVM.Product = _context.Product.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
           
        }

        //POST-Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //Creating
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString(); //Generating random file name
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _context.Product.Add(productVM.Product);

                }
                else
                {
                    //Updating

                    var objFromDb = _context.Product.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);
                    

                    if(files.Count > 0)  //This means a new file has ben uploaded for an existing product
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString(); //Generating random file name
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload,objFromDb.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;

                    }
                    else //If file was not updated something else was updated 
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _context.Product.Update(productVM.Product);

                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            productVM.CategorySelectList = _context.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return View(productVM);
        }        

        //GET-DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product product = _context.Product.Include(u => u.Category ).FirstOrDefault(u => u.Id== id);  //Eager Loading
           // product.Category = _context.Category.Find(product.CategoryId);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //POST-DELETE
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _context.Product.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;
            
            

            var oldFile = Path.Combine(upload, obj.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _context.Product.Remove(obj);
            _context.SaveChanges();
            return RedirectToAction("Index");


        }

    }
}
