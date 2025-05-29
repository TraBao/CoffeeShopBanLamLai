using coffeeShop.Models.Interfaces;
using CoffeeShop.Models.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coffeeshop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IProductRepository _productRepository;  

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, IProductRepository productRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var items = _shoppingCartRepository.GetAllShoppingCartItems();
            ViewBag.TotalCart = _shoppingCartRepository.GetShoppingCartTotal();
            return View(items);
        }

        public RedirectToActionResult AddToShoppingCart(int pId)
        {
            var product = _productRepository.GetAllProducts().FirstOrDefault(p => p.Id == pId);
            if (product != null)
            {
                _shoppingCartRepository.AddToCart(product);
                int cartCount = _shoppingCartRepository.GetAllShoppingCartItems().Count(); 
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }
            return RedirectToAction("Index");
        }

        public RedirectToActionResult RemoveFromShoppingCart(int pId)
        {
            var product = _productRepository.GetAllProducts().FirstOrDefault(p => p.Id == pId);
            if (product != null)
            {
                _shoppingCartRepository.RemoveFromCart(product);
                int cartCount = _shoppingCartRepository.GetAllShoppingCartItems().Count();
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }
            return RedirectToAction("Index");
        }
    }
}