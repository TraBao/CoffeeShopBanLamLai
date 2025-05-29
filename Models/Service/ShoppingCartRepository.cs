using coffeeShop.Models.Interfaces;
using CoffeeShop.Data;
using CoffeeShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Models.Services
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly CoffeeshopDbContext _dbContext;

        public ShoppingCartRepository(CoffeeshopDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ShoppingCartItem>? ShoppingCartItems { get; set; }
        public string? ShoppingCartId { get; set; }

        public static ShoppingCartRepository GetCart(IServiceProvider services)
        {
            var session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            var context = services.GetRequiredService<CoffeeshopDbContext>();

            string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
            session?.SetString("CartId", cartId);

            return new ShoppingCartRepository(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Product product)
        {
            if (ShoppingCartId == null) return;
            var shoppingCartItem = _dbContext.ShoppingCartItems
                .FirstOrDefault(s => s.ProductId == product.Id && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    ProductId = product.Id, 
                    Qty = 1
                };
                _dbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Qty++;
            }

            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }
        }

        public void ClearCart()
        {
            if (ShoppingCartId == null) return;

            var cartItems = _dbContext.ShoppingCartItems
                .Where(s => s.ShoppingCartId == ShoppingCartId)
                .ToList();

            _dbContext.ShoppingCartItems.RemoveRange(cartItems);
            _dbContext.SaveChanges();
        }
        public List<ShoppingCartItem> GetAllShoppingCartItems()
        {
            if (ShoppingCartId == null) return new List<ShoppingCartItem>();

            return ShoppingCartItems ??= _dbContext.ShoppingCartItems
                .Where(s => s.ShoppingCartId == ShoppingCartId)
                .Include(p => p.Product)
                .ToList();
        }

        public decimal GetShoppingCartTotal()
        {
            if (ShoppingCartId == null) return 0;

            return _dbContext.ShoppingCartItems
                .Where(s => s.ShoppingCartId == ShoppingCartId)
                .Select(s => s.Product.Price * s.Qty)
                .Sum();
        }

        public int RemoveFromCart(Product product)
        {
            if (ShoppingCartId == null) return 0;

            var shoppingCartItem = _dbContext.ShoppingCartItems
                .FirstOrDefault(s => s.Product.Id == product.Id && s.ShoppingCartId == ShoppingCartId);

            int quantity = 0;

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Qty > 1)
                {
                    shoppingCartItem.Qty--;
                    quantity = shoppingCartItem.Qty;
                }
                else
                {
                    _dbContext.ShoppingCartItems.Remove(shoppingCartItem);
                }

                _dbContext.SaveChanges();
            }

            return quantity;
        }
    }
}
