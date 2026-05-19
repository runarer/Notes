using NotesWeb.Entities;

namespace NotesWeb.Data.Interfaces;

public interface IUserAccess : IDataAccess
{
    public Task<bool> UsernameTakenAsync(string username, CancellationToken cancellationToken);

    public Task<bool> EmailTakenAsync(string email, CancellationToken cancellationToken);

    public Task AddUserAsync(User user, CancellationToken cancellationToken);
    public Task<User?> TryFindUserByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<User?> TryFindUserByEmailAsync(string email, CancellationToken cancellationToken);
}