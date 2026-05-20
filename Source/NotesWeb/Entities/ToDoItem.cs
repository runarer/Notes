
namespace NotesWeb.Entities;

public class ToDoItem : OwnedResource
{
    public required string Title { get; set; }
    public bool Completed { get; set; }
    public DateTimeOffset? Due { get; set; }
    public Guid ParentListId { get; set; }
    public ToDoList ParentList { get; set; } = null!;
    public string? Description { get; set; }
}
