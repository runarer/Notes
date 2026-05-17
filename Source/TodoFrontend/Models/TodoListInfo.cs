namespace TodoFrontend.Models;

// Make this into a record? More functional and server is source of truth!
// When changes occure, create new objects
public class TodoListInfo
{
    // public Guid Id { get; set; }
    public int Id { get; set; }
    public required string Title { get; set; }
}