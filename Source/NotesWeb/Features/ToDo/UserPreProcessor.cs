
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo;

// This PreProcessor check if a user exists
public class UserPreProcessor : IPreProcessor<UserRequest>
{
    public async Task PreProcessAsync(IPreProcessorContext<UserRequest> context, CancellationToken ct)
    {
        // Get user id from claims, this makes the preprocessor independent
        var userIdClaim = context.HttpContext.User.ClaimValue("UserId");
        int userId = default;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out userId))
            await context.HttpContext.Response.SendUnauthorizedAsync(ct);

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<NoteBoardDBContext>();
        //Check if User exists
        if (!await dbContext.Users.AnyAsync(user => user.Id == userId, ct))
            await context.HttpContext.Response.SendUnauthorizedAsync(ct);

    }
}