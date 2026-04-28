
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;
using NotesWeb.Entities;

namespace NotesWeb.Features.Users.Login;

public class UserLoginEndpoint(NoteBoardDBContext dbContext, IPasswordHasher<User> passwordHasher) : Endpoint<Request, Response>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public override void Configure()
    {
        Post("/users/login");
        AllowAnonymous();

    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        User? user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == request.Email, ct);

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
        var jwtSecret = Config["Auth:JwtSecretKey"];
        if (jwtSecret is null)
        {
            Logger.LogCritical("Auth:JwtSecretKey is null");
            ThrowError("Issue #auth:JwtSecretKey", 500);
        }
        var jwtToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = jwtSecret; // get this secret from an external place
            o.ExpireAt = DateTime.UtcNow.AddDays(30);
            o.User.Roles.Add("User");
            o.User.Claims.Add(("UserId", user.Id.ToString()));
        });

        await Send.OkAsync(new Response() { Token = jwtToken }, ct);

        // await Send.OkAsync(JwtBearer.CreateToken(jwtSecret, p => p["UserId"] = "001"));
    }
}

