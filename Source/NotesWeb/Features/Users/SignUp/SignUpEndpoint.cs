
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data;
using NotesWeb.Entities;

namespace NotesWeb.Features.Users.SignUp;

public class SignUpEndpoint(TimeProvider timeProvider, NoteBoardDBContext dbContext, IPasswordHasher<User> passwordHasher) : Endpoint<Request, Response, SignUpMapper>
{

    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly NoteBoardDBContext _dbContext = dbContext;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public override void Configure()
    {
        Post("/users/signup");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        User user = Map.ToEntity(request);

        bool userExists = await _dbContext.Users.AnyAsync(user => user.Username == request.Username, ct);
        if (userExists)
            AddError(r => r.Username, "this username is taken!");

        bool emailTaken = await _dbContext.Users.AnyAsync(user => user.Email == request.Email, ct);
        if (emailTaken)
            AddError(r => r.Email, "this email is already used!");

        ThrowIfAnyErrors();

        user.HashedPassword = _passwordHasher.HashPassword(user, user.HashedPassword);
        user.CreatedAtUtc = _timeProvider.GetUtcNow();
        user.UpdatedAtUtc = user.CreatedAtUtc;

        await _dbContext.Users.AddAsync(user, ct);
        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(user);
        await Send.CreatedAtAsync("/users/", new { user.Id }, response, cancellation: ct);
    }
}
