
namespace NotesWeb.Features.ToDo.ToDoItems.CompleteToDoItem;

public class Request : UserRequest
{
    // [FromClaim] public int UserId { get; set; }
    public Guid ItemId { get; set; }
    public Guid ListId { get; set; }
    [FromQuery] public Query? Query { get; set; }

}

public class Query
{
    public bool Completed { get; set; }
}
