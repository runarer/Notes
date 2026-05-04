
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.MoveToDoItem;

public class MoveToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : ItemBaseEndpoint<Request, Response, Mapper>(dbContext)
{

    private readonly TimeProvider _timeProvider = timeProvider;

    public override void Configure()
    {
        // Patch("/todo/{ListId}/{ItemId}/move");
        Patch("/todo/item/{ItemId}/move");
        Description(x => x.Accepts<Request>());
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
        var fromList = await GetList(todoItem.ParentListId, request, ct);
        if (fromList is null) return;

        // Get toList, check if it exist and that user owns it
        var toList = await GetList(request.ToList, request, ct);
        if (toList is null) return;

        // All is Ok, make the move
        var time = _timeProvider.GetUtcNow();
        fromList.UpdatedAtUtc = time;
        todoItem.ParentListId = toList.Id;
        todoItem.UpdatedAtUtc = time;
        toList.UpdatedAtUtc = time;
        // fromList.UpdatedAtUtc = todoItem.UpdatedAtUtc;

        await Repo.SaveChangesAsync(ct);
        await Send.OkAsync(Map.FromEntity(todoItem), cancellation: ct);


    }
}