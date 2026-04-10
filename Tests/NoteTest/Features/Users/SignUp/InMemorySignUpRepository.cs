
using NotesWeb.Entities;
using NotesWeb.Features.Users.SignUp.Persistence;

namespace NoteTest.Features.Users.SignUp;

public class InMemorySignUpRepository : ISignUpRepository
{

    private readonly List<User> _users = [];
    public async Task<bool> EmailUsedAsync(string email, CancellationToken ct)
    {
        return _users.Any(u => u.Email == email);
    }

    public async Task<User> SaveUserAsync(User user, CancellationToken ct)
    {
        user.Id = _users.Count + 1;
        _users.Add(user);

        return user;
    }

    public async Task<bool> UserNameExistsAsync(string username, CancellationToken ct)
    {
        return _users.Any(u => u.Username == username);
    }
}
