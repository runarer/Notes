
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.EditToDoItem;

public class EditToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : ItemBaseEndpoint<Request, ItemResponse, ItemMapper<Request>>(dbContext)
{

    private readonly TimeProvider _timeProvider = timeProvider;

    public override void Configure()
    {
        Patch("/todo/{ListId}/{ItemId}");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
        Summary(s =>
        {
            s.Summary = "Edit an item";
            s.Description = "Edit the details of an existing item.";
        });
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {

        // Get Item, check if it exist and that user owns it
        var todoItem = await GetItem(request.ItemId, request, ct);
        if (todoItem is null) return;

        //Get list, check if it exists and that user owns it
        var todoList = await GetList(todoItem.ParentListId, request, ct);
        if (todoList is null) return;


        // All is ok, do the update and send response
        if (request.Title is not null)
        {
            todoItem.Title = request.Title;
        }
        if (request.Description is not null)
        {
            todoItem.Description = request.Description;
        }
        if (request.Due.HasValue)
        {
            // Check if Due date is in the past, execution ends here if so.
            if (request.Due.Value < _timeProvider.GetUtcNow())
                ThrowError(r => r.Due, "Due date cannot be in the past");
            todoItem.Due = request.Due.Value;
        }
        todoItem.UpdatedAtUtc = _timeProvider.GetUtcNow();
        todoList.UpdatedAtUtc = todoItem.UpdatedAtUtc;

        await Repo.SaveChangesAsync(ct);
        await Send.OkAsync(Map.FromEntity(todoItem), cancellation: ct);
    }
}
