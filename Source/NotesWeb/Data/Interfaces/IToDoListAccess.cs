using NotesWeb.Entities;

namespace NotesWeb.Data.Interfaces;

public interface IToDoListAccess : IDataAccess
{
    Task<ToDoList?> TryFindToDoListByIdAsync(Guid id);
    Task AddToDoListAsync(ToDoList list);

}