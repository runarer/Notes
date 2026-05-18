
namespace NotesWeb.Entities;

public class User : CreatedResource
{
    // public int Id { get; set; }
    // public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string HashedPassword { get; set; }
    // public DateTimeOffset CreatedAtUtc { get; set; }
    // public DateTimeOffset UpdatedAtUtc { get; set; }
    public string? FullName { get; set; }
}
