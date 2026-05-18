namespace NotesWeb.Entities;

// For now I just use an int as an id, but we might want to switch to Guid later.
// And add an int as a primary key for the database, and use the Guid as an external id. 
// This way we can avoid exposing internal ids to the clients, and also make it easier 
// to switch to a different database in the future if we want to.
public class CreatedResource
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}