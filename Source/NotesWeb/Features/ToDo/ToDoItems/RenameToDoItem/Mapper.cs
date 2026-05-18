
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoItems.RenameToDoItem;

public class Mapper : Mapper<Request, Response, ToDoItem>
{

    public override ToDoItem ToEntity(Request r) => new()
    {
        Title = r.Title,
    };
    public override Response FromEntity(ToDoItem e) => new()
    {
        ItemId = e.Id,
        Title = e.Title,
        Completed = e.Completed,
        ParentListId = e.ParentListId,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
    };
}