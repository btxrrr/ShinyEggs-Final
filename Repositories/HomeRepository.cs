using Microsoft.EntityFrameworkCore;
using Humanizer.Localisation;
using static System.Reflection.Metadata.BlobBuilder;

namespace ShinyEggs.Repositories

{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;
        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Categories>> Category()
        {
            return await _db.Category.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts(string sTerm="", int categoryId = 0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Product> products = await (from product in _db.Products
                           join category in _db.Category
                           on product.CategoryId equals category.Id
                            where string.IsNullOrWhiteSpace(sTerm) || product.ProductName.ToLower().Contains(sTerm) 
                            select new Product
                           {
                               Id=product.Id,
                               ProductName=product.ProductName,
                               Price=product.Price,
                               Image=product.Image,
                               CategoryId = product.CategoryId,
                               Brand = product.Brand,
                               BriefDescription = product.BriefDescription
                           }).ToListAsync();
            if (categoryId > 0)
            {
                products = products.Where(a => a.CategoryId == categoryId).ToList();
            }
            return products;
        }
    }
}
