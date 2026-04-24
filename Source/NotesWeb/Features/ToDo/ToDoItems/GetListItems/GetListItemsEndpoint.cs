using System;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.GetList;

public class GetListItemsEndpoint(NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Get("/todo/{ListId}/items");
        Roles("user");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        // bool userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId, ct);
        // if (!userExists)
        //     AddError(r => r.UserId, "this user do not exist!");

        var todoList = await _dbContext.ToDoLists.FindAsync([request.ListId], cancellationToken: ct);
        if (todoList is null || todoList.UserId != request.UserId)
            AddError(r => r.ListId, "this list does not exist!");

        ThrowIfAnyErrors();

        var listQuery = _dbContext.ToDoItems.Where(list => list.ParentListId == request.ListId);

        if (!string.IsNullOrWhiteSpace(request.Search))
            listQuery = listQuery.Where(list => list.Title.Contains(request.Search));

        if (request.FromUtc is not null)
            listQuery = listQuery.Where(list => list.UpdatedAtUtc >= request.FromUtc);

        if (request.ToUtc is not null)
            listQuery = listQuery.Where(list => list.UpdatedAtUtc <= request.ToUtc);

        var responseList = await listQuery.Select(list => Map.FromEntity(list)).ToArrayAsync(ct);

        await Send.OkAsync(new Response { List = responseList }, ct);
    }
}