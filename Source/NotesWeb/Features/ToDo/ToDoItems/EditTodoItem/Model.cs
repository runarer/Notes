
namespace NotesWeb.Features.ToDo.ToDoItems.EditToDoItem;

public class Request : UserRequest
{
    public Guid ItemId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset? Due { get; set; }

}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(3).WithMessage("Title is to short")
            .MaximumLength(30).WithMessage("Title is to long");

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Description is to long");
    }
}