
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoLists.GetLists;

public class GetListsEndpoint(NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Get("/todo");
        Roles("user");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        bool userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId, ct);
        if (!userExists)
            AddError(r => r.UserId, "this user do not exist!");

        ThrowIfAnyErrors();

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
