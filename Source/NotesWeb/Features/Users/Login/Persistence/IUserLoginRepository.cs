
using NotesWeb.Entities;

namespace NotesWeb.Features.Users.Login.Persistence;

public interface IUserLoginRepository
{
    Task<User?> GetUserByEmail(string email, CancellationToken ct);
}
