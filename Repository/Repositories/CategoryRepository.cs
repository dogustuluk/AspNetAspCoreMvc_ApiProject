using Core;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Category> GetSingleCategoryByIdWithProductsAsync(int categoryId)
        {
            /*single ile first'ün farkı
             * SingleOrDefaultAsync -> Eğer bu id'den koşulu karşılayan birden fazla var ise db'de geriye hata döner.
             * FirstOrDefaultAsync -> Eğer aynı id'den db'de birden fazla var ise karşısına ilk çıkanı alır.
             */
            return await _context.Categories.Include(x => x.Products).Where(x => x.Id == categoryId).SingleOrDefaultAsync();
        }
    }
}
