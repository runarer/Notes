
namespace NotesWeb.Features.ToDo.ToDoItems.GetListItems;

public class Request : UserRequest
{
    public Guid ListId { get; set; }
    public string? Search { get; set; }
    public bool? Completed { get; set; }
    public DateTimeOffset? FromUtc { get; set; }
    public DateTimeOffset? ToUtc { get; set; }
}

public class ResponseItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public bool Completed { get; set; }
    public Guid ParentListId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

public class Response
{
    public ResponseItem[] List { get; set; } = [];
}

public class Validator : Validator<Request>
{
    public Validator(TimeProvider timeProvider)
    {

        RuleFor(x => x.FromUtc)
            .LessThan(timeProvider.GetUtcNow())
            .When(x => x.FromUtc.HasValue) // Is this needed here 
            .WithMessage("Date '{PropertyName}' must be in the past!");
        RuleFor(x => x.FromUtc)
            .LessThan(x => x.ToUtc)
            .When(x => x.ToUtc.HasValue && x.FromUtc.HasValue)
            .WithMessage("'{PropertyName}' must be after '{ComparisonProperty}'.");

    }
}
