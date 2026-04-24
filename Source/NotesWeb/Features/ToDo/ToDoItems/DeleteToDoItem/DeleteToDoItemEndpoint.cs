using System;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.DeleteToDoItem;

public class DeleteToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : Endpoint<Request>
{

    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Delete("/todo/{listId}/{itemId}");
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

        // All is ok, delete item
        await _dbContext.ToDoItems.Where(item => item.Id == request.ItemId).ExecuteDeleteAsync(ct);

        todoList!.UpdatedAtUtc = _timeProvider.GetUtcNow();

        await Send.NoContentAsync(cancellation: ct);
    }
}