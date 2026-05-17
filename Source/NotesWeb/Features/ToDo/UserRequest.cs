
namespace NotesWeb.Features.ToDo;

public class UserRequest
{
    [FromClaim] public int UserId { get; set; }
}
