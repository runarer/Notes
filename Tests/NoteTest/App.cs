
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using NotesWeb.Data;
using Testcontainers.PostgreSql;

namespace NoteTest;

[DisableWafCache]
public class App : AppFixture<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _postgreSqlContainer;
    public FakeTimeProvider FakeTime = new();

    protected override void ConfigureServices(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<NoteBoardDBContext>));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<NoteBoardDBContext>(options =>
        {
            options.UseNpgsql(_postgreSqlContainer!.GetConnectionString());
        });

        var timeProvider = services.SingleOrDefault(s => s.ServiceType == typeof(TimeProvider));

        if (timeProvider is not null)
        {
            services.Remove(timeProvider);
        }

        services.AddSingleton<TimeProvider>(FakeTime);
    }


    protected override async ValueTask PreSetupAsync()
    {

        // See: https://gist.github.com/dj-nitehawk/04a78cea10f2239eb81c958c52ec84e0
        _postgreSqlContainer = new PostgreSqlBuilder("postgres:latest")
                                    .WithDatabase("noteboarddatabase")
                                    .WithUsername("noteboard")
                                    .WithPassword("passw0rd")
                                    .Build();

        await _postgreSqlContainer.StartAsync();

        var connectionString = _postgreSqlContainer.GetConnectionString();

        // this is optional, we're using environment variables for connection string
        // I can use this later.
        //Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", connectionString);

        // in pre PreSetupAsync Services is not initialized yet,
        // hence need to create temporary service provider to run migrations
        // running migrations in SetupAsync is not viable as setup runs for each fixture
        // so if tests are running in parallel, migrations run in parallel too and that leads to complex situations
        var serviceCollection = new ServiceCollection();
        // custom extension method, so that the DbContext options would match test and actual program
        // serviceCollection.AddMyDbContext();

        // I have replaced cutsom method for now with hardcoded. When using envirorment variable for connection string
        // a custom method will be better as trest will change with the SUT.

        serviceCollection.AddDbContext<NoteBoardDBContext>(options => options.UseNpgsql(connectionString));

        using var serviceProvider = serviceCollection.BuildServiceProvider();
        using var dbContext = serviceProvider.GetRequiredService<NoteBoardDBContext>();
        await dbContext.Database.MigrateAsync();
    }
}

