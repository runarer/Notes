
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoItems.EditToDoItem;

public class Mapper : Mapper<Request, Response, ToDoItem>
{
    public override Response FromEntity(ToDoItem e) => new()
    {
        ItemId = e.Id,
        Title = e.Title,
        Description = e.Description,
        Due = e.Due,
        Completed = e.Completed,
        ParentListId = e.ParentListId,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
    };
}