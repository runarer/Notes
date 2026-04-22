using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoLists.CreateList.Persistence;

public class CreateListRepository(NoteBoardDBContext dbContext) : ICreateListRepository
{
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public async Task<ToDoList> CreateList(ToDoList list, CancellationToken ct)
    {
        await _dbContext.ToDoLists.AddAsync(list, ct);
        await _dbContext.SaveChangesAsync(ct);
        return list;
    }

    public async Task<bool> UserExistsAsync(int userId, CancellationToken ct)
    {
        return await _dbContext.Users.AnyAsync(user => user.Id == userId, ct);
    }
}
