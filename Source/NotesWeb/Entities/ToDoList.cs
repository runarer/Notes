
namespace NotesWeb.Entities;

public class ToDoList : OwnedResource
{
    public required string Title { get; set; }
    public ICollection<ToDoItem> Items { get; set; } = [];
}
