
using NotesWeb.Data;
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoItems;

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
        else if (todoList.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return null;
        }
        return todoList;
    }

    protected async Task<ToDoItem?> GetItem(Guid listId, UserRequest request, CancellationToken ct)
    {
        //Get item, check if it exists and that user owns it
        var todoItem = await Repo.ToDoItems.FindAsync([listId], cancellationToken: ct);
        if (todoItem is null)
        {
            await Send.NotFoundAsync(ct);
            return null;
        }
        else if (todoItem.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return null;
        }
        return todoItem;
    }
}
