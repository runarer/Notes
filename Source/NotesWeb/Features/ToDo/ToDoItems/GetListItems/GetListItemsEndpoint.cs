using System;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.GetListItems;

public class GetListItemsEndpoint(NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Get("/todo/{ListId}/items");
        PreProcessor<UserPreProcessor>();
        Roles("user");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {

        //Get list, check if it exists and that user owns it
        var todoList = await _dbContext.ToDoItems.FindAsync([request.ListId], cancellationToken: ct);
        if (todoList is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        if (todoList.UserId != request.UserId)
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

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