using System;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.GetItem;

public class GetItemEndpoint(NoteBoardDBContext dbContext) : ItemBaseEndpoint<Request, Response, Mapper>(dbContext)
{
    public override void Configure()
    {
        Get("/todo/{ListId}/{ItemId}");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {

        //Get list, check if it exists and that user owns it
        var todoList = await GetList(request.ListId, request, ct);
        if (todoList is null) return;

        var todoItem = await GetItem(request.ItemId, request, ct);
        if (todoItem is null) return;


        await Send.OkAsync(Map.FromEntity(todoItem), ct);
    }
}