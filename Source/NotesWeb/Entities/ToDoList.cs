
namespace NotesWeb.Entities;

public class ToDoList
{
    public Guid Id { get; }
    public required string Title { get; set; }
    public ICollection<ToDoItem> ToDoItems { get; } = [];
    public int UserId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
