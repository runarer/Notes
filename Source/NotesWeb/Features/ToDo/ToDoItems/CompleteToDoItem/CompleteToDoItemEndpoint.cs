using System;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.CompleteToDoItem;

public class CompleteToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : Endpoint<Request>
{

    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Patch("/todo/{listId}/{itemId}/complete");
        Roles("user");
        Claims("UserId");
    }

    // When a list exists but is not owned by the user, it doesn't exist for the user.
    public override async Task HandleAsync(Request request, CancellationToken ct)
    {

        // bool userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId, ct);
        // if (!userExists)
        //     AddError(r => r.UserId, "this user does not exist!");

        var todoList = await _dbContext.ToDoItems.FindAsync([request.ListId], cancellationToken: ct);
        if (todoList is null || todoList.UserId != request.UserId)
            AddError(r => r.ListId, "this list does not exist!");

        var todoItem = await _dbContext.ToDoItems.FindAsync([request.ItemId], cancellationToken: ct);
        if (todoItem is null || todoItem.UserId != request.UserId)
            AddError(r => r.ItemId, "this item does not exist!");

        ThrowIfAnyErrors();

        todoItem!.Completed = request.Completed ?? true;
        todoItem.UpdatedAtUtc = _timeProvider.GetUtcNow();
        todoList!.UpdatedAtUtc = todoItem.UpdatedAtUtc;

        await _dbContext.SaveChangesAsync(ct);
        await Send.OkAsync(cancellation: ct);


    }
}
