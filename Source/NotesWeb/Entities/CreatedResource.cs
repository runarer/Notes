namespace NotesWeb.Entities;

public class CreatedResource
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}