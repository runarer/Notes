
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoLists.GetLists;

public class Mapper : Mapper<Request, ResponseItem, ToDoList>
{
    public override ResponseItem FromEntity(ToDoList e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
    };
}
