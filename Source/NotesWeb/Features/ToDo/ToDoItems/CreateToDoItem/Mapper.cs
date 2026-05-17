
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem;

public class Mapper : Mapper<Request, Response, ToDoItem>
{

    public override ToDoItem ToEntity(Request r) => new()
    {
        Title = r.Title,
        UserId = r.UserId,
        ParentListId = r.ListId
    };
    public override Response FromEntity(ToDoItem e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Completed = e.Completed,
        ParentListId = e.ParentListId,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
    };
}