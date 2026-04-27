
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.CompleteToDoItem;

public class CompleteToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : ItemBaseEndpoint<Request>(dbContext)
{

    private readonly TimeProvider _timeProvider = timeProvider;

    public override void Configure()
    {
        Patch("/todo/{listId}/{itemId}/complete");
        PreProcessor<UserPreProcessor>();
        Roles("User");
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

        // All is ok, set item to completed
        todoItem!.Completed = request.Completed ?? true;
        todoItem.UpdatedAtUtc = _timeProvider.GetUtcNow();
        todoList!.UpdatedAtUtc = todoItem.UpdatedAtUtc;

        await Repo.SaveChangesAsync(ct);
        await Send.OkAsync(cancellation: ct);


    }
}
