
namespace NotesWeb.Entities;

public class ToDoItem
{
    public Guid Id { get; }
    public required string Title { get; set; }
    public bool Completed { get; set; }
    public Guid ParentListId { get; set; }

    //Ef core should set this to a value so we can use null!
    public ToDoList ParentList { get; set; } = null!;

    public int UserId { get; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
