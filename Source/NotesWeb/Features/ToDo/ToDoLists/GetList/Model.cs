
namespace NotesWeb.Features.ToDo.ToDoLists.GetList;

public class Request
{
    [FromClaim] public int UserId { get; set; }
    public Guid ListId { get; set; }
}

public class Response
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}