
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoLists.GetLists;

public class GetListsEndpoint(NoteBoardDBContext dbContext, TimeProvider timeProvider) : Endpoint<Request, Response, Mapper>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;
    private readonly TimeProvider _timeProvider = timeProvider;

    public override void Configure()
    {
        Get("/todo");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
        Summary(s =>
        {
            s.Summary = "Get lists";
            s.Description = "Get a users lists, can use searchterm and time filtering.";
        });
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        if (request.FromUtc is not null && request.FromUtc > _timeProvider.GetUtcNow())
            ThrowError(r => r.FromUtc, "Date 'from Utc' must be in the past!");

        var listQuery = _dbContext.ToDoLists.Where(list => list.UserId == request.UserId);

        if (!string.IsNullOrWhiteSpace(request.Search))
            listQuery = listQuery.Where(list => list.Title.Contains(request.Search));

        if (request.FromUtc is not null)
            listQuery = listQuery.Where(list => list.UpdatedAtUtc >= request.FromUtc);

        if (request.ToUtc is not null)
            listQuery = listQuery.Where(list => list.UpdatedAtUtc <= request.ToUtc);

        var responseList = await listQuery.Select(list => Map.FromEntity(list)).ToArrayAsync(ct);

        await Send.OkAsync(new Response { Lists = responseList }, ct);
    }
}
