
namespace NotesWeb.Features.Users.Login;

public class Request
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

}

public class Response
{
    public string Token { get; set; } = string.Empty;
}


/// <summary>
/// The validation of email and password is not critical as we do not create anything with this information,
/// but by checking we can save resources on server as the endpoint terminates early with wrong input.
/// </summary>
public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("You need to provide an email address")
            .EmailAddress().WithMessage("Email providet not valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("You need to provide a password")
            .MinimumLength(6).WithMessage("Password to short")
            .MaximumLength(30).WithMessage("Password is to long");
    }
}