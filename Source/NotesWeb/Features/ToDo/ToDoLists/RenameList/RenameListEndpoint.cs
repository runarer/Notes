
using NotesWeb.Data.Interfaces;

namespace NotesWeb.Features.ToDo.ToDoLists.RenameList;

public class RenameListEndpoint(TimeProvider timeProvider, IToDoListAccess listRepository) : Endpoint<Request, Response, Mapper>
{
    private readonly IToDoListAccess _listRepository = listRepository;
    private readonly TimeProvider _timeProvider = timeProvider;
    public override void Configure()
    {
        Patch("/todo/{ListId}");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
        Summary(s =>
        {
            s.Summary = "Rename a list";
            s.Description = "Changes the title of a list";
        });

    }

    public async override Task HandleAsync(Request request, CancellationToken ct)
    {
        // Check if list exist
        var list = await _listRepository.TryFindToDoListByIdAsync(request.ListId, ct);

        if (list is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        // Check if list is owned
        if (list.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

        list!.Title = request.Title;
        list.UpdatedAtUtc = _timeProvider.GetUtcNow();

        await _listRepository.SaveChangesAsync(ct);

        var response = Map.FromEntity(list);
        await Send.OkAsync(response, ct);
    }

}
