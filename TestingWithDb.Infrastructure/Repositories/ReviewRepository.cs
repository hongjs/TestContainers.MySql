using Microsoft.EntityFrameworkCore;
using TestingWithDb.Domain.AggregatesModel;

namespace TestingWithDb.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ProductDbContext _dbContext;

    public ReviewRepository(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ReviewProduct(int productId, int userId, string reviewContent)
    {
        var test = await _dbContext.ProductReviews.FirstOrDefaultAsync(x =>
            x.ProductId == productId &&
            x.UserId == userId);
        var existingReview = await _dbContext.ProductReviews.FirstOrDefaultAsync(x =>
            x.ProductId == productId &&
            x.UserId == userId);

        if (existingReview != null)
            existingReview.ReviewContent = reviewContent;
        else
            await _dbContext.ProductReviews.AddAsync(new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                ReviewContent = reviewContent
            });

        await _dbContext.SaveChangesAsync();
    }
}