
namespace NotesWeb.Features.ToDo.ToDoLists.DeleteList.Persistence;

public interface IDeleteListRepository
{
    Task DeleteList(Guid listId, int userId, CancellationToken ct);
}
