
namespace NotesWeb.Features.ToDo.ToDoItems.DeleteToDoItem;

public class Request
{
    [FromClaim] public int UserId { get; set; }
    public Guid ItemId { get; set; }
    public Guid ListId { get; set; }

}