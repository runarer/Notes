
namespace NotesWeb.Features.ToDo.ToDoItems.CompleteToDoItem;

public class Request : UserRequest
{
    public Guid ItemId { get; set; }
    // public Guid ListId { get; set; }
    [QueryParam] public bool? Completed { get; set; }

}

