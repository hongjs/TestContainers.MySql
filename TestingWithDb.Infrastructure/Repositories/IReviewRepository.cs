namespace TestingWithDb.Infrastructure.Repositories;

public interface IReviewRepository
{
    public Task ReviewProduct(int productId, int userId, string reviewContent);
}