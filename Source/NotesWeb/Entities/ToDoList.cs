
namespace NotesWeb.Entities;

public class ToDoList : OwnedResource
{
    // public Guid Id { get; set; }
    public required string Title { get; set; }
    // public Guid ListId { get; set; }
    // public int UserId { get; set; }
    // public DateTimeOffset CreatedAtUtc { get; set; } //Todo: Remove
    // public DateTimeOffset UpdatedAtUtc { get; set; } //Todo: Remove
}
