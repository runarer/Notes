
namespace NotesWeb.Entities;

public class User : CreatedResource
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string HashedPassword { get; set; }
    public string? FullName { get; set; }
}
