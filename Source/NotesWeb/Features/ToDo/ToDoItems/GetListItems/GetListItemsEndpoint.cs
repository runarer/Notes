
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.GetListItems;

public class GetListItemsEndpoint(NoteBoardDBContext dbContext) : ItemBaseEndpoint<Request, Response, Mapper>(dbContext)
{
    public override void Configure()
    {
        Get("/todo/{ListId}/items");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
        Summary(s =>
        {
            s.Summary = "Get list of items in a list";
            s.Description = "This returns a list of the item in a list, can filter on searchterm, completed and time";
        });
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {

        //Get list, check if it exists and that user owns it
        var todoList = await GetList(request.ListId, request, ct);
        if (todoList is null) return;

        var listQuery = Repo.ToDoItems.Where(list => list.ParentListId == request.ListId);

        if (!string.IsNullOrWhiteSpace(request.Search))
            listQuery = listQuery.Where(list => list.Title.Contains(request.Search));

        if (request.Completed is not null)
            listQuery = listQuery.Where(list => list.Completed == request.Completed);

        if (request.FromUtc is not null)
            listQuery = listQuery.Where(list => list.UpdatedAtUtc >= request.FromUtc);

        if (request.ToUtc is not null)
            listQuery = listQuery.Where(list => list.UpdatedAtUtc <= request.ToUtc);

        var responseList = await listQuery.Select(list => Map.FromEntity(list)).ToArrayAsync(ct);

        await Send.OkAsync(new Response { List = responseList }, ct);
    }
}