
using NotesWeb.Entities;

namespace NotesWeb.Features.Users.SignUp;

public class Mapper : Mapper<Request, Response, User>
{
    public override User ToEntity(Request r) => new()
    {
        FullName = r.FullName,
        Username = r.Username,
        Email = r.Email.ToLowerInvariant(),
        HashedPassword = r.Password, //TODO Add hashing of password
    };

    public override Response FromEntity(User e) => new()
    {
        Id = e.Id,
        FullName = e.FullName,
        Username = e.Username,
        Email = e.Email,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
    };
}
