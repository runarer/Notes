
using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;
using NotesWeb.Data.Interfaces;
using NotesWeb.Entities;

namespace NotesWeb.Features.Users.SignUp;

public class SignUpEndpoint(TimeProvider timeProvider, IUserAccess dbContext, IPasswordHasher<User> passwordHasher) : Endpoint<Request, Response, SignUpMapper>
{

    private readonly TimeProvider _timeProvider = timeProvider;
    // private readonly NoteBoardDBContext _dbContext = dbContext;
    private readonly IUserAccess _dbContext = dbContext;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public override void Configure()
    {
        Post("/users/signup");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Sign up endpoint";
            s.Description = "Sign up a user for the service, fullName is optional";
        });
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        User user = Map.ToEntity(request);

        // bool userExists = await _dbContext.Users.AnyAsync(user => user.Username == request.Username, ct);
        bool userExists = await _dbContext.UsernameTakenAsync(user.Username);
        if (userExists)
            AddError(r => r.Username, "this username is taken!");

        // bool emailTaken = await _dbContext.Users.AnyAsync(user => user.Email == request.Email, ct);
        bool emailTaken = await _dbContext.EmailTakenAsync(user.Email);
        if (emailTaken)
            AddError(r => r.Email, "this email is already used!");

        ThrowIfAnyErrors();

        user.HashedPassword = _passwordHasher.HashPassword(user, user.HashedPassword);
        user.CreatedAtUtc = _timeProvider.GetUtcNow();
        user.UpdatedAtUtc = user.CreatedAtUtc;

        // await _dbContext.Users.AddAsync(user, ct);
        await _dbContext.CreateUserAsync(user);
        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(user);
        await Send.CreatedAtAsync("/users/", new { user.Id }, response, cancellation: ct);
    }
}
