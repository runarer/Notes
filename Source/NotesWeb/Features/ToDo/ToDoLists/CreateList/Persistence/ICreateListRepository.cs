
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoLists.CreateList.Persistence;

public interface ICreateListRepository
{
    Task<ToDoList> CreateList(ToDoList list, CancellationToken ct);

    // Task<bool> UserExistsAsync(int userId, CancellationToken ct);
}
