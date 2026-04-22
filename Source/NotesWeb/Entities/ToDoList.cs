
namespace NotesWeb.Entities;

public class ToDoList
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public ICollection<ToDoItem> ToDoItems { get; } = [];
}
