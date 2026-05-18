
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoLists.DeleteList;

public class DeleteListEndpoint(NoteBoardDBContext dbContext) : Endpoint<Request>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Delete("/todo/{ListId}");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("userId");
        Summary(s =>
        {
            s.Summary = "Delete a todo list";
            s.Description = "Deletes a todolist and all todoitems in it";
        });
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        await _dbContext.ToDoLists.Where(list => list.Id == request.ListId && list.UserId == request.UserId).ExecuteDeleteAsync(ct);
        await _dbContext.ToDoItems.Where(item => item.ParentListId == request.ListId).ExecuteDeleteAsync(ct);
        await Send.NoContentAsync(ct);
    }
}
