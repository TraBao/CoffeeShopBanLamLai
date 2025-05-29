using CoffeeShop.Data;
using CoffeeShop.Models;
using CoffeeShop.Models.interfaces;

namespace CoffeeShop.Models.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly CoffeeshopDbContext _dbContext;

        public ProductRepository(CoffeeshopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _dbContext.Products;
        }

        public Product? GetProductDetail(int id)
        {
            return _dbContext.Products.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Product> GetTrendingProducts()
        {
            return _dbContext.Products.Where(p => p.IsTrendingProduct);
        }
    }
}
