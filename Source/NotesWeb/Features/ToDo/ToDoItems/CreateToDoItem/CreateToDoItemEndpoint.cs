
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem;

public class CreateToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : ItemBaseEndpoint<Request, Response, Mapper>(dbContext)
{

    private readonly TimeProvider _timeProvider = timeProvider;
    // private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Post("/todo/{listId}/{itemId}");
        PreProcessor<UserPreProcessor>();
        Roles("user");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        //Get list, check if it exists and that user owns it
        var todoList = await GetList(request.ListId, request, ct);
        if (todoList is null) return;

        // All is ok, create the item
        todoList!.UpdatedAtUtc = _timeProvider.GetUtcNow();

        var todoItem = Map.ToEntity(request);
        todoItem.CreatedAtUtc = _timeProvider.GetUtcNow();
        todoItem.UpdatedAtUtc = todoList.CreatedAtUtc;
        todoItem.ParentListId = todoList.Id;
        todoList.UpdatedAtUtc = todoItem.CreatedAtUtc;

        await Repo.ToDoItems.AddAsync(todoItem, ct);
        await Repo.SaveChangesAsync(ct);

        var response = Map.FromEntity(todoItem);
        await Send.CreatedAtAsync("/todo/{}/{}", new { ListId = todoList.Id, ItemId = todoItem.Id }, response, cancellation: ct);
    }
}
