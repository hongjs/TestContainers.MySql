using Microsoft.AspNetCore.Mvc;
using TestingWithDb.Infrastructure.Repositories;

namespace TestingWithDb.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController
{
    private readonly IFavoriteRepository _favoriteRepo;

    public ProductController(IFavoriteRepository favoriteRepo)
    {
        _favoriteRepo = favoriteRepo;
    }

    [HttpPost]
    public async Task FavoriteBook([FromBody] FavoriteBookRequest request)
    {
        await _favoriteRepo.FavoriteProduct(request.ProductId, request.UserId);
    }
}

public record FavoriteBookRequest(int ProductId, int UserId);