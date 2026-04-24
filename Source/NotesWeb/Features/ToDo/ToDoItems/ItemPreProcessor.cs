
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems;


/// <summary>
/// This PreProcessor check if a user exists,
/// then if the list exist,
/// and last is the user owns that list
/// 
/// This class was not used since each endpoint need the list and therefore we will make two
/// calles to db, this is to expensive.
/// </summary>
/// <param name="dbContext"></param>
public class ItemPreProcessor(NoteBoardDBContext dbContext) : IPreProcessor<ItemRequest>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public async Task PreProcessAsync(IPreProcessorContext<ItemRequest> context, CancellationToken ct)
    {
        //Check if User exists
        if (context.Request is null || await _dbContext.Users.AnyAsync(user => user.Id == context.Request.UserId, ct))
        {
            await context.HttpContext.Response.SendUnauthorizedAsync(ct);
        }
        else
        {
            var list = await _dbContext.ToDoLists.FindAsync([context.Request.ListId], ct);
            if (list is null)
                await context.HttpContext.Response.SendNotFoundAsync(ct);
            else if (list.UserId != context.Request.UserId)
                await context.HttpContext.Response.SendForbiddenAsync(ct);
        }
    }
}
