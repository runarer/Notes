
namespace NotesWeb.Features.ToDo.ToDoItems.GetList;

public class Request
{
    [FromClaim] public int UserId { get; set; }
    [FromQuery] public Guid ListId { get; set; }
    public string? Search { get; set; }
    public bool? Completed { get; set; }
    public DateTimeOffset? FromUtc { get; set; }
    public DateTimeOffset? ToUtc { get; set; }
}

public class ResponseItem
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
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
    public Validator(/*TimeProvider timeProvider*/)
    {

        // RuleFor(x => x.ToUtc)
        //     .GreaterThanOrEqualTo(x => x.FromUtc)
        //     .When(x => x.ToUtc.HasValue && x.FromUtc.HasValue)
        //     .WithMessage("'{PropertyName}' must be after '{ComparisonProperty}'.");

        RuleFor(x => x.FromUtc)
            // .LessThan(timeProvider.GetUtcNow())
            .LessThan(DateTimeOffset.UtcNow)
            .When(x => x.FromUtc.HasValue) // Is this needed here 
            .WithMessage("Date 'from' must be in the past!")
            .LessThan(x => x.ToUtc)
            .When(x => x.ToUtc.HasValue && x.FromUtc.HasValue)
            .WithMessage("'{PropertyName}' must be after '{ComparisonProperty}'.");

        // RuleFor(x => x.FromUtc).NotNull().DependentRules(() =>
        // {
        //     RuleFor(x => x.FromUtc)
        // });

    }
}
