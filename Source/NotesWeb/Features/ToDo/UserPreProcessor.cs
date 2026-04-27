
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
}