

using Microsoft.EntityFrameworkCore;
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
        Roles("user");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        ToDoList todoList = Map.ToEntity(request);

        bool userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId, ct);
        if (!userExists)
            AddError(r => r.UserId, "this user do not exist!");

        ThrowIfAnyErrors();

        todoList.CreatedAtUtc = _timeProvider.GetUtcNow();
        todoList.UpdatedAtUtc = todoList.CreatedAtUtc;

        await _dbContext.ToDoLists.AddAsync(todoList, ct);
        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(todoList);
        await Send.CreatedAtAsync("/todo/", new { todoList.Id }, response, cancellation: ct);
    }
}
