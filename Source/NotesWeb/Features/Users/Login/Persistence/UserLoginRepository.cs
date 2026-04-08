
using NotesWeb.Entities;
using NotesWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace NotesWeb.Features.Users.Login.Persistence;

public class UserLoginRepository(NoteBoardDBContext dbContext) : IUserLoginRepository
{
    private readonly NoteBoardDBContext _dbContext = dbContext;
    public async Task<User?> GetUserByEmail(string email, CancellationToken ct)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email, ct);
    }
}