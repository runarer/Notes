
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoLists.RenameList;

public class RenameListEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;
    private readonly TimeProvider _timeProvider = timeProvider;
    public override void Configure()
    {
        Patch("/todo/{ListId}");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");

    }

    public async override Task HandleAsync(Request request, CancellationToken ct)
    {
        var list = await _dbContext.ToDoLists.FirstOrDefaultAsync(list => list.Id == request.ListId, ct);
        if (list is null)
        {
            await Send.NotFoundAsync(ct);
        }
        else
        {
            list!.Title = request.Title;
            list.UpdatedAtUtc = _timeProvider.GetUtcNow();

            await _dbContext.SaveChangesAsync(ct);

            var response = Map.FromEntity(list);
            await Send.OkAsync(response, ct);
        }
    }

}
