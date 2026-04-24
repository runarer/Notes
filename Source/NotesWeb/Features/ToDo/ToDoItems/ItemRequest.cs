
namespace NotesWeb.Features.ToDo.ToDoItems;

public class ItemRequest
{
    [FromClaim] public int UserId { get; set; }
    public Guid ListId { get; set; }
}
