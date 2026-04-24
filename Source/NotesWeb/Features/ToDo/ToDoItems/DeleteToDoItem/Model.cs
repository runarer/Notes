
namespace NotesWeb.Features.ToDo.ToDoItems.DeleteToDoItem;

public class Request : UserRequest
{
    // [FromClaim] public int UserId { get; set; }
    public Guid ItemId { get; set; }
    public Guid ListId { get; set; }

}