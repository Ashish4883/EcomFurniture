using EcomFurniture.Data;
using EcomFurniture.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcomFurniture.Controllers
{
    public class CategoryController : Controller
    {
        private readonly FurnitureContext _context;

        public CategoryController(FurnitureContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _context.Category;
            return View(objList);
        }

        //GET-CREATE
        public IActionResult Create()
        {            
            return View();
        }

        //POST-CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _context.Category.Add(obj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET-EDIT
        public IActionResult Edit(int? id)
        {
            if(id==null || id == 0)
            {
                return NotFound();
            }

            var obj = _context.Category.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST-EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _context.Category.Update(obj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET-DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var obj = _context.Category.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        //POST-DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _context.Category.Find(id);
            if(obj == null)
            {
                return NotFound();
            }
            
                _context.Category.Remove(obj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            
            
        }

    }
}
