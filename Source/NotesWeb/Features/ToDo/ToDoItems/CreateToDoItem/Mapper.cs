
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
}