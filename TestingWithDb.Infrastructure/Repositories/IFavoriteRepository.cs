namespace TestingWithDb.Infrastructure.Repositories;

public interface IFavoriteRepository
{
    public Task FavoriteProduct(int productId, int userId);
}