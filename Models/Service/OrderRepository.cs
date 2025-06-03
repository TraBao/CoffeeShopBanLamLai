using CoffeeShop.Data;
using CoffeeShop.Models.Interface;
using coffeeShop.Models.Interfaces;
using CoffeeShop.Models;

namespace CoffeeShop.Models.Services
{
    public class OrderRepository : IOrderRepository
    {
        private CoffeeshopDbContext dbContext;
        private IShoppingCartRepository shoppingCartRepository;

        public OrderRepository(CoffeeshopDbContext dbContext, IShoppingCartRepository shoppingCartRepository)
        {
            this.dbContext = dbContext;
            this.shoppingCartRepository = shoppingCartRepository;
        }

        public void PlaceOrder(Order order)
        {
            var shoppingCartItems = shoppingCartRepository.GetAllShoppingCartItems();
            order.OrderDetails = new List<OrderDetail>();
            foreach (var cartItem in shoppingCartItems)
            {
                var orderDetail = new OrderDetail
                {
                    Quantity = cartItem.Qty,
                    ProductId = cartItem.Product.Id,
                    Price = cartItem.Product.Price
                };
                order.OrderDetails.Add(orderDetail);
            }

            order.OrderPlaced = DateTime.Now;
            order.OrderTotal = shoppingCartRepository.GetShoppingCartTotal();
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
        }
    }
}
