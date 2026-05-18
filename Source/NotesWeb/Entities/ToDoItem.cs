
namespace NotesWeb.Entities;

public class ToDoItem //: OwnedResource
{
    // public Guid Id { get; set; }
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public bool Completed { get; set; }
    public DateTimeOffset? Due { get; set; }
    public Guid ParentListId { get; set; }
    public string? Description { get; set; }
    public int UserId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
