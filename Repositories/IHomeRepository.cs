namespace ShinyEggs
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Product>> GetProducts(string sTerm = "", int categoryId = 0);
        Task<IEnumerable<Categories>> Category();
    }
}