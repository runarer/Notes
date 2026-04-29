
namespace NotesWeb.Features.ToDo.ToDoItems.MoveToDoItem;

public class Request : UserRequest
{
    // [FromClaim] public int UserId { get; set; }
    public Guid ItemId { get; set; }
    public Guid ListId { get; set; }
    // public Guid ToList { get; set; }
    [FromQuery] public Query? Query { get; set; }

}

public class Query
{
    public Guid ToList { get; set; }
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
        RuleFor(x => x.Query)
            .NotNull().WithMessage("You need to provide a destination");
    }
}