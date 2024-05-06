using AutoFixture;
using TestingWithDb.Infrastructure;

namespace TestingWithDb.IntegrationTests.Setup;

[Collection(nameof(DatabaseTestCollection))]
public abstract class BaseTest : IAsyncLifetime
{
    private readonly IntegrationTestFactory _factory;
    private readonly Func<Task> _resetDatabase;
    protected readonly ProductDbContext DbContext;
    protected readonly Fixture Fixture;

    public BaseTest(IntegrationTestFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabase;
        DbContext = factory.DbContext;
        Fixture = new Fixture();
        Fixture.Customize(new NoCircularReferencesCustomization());
        Fixture.Customize(new IgnoreVirtualMembersCustomization());
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return _resetDatabase();
    }

    public async Task Insert<T>(T entity) where T : class
    {
        await DbContext.AddAsync(entity);
        await DbContext.SaveChangesAsync();
    }

    protected T GetRequiredService<T>() where T : notnull
    {
        return _factory.GetRequiredService<T>();
    }
}