

using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<NoteBoardDBContext>();

        dbContext.Database.Migrate();
    }
}
