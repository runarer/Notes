using NotesWeb.Entities;

namespace NotesWeb.Data.Interfaces;

public interface IToDoItemAccess : IDataAccess
{
    Task<ToDoItem?> TryFindToDoItemById(Guid id);
    Task AddToDoItemAsync(ToDoList list);
}