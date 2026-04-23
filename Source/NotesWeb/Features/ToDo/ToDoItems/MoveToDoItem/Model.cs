
namespace NotesWeb.Features.ToDo.ToDoItems.MoveToDoItem;

public class Request
{
    [FromClaim] public int UserId { get; set; }
    public Guid ItemId { get; set; }
    public Guid ListId { get; set; }
    [FromQuery] public Guid ToList { get; set; }

}

public class Response
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public bool Completed { get; set; }
    public Guid ParentListId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
