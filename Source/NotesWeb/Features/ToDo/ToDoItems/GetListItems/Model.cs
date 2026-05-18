
namespace NotesWeb.Features.ToDo.ToDoItems.GetListItems;

public class Request : UserRequest
{
    public Guid ListId { get; set; }
    public string? Search { get; set; }
    public bool? Completed { get; set; }
    public DateTimeOffset? FromUtc { get; set; }
    public DateTimeOffset? ToUtc { get; set; }
}

public class Response
{
    public ItemResponse[] List { get; set; } = [];
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.FromUtc)
            .LessThan(x => x.ToUtc)
            .When(x => x.ToUtc.HasValue && x.FromUtc.HasValue)
            .WithMessage("'{PropertyName}' must be after '{ComparisonProperty}'.");

    }
}
