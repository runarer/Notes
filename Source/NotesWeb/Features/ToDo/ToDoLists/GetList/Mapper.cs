
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoLists.GetList;

public class Mapper : Mapper<Request, Response, ToDoList>
{
    public override Response FromEntity(ToDoList e) => new()
    {
        ListId = e.ListId,
        Title = e.Title,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
    };
}