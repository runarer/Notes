
using NotesWeb.Data;
using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoLists.CreateList;

public class CreateListEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{

    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Post("/todo");
        PreProcessor<UserPreProcessor>();
        Roles("user");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        ToDoList todoList = Map.ToEntity(request);

        todoList.CreatedAtUtc = _timeProvider.GetUtcNow();
        todoList.UpdatedAtUtc = todoList.CreatedAtUtc;

        await _dbContext.ToDoLists.AddAsync(todoList, ct);
        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(todoList);
        await Send.CreatedAtAsync("/todo/", new { todoList.Id }, response, cancellation: ct);
    }
}
