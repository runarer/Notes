
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.GetListItems;

public class GetListItemsEndpoint(NoteBoardDBContext dbContext, TimeProvider timeProvider) : ItemBaseEndpoint<Request, Response, ItemMapper<Request>>(dbContext)
{
    private readonly TimeProvider _timeProvider = timeProvider;

    public override void Configure()
    {
        Get("/todo/{ListId}/items");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
        Summary(s =>
        {
            s.Summary = "Get list of items in a list";
            s.Description = "This returns a list of the item in a list, can filter on searchterm (title and description), completed, due time and update time.";
        });
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        if (request.FromUtc is not null && request.FromUtc > _timeProvider.GetUtcNow())
            ThrowError(r => r.FromUtc, "Date 'from Utc' must be in the past!");

        //Get list, check if it exists and that user owns it
        var todoList = await GetList(request.ListId, request, ct);
        if (todoList is null) return;

        var listQuery = Repo.ToDoItems.Where(list => list.ParentListId == request.ListId);

        if (!string.IsNullOrWhiteSpace(request.Search))
            listQuery = listQuery.Where(item =>
                item.Title.Contains(request.Search) ||
                item.Description != null && item.Description.Contains(request.Search));

        if (request.Completed is not null)
            listQuery = listQuery.Where(item => item.Completed == request.Completed);

        if (request.FromUtc is not null)
            listQuery = listQuery.Where(item => item.UpdatedAtUtc >= request.FromUtc);

        if (request.ToUtc is not null)
            listQuery = listQuery.Where(item => item.UpdatedAtUtc <= request.ToUtc);

        if (request.DueFromUtc is not null)
            listQuery = listQuery.Where(item => item.Due >= request.DueFromUtc);

        if (request.DueToUtc is not null)
            listQuery = listQuery.Where(item => item.Due <= request.DueToUtc);

        var responseList = await listQuery.Select(item => Map.FromEntity(item)).ToArrayAsync(ct);

        await Send.OkAsync(new Response { List = responseList }, ct);
    }
}