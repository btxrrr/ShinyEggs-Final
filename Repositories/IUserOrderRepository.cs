namespace ShinyEggs.Repositories
{
	public interface IUserOrderRepository
	{
		Task<IEnumerable<Order>> UserOrders();
	}
}