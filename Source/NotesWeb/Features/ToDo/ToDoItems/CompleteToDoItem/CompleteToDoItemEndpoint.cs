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
        PreProcessor<UserPreProcessor>();
        Roles("user");
        Claims("UserId");
    }

    // When a list exists but is not owned by the user, it doesn't exist for the user.
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

        // All is ok, set item to completed
        todoItem!.Completed = request.Completed ?? true;
        todoItem.UpdatedAtUtc = _timeProvider.GetUtcNow();
        todoList!.UpdatedAtUtc = todoItem.UpdatedAtUtc;

        await _dbContext.SaveChangesAsync(ct);
        await Send.OkAsync(cancellation: ct);


    }
}
