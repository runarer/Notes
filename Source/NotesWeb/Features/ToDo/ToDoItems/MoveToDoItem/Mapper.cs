using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoItems.MoveToDoItem;

public class Mapper : Mapper<Request, Response, ToDoItem>
{
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
