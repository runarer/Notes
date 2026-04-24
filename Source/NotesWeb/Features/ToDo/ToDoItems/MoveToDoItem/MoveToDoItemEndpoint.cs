
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.MoveToDoItem;

public class RenameToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{

    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Patch("/todo/{listId}/move/{itemId}");
        PreProcessor<UserPreProcessor>();
        Roles("user");
        Claims("UserId");
    }


    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        //Get list, check if it exists and that user owns it
        var fromList = await _dbContext.ToDoItems.FindAsync([request.ListId], cancellationToken: ct);
        if (fromList is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        if (fromList.UserId != request.UserId)
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

        // Get toList, check if it exist and that user owns it
        var toList = await _dbContext.ToDoItems.FindAsync([request.ToList], cancellationToken: ct);
        if (toList is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        if (toList.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

        // All is Ok, make the move
        todoItem!.ParentListId = toList!.Id;
        todoItem.UpdatedAtUtc = _timeProvider.GetUtcNow();
        fromList!.UpdatedAtUtc = todoItem.UpdatedAtUtc;

        await _dbContext.SaveChangesAsync(ct);
        await Send.OkAsync(Map.FromEntity(todoItem), cancellation: ct);


    }
}