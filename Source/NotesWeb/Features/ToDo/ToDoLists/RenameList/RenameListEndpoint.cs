
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoLists.RenameList;

public class RenameListEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;
    private readonly TimeProvider _timeProvider = timeProvider;
    public override void Configure()
    {
        Patch("/todo/{listId}");
        Roles("user");
        Claims("UserId");
    }

    public async override Task HandleAsync(Request request, CancellationToken ct)
    {
        bool userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId, ct);
        if (!userExists)
            AddError(r => r.UserId, "this user do not exist!");

        var list = await _dbContext.ToDoLists.FirstOrDefaultAsync(list => list.Id == request.ListId, ct);
        if (list is null)
            AddError(r => r.ListId, "this list was not found!");

        ThrowIfAnyErrors();

        list!.Title = request.Title;
        list.UpdatedAtUtc = _timeProvider.GetUtcNow();

        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(list);
        await Send.OkAsync(response, ct);
    }

}
