
namespace NotesWeb.Entities;

public class ToDoItem
{
    public Guid Id { get; set; }
    public required string Note { get; set; }
    public bool Completed { get; set; }
    public Guid ParentListId { get; set; }

    //Ef core should set this to a value so we can use null!
    public ToDoList ParentList { get; set; } = null!;

}
