
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem;

public class CreateToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{

    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly NoteBoardDBContext _dbContext = dbContext;

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

        // All is ok, create the item
        todoList!.UpdatedAtUtc = _timeProvider.GetUtcNow();

        var todoItem = Map.ToEntity(request);
        todoItem.CreatedAtUtc = _timeProvider.GetUtcNow();
        todoItem.UpdatedAtUtc = todoList.CreatedAtUtc;
        todoItem.ParentListId = todoList.Id;
        todoList.UpdatedAtUtc = todoItem.CreatedAtUtc;

        await _dbContext.ToDoItems.AddAsync(todoItem, ct);
        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(todoItem);
        await Send.CreatedAtAsync("/todo/{}/{}", new { ListId = todoList.Id, ItemId = todoItem.Id }, response, cancellation: ct);
    }
}
