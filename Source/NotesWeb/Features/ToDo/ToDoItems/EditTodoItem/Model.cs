
namespace NotesWeb.Features.ToDo.ToDoItems.EditToDoItem;

public class Request : UserRequest
{
    public Guid ItemId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? Due { get; set; }

}

public class Response
{
    public Guid ItemId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? Due { get; set; }
    public bool Completed { get; set; }
    public Guid ParentListId { get; set; }
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

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Description is to long");
    }
}