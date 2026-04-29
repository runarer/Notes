
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo;

/// <summary>
/// This PreProcessor check if a user exists
/// </summary>
/// <param name="dbContext"></param>
public class UserPreProcessor/*(NoteBoardDBContext dbContext)*/ : IPreProcessor<UserRequest>
{
    //     private readonly NoteBoardDBContext _dbContext = dbContext;

    public async Task PreProcessAsync(IPreProcessorContext<UserRequest> context, CancellationToken ct)
    {
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<NoteBoardDBContext>();
        //Check if User exists
        if (context.Request is null || !await dbContext.Users.AnyAsync(user => user.Id == context.Request.UserId, ct))
            await context.HttpContext.Response.SendUnauthorizedAsync(ct);

    }

    //     public async Task PreProcessAsync(IPreProcessorContext<UserRequest> context, CancellationToken ct)
    // {
    //     // Get user id from claims, this makes the preprocessor independent
    //     var userIdClaim = context.HttpContext.User.ClaimValue("UserId");
    //     int userId = 0;
    //     if (userIdClaim is null || !int.TryParse(userIdClaim, out userId))
    //         await context.HttpContext.Response.SendUnauthorizedAsync(ct);

    //     var dbContext = context.HttpContext.RequestServices.GetRequiredService<NoteBoardDBContext>();
    //     //Check if User exists
    //     if (!await dbContext.Users.AnyAsync(user => user.Id == userId, ct))
    //         await context.HttpContext.Response.SendUnauthorizedAsync(ct);

    // }
}