
namespace NotesWeb.Features.ToDo.ToDoLists.GetList;

public class Request : UserRequest
{
    // [FromClaim] public int UserId { get; set; }
    public Guid ListId { get; set; }
}

public class Response
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}