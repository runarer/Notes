
using Microsoft.AspNetCore.Identity.Data;
using NotesWeb.Entities;
using NotesWeb.Features.Users.Login.Persistence;

namespace NotesWeb.Features.Users.Login;

public class UserLoginEndpoint(IUserLoginRepository userLoginRepository) : Endpoint<LoginRequest>
{
    private readonly IUserLoginRepository _userLoginRepository = userLoginRepository;
    public override void Configure()
    {
        Post("/api/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest request, CancellationToken ct)
    {
        User? user = await _userLoginRepository.GetUserByEmail(request.Email, ct);

        if (user is null || user.HashedPassword != request.Password)// Hash the password
        {
            await Send.UnauthorizedAsync(ct);
        }
        else
        {
            //Create JWT
        }
    }
}

