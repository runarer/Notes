
// using System.Data.Common;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.DependencyInjection.Extensions;
// using NotesWeb.Features.Users.SignUp.Persistence;
using NotesWeb.Data;
using Testcontainers.PostgreSql;

namespace NoteTest;

public class App : AppFixture<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _postgreSqlContainer;

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


        // services.AddSingleton<ISignUpRepository, Features.Users.SignUp.InMemorySignUpRepository>();
        // var dbContextDescriptor = services.SingleOrDefault(
        //     d => d.ServiceType ==
        //         typeof(IDbContextOptionsConfiguration<NoteBoardDBContext>)) ?? throw new InvalidOperationException("dbContextDescriptor is null");

        // services.Remove(dbContextDescriptor);

        // var dbConnectionDescriptor = services.SingleOrDefault(
        //     d => d.ServiceType ==
        //         typeof(DbConnection));// ?? throw new InvalidCastException("dbConnectionDescriptor is null");

        // services.Remove(dbConnectionDescriptor);

        // // Create open SqliteConnection so EF won't automatically close it.
        // services.AddSingleton<DbConnection>(container =>
        // {
        //     var connection = new SqliteConnection("DataSource=:memory:");
        //     connection.Open();

        //     return connection;
        // });

        // services.AddDbContext<NoteBoardDBContext>((container, options) =>
        // {
        //     var connection = container.GetRequiredService<DbConnection>();
        //     options.UseSqlite(connection);
        // });
    }


    // protected override async ValueTask SetupAsync()
    // {
    //     using var scope = Services.CreateScope();
    //     await using var dbContext = scope.ServiceProvider.GetRequiredService<NoteBoardDBContext>();
    //     await dbContext.Database.MigrateAsync();
    // }
    // This runs once per assembly
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


    protected override async ValueTask TearDownAsync()
    {
        // do cleanups here
        // This may not be needed
        await _postgreSqlContainer!.StopAsync();
    }

}

