using NotesWeb.Entities;

namespace NotesWeb.Data.Interfaces;

public interface IToDoListAccess : IDataAccess
{
    Task<ToDoList?> TryFindToDoListById(Guid id);

}