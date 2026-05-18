namespace NotesWeb.Entities;

public class OwnedResource : CreatedResource
{
    public Guid UserId { get; set; }
}