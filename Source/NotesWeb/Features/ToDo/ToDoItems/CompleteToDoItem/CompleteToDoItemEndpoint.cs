
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoItems.CompleteToDoItem;

public class CompleteToDoItemEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : ItemBaseEndpoint<Request>(dbContext)
{

    private readonly TimeProvider _timeProvider = timeProvider;

    public override void Configure()
    {
        Patch("/todo/item/{ItemId}/complete");
        Description(x => x.Accepts<Request>());
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
        Summary(s =>
    {
        s.Summary = "Completes an item";
        s.Description = "Completes an item, theres a optional query parameter for setting the value to true or false";
    });
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        // Get Item, check if it exist and that user owns it
        var todoItem = await GetItem(request.ItemId, request, ct);
        if (todoItem is null) return;

        //Get list, check if it exists and that user owns it
        var todoList = await GetList(todoItem.ParentListId, request, ct);
        if (todoList is null) return;


        // All is ok, set item to completed
        todoItem.Completed = request.Completed ?? true;
        todoItem.UpdatedAtUtc = _timeProvider.GetUtcNow();
        todoList.UpdatedAtUtc = todoItem.UpdatedAtUtc;

        await Repo.SaveChangesAsync(ct);
        await Send.OkAsync(cancellation: ct);


    }
}
