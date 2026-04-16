
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using NotesWeb.Entities;
using NotesWeb.Features.Users.Login.Persistence;

namespace NotesWeb.Features.Users.Login;

public class UserLoginEndpoint(IUserLoginRepository userLoginRepository, IPasswordHasher<User> passwordHasher) : Endpoint<Request, Response>
{
    private readonly IUserLoginRepository _userLoginRepository = userLoginRepository;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public override void Configure()
    {
        Post("/users/login");
        // PreProcessor<UserLoginPreProcessor>();
        // PostProcessor<UserLoginPostProcessor>();
        AllowAnonymous();

    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        User? user = await _userLoginRepository.GetUserByEmail(request.Email, ct);

        // User exsists?
        if (user is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        // Wrong password?
        var passwordResults = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, request.Password);
        if (passwordResults == PasswordVerificationResult.Failed)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        // User exsists and got right password, bu it might need Rehash. Rehash not supported.
        //Create JWT
        var jwtToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = "My big secret needs to be longer"; // get this secret from an external place
            o.ExpireAt = DateTime.UtcNow.AddMinutes(30);
            o.User.Roles.Add("User");
            o.User["UserId"] = user.Id.ToString();
        });

        await Send.OkAsync(new Response() { Token = jwtToken }, ct);
    }
}

