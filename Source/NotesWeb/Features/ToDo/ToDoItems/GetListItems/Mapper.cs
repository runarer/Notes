
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoItems.GetListItems;

public class Mapper : Mapper<Request, ResponseItem, ToDoItem>
{
    public override ResponseItem FromEntity(ToDoItem e) => new()
    {
        ItemId = e.ItemId,
        Title = e.Title,
        Completed = e.Completed,
        ParentListId = e.ParentListId,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
    };
}
