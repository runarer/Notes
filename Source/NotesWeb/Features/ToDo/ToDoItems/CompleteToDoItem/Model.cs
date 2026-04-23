
namespace NotesWeb.Features.ToDo.ToDoItems.CompleteToDoItem;

public class Request
{
    [FromClaim] public int UserId { get; set; }
    public Guid ItemId { get; set; }
    public Guid ListId { get; set; }
    [FromQuery] public bool? Completed { get; set; }

}
