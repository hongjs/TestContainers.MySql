using System.Data.Common;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Testcontainers.MySql;
using TestingWithDb.Api;
using TestingWithDb.Infrastructure;

namespace TestingWithDb.IntegrationTests.Setup;

[CollectionDefinition(nameof(DatabaseTestCollection))]
public class DatabaseTestCollection : ICollectionFixture<IntegrationTestFactory>
{
}

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _container;
    private DbConnection _connection = null!;
    private Respawner _respawner = null!;
    private IServiceProvider? _serviceProvider;

    public IntegrationTestFactory()
    {
        _container = new MySqlBuilder()
            .WithDatabase("db")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithExposedPort(3306)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(3306))
            .WithCleanUp(true)
            .Build();
    }

    public ProductDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        try
        {
            await _container.StartAsync();
            await _container.ExecAsync([
                "mysql", "-p", "mysql", "-e", "ALTER DATABASE db CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;"
            ]);

            _serviceProvider = Services.CreateScope().ServiceProvider;
            DbContext = _serviceProvider.GetRequiredService<ProductDbContext>();
            _connection = DbContext.Database.GetDbConnection();
            await _connection.OpenAsync();

            _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.MySql,
                SchemasToInclude = ["db"]
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public new async Task DisposeAsync()
    {
        await _connection.CloseAsync();
        await _container.DisposeAsync();
    }

    public async Task ResetDatabase()
    {
        await _respawner.ResetAsync(_connection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connString = _container.GetConnectionString();

        builder.ConfigureTestServices(services =>
        {
            services.RemoveDbContext<ProductDbContext>();
            services.AddDbContext<ProductDbContext>(options =>
            {
                options.UseMySql(connString, ServerVersion.AutoDetect(connString));
            });
            services.EnsureDbCreated<ProductDbContext>();
        });
    }

    internal T GetRequiredService<T>() where T : notnull
    {
        return (_serviceProvider ?? throw new InvalidOperationException()).GetRequiredService<T>();
    }
}