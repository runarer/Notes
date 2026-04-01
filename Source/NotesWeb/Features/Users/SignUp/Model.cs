
using NotesWeb.Features.Users.SignUp.Persistence;

namespace NotesWeb.Features.Users.SignUp;

public class Request
{
    public string? FullName { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class Response
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("You need to provide a username")
            .MinimumLength(3).WithMessage("Username to short")
            .MaximumLength(16).WithMessage("Username to long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("You need to provide an email address")
            .EmailAddress().WithMessage("Email providet not valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("You need to privede a password")
            .MinimumLength(6).WithMessage("Password to short")
            .MaximumLength(30).WithMessage("Password is to long");
    }
}
