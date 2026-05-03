
namespace NotesWeb.Features.ToDo.ToDoItems;

// Todo: refactor ToDoItem endpoints to use this class, after tests are written.

public class ItemRequest : UserRequest
{
    public Guid ListId { get; set; }
}
