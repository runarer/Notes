using System;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Extensions;

public static class DbExtensions
{

    // public static IServiceCollection AddMyDbContext(this IServiceCollection services)
    // {
    //     services.AddDbContext<NoteBoardDBContext>(
    //         options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    //     return services;
    // }
}
