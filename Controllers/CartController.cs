using Google.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShinyEggs.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;
		private readonly ApplicationDbContext _context;
		public CartController(ICartRepository cartRepo, ApplicationDbContext context)
        {
            _cartRepo = cartRepo;
			_context = context; // Initialize ApplicationDbContext

		}
        public async Task<IActionResult> AddItem(int productId, int qty = 1, int redirect = 0 )
        {
            var cartCount = await _cartRepo.AddItem(productId, qty);

			// Create an audit record for adding items to cart
			var auditRecord = new AuditRecord
			{
				AuditActionType = "Add to Cart",
				DateTimeStamp = DateTime.Now,
				Username = User.Identity.Name,
				KeyProductFieldID = productId,
				OriginalValues = "", // No original values for adding items to cart
				NewValues = $"User Added {qty} items to Cart"
			};

			_context.AuditRecords.Add(auditRecord);
			await _context.SaveChangesAsync();


			if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int productId)
        {
            var cartCount = await _cartRepo.RemoveItem(productId);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }
        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetCartItemCount();
            return Ok(cartItem);
        }

        //checkout function
        public async Task<IActionResult> Checkout()
        {
            bool isCheckedOut = await _cartRepo.DoCheckout();
            if (!isCheckedOut)
                throw new Exception("Checkout failed");
            return RedirectToAction("Index", "Home");
        }

    }
}
