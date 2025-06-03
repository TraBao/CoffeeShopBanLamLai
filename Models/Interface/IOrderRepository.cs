namespace CoffeeShop.Models.Interface
{
    public interface IOrderRepository
    {
        void PlaceOrder(Order order);
    }
}
