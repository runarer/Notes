
namespace NotesWeb.Features.ToDo;

// All request need userid so the extend this class
public class UserRequest
{
    [FromClaim] public Guid UserId { get; set; }
}
