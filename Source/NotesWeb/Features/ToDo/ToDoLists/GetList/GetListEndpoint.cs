
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoLists.GetList;

public class GetListEndpoint(NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Get("/todo/{listId}");
        Roles("user");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        // bool userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId, ct);
        // if (!userExists)
        //     AddError(r => r.UserId, "this user does not exist!");

        var list = await _dbContext.ToDoLists.FindAsync([request.ListId], ct);
        if (list is null || list.UserId != request.UserId)
            AddError(r => r.ListId, "this list does not exist!");

        ThrowIfAnyErrors();

        await Send.OkAsync(Map.FromEntity(list!), ct);
    }
}