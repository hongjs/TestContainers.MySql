using Microsoft.EntityFrameworkCore;
using TestingWithDb.Domain.AggregatesModel;

namespace TestingWithDb.Infrastructure.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly ProductDbContext _dbContext;

    public FavoriteRepository(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task FavoriteProduct(int productId, int userId)
    {
        var existingFavorite = await _dbContext.ProductFavorites.FirstOrDefaultAsync(x =>
            x.ProductId == productId &&
            x.UserId == userId
        );

        if (existingFavorite != null) return;

        await _dbContext.ProductFavorites.AddAsync(new ProductFavorite
        {
            ProductId = productId,
            UserId = userId
        });
        await _dbContext.SaveChangesAsync();
    }
}