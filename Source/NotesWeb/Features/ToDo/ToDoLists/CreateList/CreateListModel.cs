
namespace NotesWeb.Features.ToDo.ToDoLists.CreateList;

public class Request
{
    public required string Title { get; set; }
    [FromClaim]
    public int UserId { get; set; }
}

public class Response
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("You need to provide a title")
            .MinimumLength(3).WithMessage("Title is to short")
            .MaximumLength(30).WithMessage("Title is to long");
    }
}