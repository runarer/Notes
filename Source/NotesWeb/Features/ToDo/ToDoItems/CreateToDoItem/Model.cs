
namespace NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem;

public class Request : UserRequest
{
    public Guid ListId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }
    public DateTimeOffset? Due { get; set; }

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