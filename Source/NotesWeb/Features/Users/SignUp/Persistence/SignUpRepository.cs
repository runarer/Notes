
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;
using NotesWeb.Entities;

namespace NotesWeb.Features.Users.SignUp.Persistence;

public class SignUpRepository(NoteBoardDBContext dbContext) : ISignUpRepository
{
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public async Task<bool> EmailUsedAsync(string email, CancellationToken ct)
    {
        return await _dbContext.Users.AnyAsync(user => user.Email == email, ct);
    }
    public async Task<User> SaveUserAsync(User user, CancellationToken ct)
    {
        await _dbContext.Users.AddAsync(user, ct);
        await _dbContext.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> UserNameExistsAsync(string username, CancellationToken ct)
    {
        return await _dbContext.Users.AnyAsync(user => user.Username == username, ct);
    }
}
