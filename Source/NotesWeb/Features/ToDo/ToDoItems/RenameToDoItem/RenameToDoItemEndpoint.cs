
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.RenameToDoItem;

public class RenameToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{

    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Patch("/todo/{listId}/{itemId}");
        PreProcessor<UserPreProcessor>();
        Roles("user");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        //Get list, check if it exists and that user owns it
        var todoList = await _dbContext.ToDoItems.FindAsync([request.ListId], cancellationToken: ct);
        if (todoList is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        if (todoList.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

        // Get Item, check if it exist and that user owns it
        var todoItem = await _dbContext.ToDoItems.FindAsync([request.ItemId], cancellationToken: ct);
        if (todoItem is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        if (todoItem.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

        // All is ok, do the update and send response
        todoItem.Title = request.Title;
        todoItem.UpdatedAtUtc = _timeProvider.GetUtcNow();
        todoList.UpdatedAtUtc = todoItem.UpdatedAtUtc;

        await _dbContext.SaveChangesAsync(ct);
        await Send.OkAsync(Map.FromEntity(todoItem), cancellation: ct);


    }
}
