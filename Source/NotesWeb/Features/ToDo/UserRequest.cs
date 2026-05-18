
namespace NotesWeb.Features.ToDo;

public class UserRequest
{
    [FromClaim] public Guid UserId { get; set; }
}
