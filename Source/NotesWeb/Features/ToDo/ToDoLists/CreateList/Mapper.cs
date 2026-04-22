
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoLists.CreateList;

public class Mapper : Mapper<Request, Response, ToDoList>
{

    public override ToDoList ToEntity(Request r) => new()
    {
        Title = r.Title,
        UserId = r.UserId,
    };

    public override Response FromEntity(ToDoList e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
    };
}
