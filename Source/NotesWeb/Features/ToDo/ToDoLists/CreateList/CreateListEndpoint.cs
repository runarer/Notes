

using NotesWeb.Entities;
using NotesWeb.Features.ToDo.ToDoLists.CreateList.Persistence;

namespace NotesWeb.Features.ToDo.ToDoLists.CreateList;

public class CreateListEndpoint(TimeProvider timeProvider, ICreateListRepository createListRepository) : Endpoint<Request, Response, Mapper>
{

    private readonly ICreateListRepository _createListRepository = createListRepository;
    private readonly TimeProvider _timeProvider = timeProvider;
    public override void Configure()
    {
        Post("/todo");
        Roles("user");
        Claims("UserId");
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        ToDoList todoList = Map.ToEntity(request);


        // this code was for better error messages, but if the user dont exists there are errors elsewere
        //         bool userExists = await _createListRepository.UserExistsAsync(todoList.UserId, ct);
        //         if (!userExists)
        //             AddError(r => r.UserId, "this user do not exist!");

        //         ThrowIfAnyErrors();

        todoList.CreatedAtUtc = _timeProvider.GetUtcNow();
        todoList.UpdatedAtUtc = todoList.CreatedAtUtc;

        await _createListRepository.CreateList(todoList, ct);

        var response = Map.FromEntity(todoList);
        await Send.CreatedAtAsync("/todo/", new { todoList.Id }, response, cancellation: ct);
    }
}
