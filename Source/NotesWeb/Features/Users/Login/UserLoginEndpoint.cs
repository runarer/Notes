
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using NotesWeb.Entities;
using NotesWeb.Features.Users.Login.Persistence;

namespace NotesWeb.Features.Users.Login;

public class UserLoginEndpoint(IUserLoginRepository userLoginRepository, IPasswordHasher<User> passwordHasher) : Endpoint<LoginRequest>
{
    private readonly IUserLoginRepository _userLoginRepository = userLoginRepository;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public override void Configure()
    {
        Post("/api/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest request, CancellationToken ct)
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
        //Creae JWT


    }
}

