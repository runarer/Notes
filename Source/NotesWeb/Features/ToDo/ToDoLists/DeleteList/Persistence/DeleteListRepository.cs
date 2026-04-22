
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoLists.DeleteList.Persistence;

public class DeleteListRepository(NoteBoardDBContext dbContext) : IDeleteListRepository
{
    private readonly NoteBoardDBContext _dbContext = dbContext;
    public async Task DeleteList(Guid listId, int userId, CancellationToken ct)
    {
        await _dbContext.ToDoLists.Where(list => list.Id == listId && list.UserId == userId).ExecuteDeleteAsync(ct);
    }
}
