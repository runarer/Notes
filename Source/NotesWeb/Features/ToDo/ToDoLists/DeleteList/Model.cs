
namespace NotesWeb.Features.ToDo.ToDoLists.DeleteList;

public class Request : UserRequest
{
    public Guid ListId { get; set; }
}