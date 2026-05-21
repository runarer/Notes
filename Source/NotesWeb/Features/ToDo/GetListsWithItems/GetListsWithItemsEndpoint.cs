
using NotesWeb.Data;
using NotesWeb.Features.ToDo.ToDoItems;

namespace NotesWeb.Features.ToDo.GetListsWithItems;

public class GetListsWithItemsEndpoint(NoteBoardDBContext dbContext, TimeProvider timeProvider) : ItemBaseEndpoint<Request, Response, ItemMapper<Request>>(dbContext)
{
    private readonly TimeProvider _timeProvider = timeProvider;
    public override void Configure()
    {
        Get("/todo/");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
        Summary(s =>
        {
            s.Summary = "Get lists of items";
            s.Description = "This returns a list of all the lists with items matching the query. If no query provided all items will be returned.";
        });
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        if (request.FromUtc is not null && request.FromUtc > _timeProvider.GetUtcNow())
            ThrowError(r => r.FromUtc, "Date 'from Utc' must be in the past!");

        //Get all items fullfilling the query
        var itemQuery = Repo.ToDoItems.Where(item => item.UserId == request.UserId);
        if (!string.IsNullOrWhiteSpace(request.Search))
            itemQuery = itemQuery.Where(item =>
                item.Title.Contains(request.Search) ||
                item.Description != null && item.Description.Contains(request.Search));

        if (request.Completed is not null)
            itemQuery = itemQuery.Where(item => item.Completed == request.Completed);

        if (request.FromUtc is not null)
            itemQuery = itemQuery.Where(item => item.UpdatedAtUtc >= request.FromUtc);

        if (request.ToUtc is not null)
            itemQuery = itemQuery.Where(item => item.UpdatedAtUtc <= request.ToUtc);

        if (request.DueFromUtc is not null)
            itemQuery = itemQuery.Where(item => item.Due >= request.DueFromUtc);

        if (request.DueToUtc is not null)
            itemQuery = itemQuery.Where(item => item.Due <= request.DueToUtc);

        // Group by Parentlist and create response items
        var responseList = itemQuery.GroupBy(item => item.ParentList)
            .Select(group => new ResponseList
            {
                Title = group.Key.Title,
                ListId = group.Key.Id,
                Items = group.Select(item => Map.FromEntity(item)).ToArray()
            }).ToArray();

        await Send.OkAsync(new Response { Lists = responseList }, ct);
    }
}
