using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Models;

namespace TechWizProject.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly DatabaseContext db;
        public CategoriesService(DatabaseContext _db)
        {
            db = _db;
        }
        public async Task<List<Category>> LoadCategories()
        {
            return await db.Categories.ToListAsync();
        }

        public async Task<Category> FindCategory(int idCategory)
        {
            return await db.Categories.FindAsync(idCategory);
        }
    }
}
