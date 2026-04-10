
// using System.Data.Common;
// using Microsoft.Data.Sqlite;
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
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:latest")
                                                            .WithDatabase("noteboarddatabase")
                                                            .WithUsername("noteboard")
                                                            .WithPassword("passw0rd")
                                                            .Build();

    // protected override void ConfigureApp(IWebHostBuilder builder)
    // {
    //     builder.UseEnvironment("Development");
    // }
    protected override void ConfigureServices(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<NoteBoardDBContext>));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<NoteBoardDBContext>(options =>
        {
            options.UseNpgsql(_container.GetConnectionString());
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
    protected override async ValueTask PreSetupAsync()
    {
        await _container.StartAsync();
    }
    // protected override async ValueTask SetupAsync()
    // {
    //     // place one-time setup code here
    //     await _container.StartAsync();
    //     // return ValueTask.CompletedTask;
    // }

    protected override async ValueTask TearDownAsync()
    {
        // do cleanups here
        // return ValueTask.CompletedTask;
        await _container.StopAsync();
    }

}

