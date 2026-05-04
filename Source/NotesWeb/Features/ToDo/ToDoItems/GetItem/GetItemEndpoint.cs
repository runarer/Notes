
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.GetItem;

public class GetItemEndpoint(NoteBoardDBContext dbContext) : ItemBaseEndpoint<Request, Response, Mapper>(dbContext)
{
    public override void Configure()
    {
        Get("/todo/item/{ItemId}");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var todoItem = await GetItem(request.ItemId, request, ct);
        if (todoItem is null) return;


        await Send.OkAsync(Map.FromEntity(todoItem), ct);
    }
}