
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoItems.GetList;

public class Mapper : Mapper<Request, ResponseItem, ToDoItem>
{
    public override ResponseItem FromEntity(ToDoItem e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Completed = e.Completed,
        ParentListId = e.ParentListId,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
    };
}
