
namespace NotesWeb.Features.ToDo.ToDoLists.GetLists;

public class Request : UserRequest
{
    public string? Search { get; set; }
    public DateTimeOffset? FromUtc { get; set; }
    public DateTimeOffset? ToUtc { get; set; }
}

public class ResponseItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

public class Response
{
    public ResponseItem[] Lists { get; set; } = [];
}

public class Validator : Validator<Request>
{
    public Validator(TimeProvider timeProvider)
    {
        RuleFor(x => x.FromUtc)
            .LessThan(x => x.ToUtc)
            .When(x => x.ToUtc.HasValue && x.FromUtc.HasValue)
            .WithMessage("'{PropertyName}' must be after '{ComparisonProperty}'.");

    }
}
