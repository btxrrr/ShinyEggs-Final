using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShinyEggs.Models;

namespace ShinyEggs.Repositories
{
	public class UserOrderRepository : IUserOrderRepository
	{
		private readonly ApplicationDbContext _db;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<ApplicationUser> _userManager;

		public UserOrderRepository(ApplicationDbContext db,
			UserManager<ApplicationUser> userManager,
			IHttpContextAccessor httpContextAccessor) 
		{
			_db = db;
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
		}
		public async Task<IEnumerable<Order>> UserOrders()
		{
			var userId = GetUserId();
			if (string.IsNullOrEmpty(userId))
				throw new Exception("User is not logged in");
			var orders = await _db.Orders
                            .Include(x => x.OrderStatus)
                            .Include(x => x.OrderDetail)
                            .ThenInclude(x => x.Products)
                            .ThenInclude(x => x.Categories)
							.Where(a => a.CustomerId == userId)
							.ToListAsync();
			return orders;
		}

		private string GetUserId()
		{
			var principal = _httpContextAccessor.HttpContext.User;
			string userId = _userManager.GetUserId(principal);
			return userId;
		}

	}
}
