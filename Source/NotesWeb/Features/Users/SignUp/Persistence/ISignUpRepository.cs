
using NotesWeb.Entities;

namespace NotesWeb.Features.Users.SignUp.Persistence;

public interface ISignUpRepository
{
    Task<bool> EmailUsedAsync(string email, CancellationToken ct);
    Task<bool> UserNameExistsAsync(string username, CancellationToken ct);
    Task<User> SaveUserAsync(User user, CancellationToken ct);
}
