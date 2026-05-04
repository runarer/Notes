
namespace NotesWeb.Features.ToDo.ToDoItems.MoveToDoItem;

public class Request : UserRequest
{
    public Guid ItemId { get; set; }
    [QueryParam] public Guid ToList { get; set; }

}


public class Response
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public bool Completed { get; set; }
    public Guid ParentListId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ToList)
            .NotEqual(default(Guid)).WithMessage("You need to provide a destination");
    }
}