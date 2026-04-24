
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.RenameToDoItem;

public class RenameToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : ItemBaseEndpoint<Request, Response, Mapper>(dbContext)
{

    private readonly TimeProvider _timeProvider = timeProvider;

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
        var todoList = await GetList(request.ListId, request, ct);
        if (todoList is null) return;

        // Get Item, check if it exist and that user owns it
        var todoItem = await GetItem(request.ItemId, request, ct);
        if (todoItem is null) return;

        // All is ok, do the update and send response
        todoItem.Title = request.Title;
        todoItem.UpdatedAtUtc = _timeProvider.GetUtcNow();
        todoList.UpdatedAtUtc = todoItem.UpdatedAtUtc;

        await Repo.SaveChangesAsync(ct);
        await Send.OkAsync(Map.FromEntity(todoItem), cancellation: ct);
    }
}
