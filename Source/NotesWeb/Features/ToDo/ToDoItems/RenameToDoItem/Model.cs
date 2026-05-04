using System;

namespace NotesWeb.Features.ToDo.ToDoItems.RenameToDoItem;

public class Request : UserRequest
{
    // [FromClaim] public int UserId { get; set; }
    public Guid ItemId { get; set; }
    // public Guid ListId { get; set; }
    public string Title { get; set; } = null!;

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
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("You need to provide a title")
            .MinimumLength(3).WithMessage("Title is to short")
            .MaximumLength(30).WithMessage("Title is to long");
    }
}