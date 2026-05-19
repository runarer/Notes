using NotesWeb.Entities;

namespace NotesWeb.Data.Interfaces;

public interface IToDoItemAccess : IDataAccess
{
    Task<ToDoItem?> TryFindToDoItemByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddToDoItemAsync(ToDoList list, CancellationToken cancellationToken);
}