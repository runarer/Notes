
namespace TodoFrontend.Models;

// Make this into a record? More functional and server is source of truth!
// When changes occure, create new objects
public class TodoItem
{
    // public Guid Id { get; set; }
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool Completed { get; set; } = false;
    public DateTimeOffset Due { get; set; }
    public string Description { get; set; } = "";
    public int? ListId { get; set; } // Is this needed on frontend
    public int Order { get; set; }
}
