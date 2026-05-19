
using NotesWeb.Data;
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoItems;

// These classes contain code for checking if list or items exist and if they are owned by the user.
// Most endpoints for items require this code and can expand one of these classes.
public class ItemBaseEndpoint<TReq, TRes, TMapper>(NoteBoardDBContext dbContext) : Endpoint<TReq, TRes, TMapper>
    where TReq : notnull
    where TRes : notnull
    where TMapper : class, IMapper
{
    protected NoteBoardDBContext Repo = dbContext;

    protected async Task<ToDoList?> GetList(Guid listId, UserRequest request, CancellationToken ct)
    {
        //Get list, check if it exists and that user owns it
        var todoList = await Repo.ToDoLists.FindAsync([listId], cancellationToken: ct);
        if (todoList is null)
        {
            await Send.NotFoundAsync(ct);
            return null;
        }
        if (todoList.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return null;
        }
        return todoList;
    }

    protected async Task<ToDoItem?> GetItem(Guid itemId, UserRequest request, CancellationToken ct)
    {
        //Get item, check if it exists and that user owns it
        var todoItem = await Repo.ToDoItems.FindAsync([itemId], cancellationToken: ct);
        if (todoItem is null)
        {
            await Send.NotFoundAsync(ct);
            return null;
        }
        if (todoItem.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return null;
        }
        return todoItem;
    }
}

public class ItemBaseEndpoint<TReq>(NoteBoardDBContext dbContext) : Endpoint<TReq>
    where TReq : notnull
{
    protected NoteBoardDBContext Repo = dbContext;

    protected async Task<ToDoList?> GetList(Guid listId, UserRequest request, CancellationToken ct)
    {
        //Get list, check if it exists and that user owns it
        var todoList = await Repo.ToDoLists.FindAsync([listId], cancellationToken: ct);
        if (todoList is null)
        {
            await Send.NotFoundAsync(ct);
            return null;
        }
        if (todoList.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return null;
        }
        return todoList;
    }

    protected async Task<ToDoItem?> GetItem(Guid itemId, UserRequest request, CancellationToken ct)
    {
        //Get item, check if it exists and that user owns it
        var todoItem = await Repo.ToDoItems.FindAsync([itemId], cancellationToken: ct);
        if (todoItem is null)
        {
            await Send.NotFoundAsync(ct);
            return null;
        }
        if (todoItem.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return null;
        }
        return todoItem;
    }
}
