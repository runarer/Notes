
using NotesWeb.Features.ToDo.ToDoItems;

namespace NotesWeb.Features.ToDo.GetListsWithItems;

public class Request : UserRequest
{
    public string? Search { get; set; }
    public bool? Completed { get; set; }
    public DateTimeOffset? FromUtc { get; set; }
    public DateTimeOffset? ToUtc { get; set; }
    public DateTimeOffset? DueFromUtc { get; set; }
    public DateTimeOffset? DueToUtc { get; set; }

}

public class ResponseList
{
    public Guid ListId { get; set; }
    public string Title { get; set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }

    public ItemResponse[] Items { get; set; } = [];
}

public class Response
{
    public ResponseList[] Lists { get; set; } = [];
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.FromUtc)
            .LessThan(x => x.ToUtc)
            .When(x => x.ToUtc.HasValue && x.FromUtc.HasValue)
            .WithMessage("'{PropertyName}' must be after '{ComparisonProperty}'.");

        RuleFor(x => x.DueFromUtc)
            .LessThan(x => x.DueToUtc)
            .When(x => x.DueToUtc.HasValue && x.DueFromUtc.HasValue)
            .WithMessage("'{PropertyName}' must be after '{ComparisonProperty}'.");
    }
}
