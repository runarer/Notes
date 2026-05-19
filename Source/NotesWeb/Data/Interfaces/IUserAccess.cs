using NotesWeb.Entities;

namespace NotesWeb.Data.Interfaces;

public interface IUserAccess : IDataAccess
{
    public Task<bool> UsernameTakenAsync(string username);

    public Task<bool> EmailTakenAsync(string email);

    public Task CreateUserAsync(User user);
    public Task<User?> TryFindByIdAsync(Guid id);
    public Task<User?> TryFindUserByEmailAsync(string email);
}