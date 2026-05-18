
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem;

public class Mapper : ItemMapper<Request>
{

    public override ToDoItem ToEntity(Request r) => new()
    {
        Title = r.Title,
        Description = r.Description,
        Due = r.Due,
        UserId = r.UserId,
        ParentListId = r.ListId
    };
    // public override Response FromEntity(ToDoItem e) => new()
    // {
    //     ItemId = e.Id,
    //     Title = e.Title,
    //     Completed = e.Completed,
    //     ParentListId = e.ParentListId,
    //     CreatedAtUtc = e.CreatedAtUtc,
    //     UpdatedAtUtc = e.UpdatedAtUtc,
    //     Description = e.Description,
    //     Due = e.Due
    // };
}