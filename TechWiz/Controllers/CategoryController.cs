using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechWizProject.Models;
using TechWizProject.Services;

namespace TechWizProject.Controllers
{
    [Route("category")]
    public class CategoryController : Controller
    {
        private readonly CategoryService categoryService;
        private readonly DatabaseContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CategoryController(CategoryService _categoryService, DatabaseContext context, IWebHostEnvironment _webHostEnvironment)
        {
            categoryService = _categoryService;
            _context= context;
            webHostEnvironment = _webHostEnvironment;
        }

        [HttpGet]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            return View(await categoryService.List());
        }

        [HttpGet]
        [Route("details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await categoryService.Details(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile photo,[Bind("Id,Name,Description,Icon,Status,CreatedAt,UpdatedAt")] Category category)
        {
            if (photo != null)
            {
                var fileName = System.Guid.NewGuid().ToString().Replace("-", "");
                var ext = photo.ContentType.Split(new char[] { '/' })[1];
                var path = Path.Combine(webHostEnvironment.WebRootPath, "Categories", fileName + "." + ext);
                using (var file = new FileStream(path, FileMode.Create))
                {
                    photo.CopyTo(file);
                }
                category.Icon = fileName + "." + ext;
            }
            else
            {
                category.Icon = "NOimage.jpg";
            }
            if (!ModelState.IsValid) return View(category);
            await categoryService.Create(category);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [Route("edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Icon,Status,CreatedAt,UpdatedAt")] Category category,IFormFile photo)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            if (photo != null)
            {
                var fileName = System.Guid.NewGuid().ToString().Replace("-", "");
                var ext = photo.ContentType.Split(new char[] { '/' })[1];
                var path = Path.Combine(webHostEnvironment.WebRootPath, "Categories", fileName + "." + ext);
                using (var filestream = new FileStream(path, FileMode.Create))
                {
                    photo.CopyTo(filestream);
                }
                category.Icon = fileName + "." + ext;

            }
            if (ModelState.IsValid)
            {
                try
                {
                    await categoryService.Edit(id, category);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Route("detele")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await categoryService.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            category.Status = 0;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }

        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
