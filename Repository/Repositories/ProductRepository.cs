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
    /*
     * sadece IProductRepository'i implemente etseydik GenericRepository içerisindeki metotları da implemente etmek zorunda olacaktık.
     */
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetProductsWithCategory()
        {
            return await _context.Products.Include(x => x.Category).ToListAsync();//include ile product'ların dahil olduğu kategorileri de aldık. Buna Eager loading denilmektedir;

        }
    }
}
