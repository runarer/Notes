
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.DeleteToDoItem;

public class DeleteToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : ItemBaseEndpoint<Request>(dbContext)
{

    private readonly TimeProvider _timeProvider = timeProvider;

    public override void Configure()
    {
        // Delete("/todo/{ListId}/{ItemId}");
        Delete("/todo/item/{ItemId}");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {

        // Get Item, check if it exist and that user owns it
        var todoItem = await GetItem(request.ItemId, request, ct);
        if (todoItem is null) return;

        //Get list, check if it exists and that user owns it
        var todoList = await GetList(todoItem.ParentListId, request, ct);
        if (todoList is null) return;


        // All is ok, delete item
        todoList.UpdatedAtUtc = _timeProvider.GetUtcNow();
        await Repo.ToDoItems.Where(item => item.Id == request.ItemId).ExecuteDeleteAsync(ct);

        // todoList.UpdatedAtUtc = _timeProvider.GetUtcNow();
        await Repo.SaveChangesAsync(ct);

        await Send.NoContentAsync(cancellation: ct);
    }
}