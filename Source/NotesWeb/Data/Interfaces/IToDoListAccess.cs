using NotesWeb.Entities;

namespace NotesWeb.Data.Interfaces;

public interface IToDoListAccess : IDataAccess
{
    Task<ToDoList?> TryFindToDoListByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddToDoListAsync(ToDoList list, CancellationToken cancellationToken);

}