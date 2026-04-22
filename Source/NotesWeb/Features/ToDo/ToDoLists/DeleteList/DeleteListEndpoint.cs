
using NotesWeb.Features.ToDo.ToDoLists.DeleteList.Persistence;

namespace NotesWeb.Features.ToDo.ToDoLists.DeleteList;

public class DeleteListEndpoint(IDeleteListRepository deleteListRepository) : Endpoint<Request>
{
    private readonly IDeleteListRepository _deleteListRepository = deleteListRepository;
    public override void Configure()
    {
        Delete("/todo/{ListId}");
        Roles("user");
        Claims("userId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        await _deleteListRepository.DeleteList(request.ListId, request.UserId, ct);
    }
}
