using System;
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoLists.RenameList.Persistence;

public interface IRenameListRepository
{
    Task<ToDoList> RenameList(Guid listId, string newName, CancellationToken ct);
}
