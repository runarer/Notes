
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
        Roles("user");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {

        // bool userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId, ct);
        // if (!userExists)
        //     AddError(r => r.UserId, "this user does not exist!");

        var todoList = await _dbContext.ToDoLists.FindAsync([request.ListId], cancellationToken: ct);
        if (todoList is null)
            AddError(r => r.ListId, "this list does not exist!");

        ThrowIfAnyErrors();

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
