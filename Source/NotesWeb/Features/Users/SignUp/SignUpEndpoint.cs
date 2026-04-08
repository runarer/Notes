
using Microsoft.AspNetCore.Identity;
using NotesWeb.Entities;
using NotesWeb.Features.Users.SignUp.Persistence;

namespace NotesWeb.Features.Users.SignUp;

public class SignUpEndpoint(TimeProvider timeProvider, ISignUpRepository signUpRepository, IPasswordHasher<User> passwordHasher) : Endpoint<Request, Response, SignUpMapper>
{

    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ISignUpRepository _signUpRepository = signUpRepository;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public override void Configure()
    {
        Post("/api/users/signup");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        User user = Map.ToEntity(request);

        bool userExists = await _signUpRepository.UserNameExistsAsync(user.Username, ct);
        if (userExists)
            AddError(r => r.Username, "this username is taken!");

        bool emailTaken = await _signUpRepository.EmailUsedAsync(user.Email, ct);
        if (emailTaken)
            AddError(r => r.Email, "this email is already used!");

        ThrowIfAnyErrors();

        user.HashedPassword = _passwordHasher.HashPassword(user, user.HashedPassword);
        user.CreatedAtUtc = _timeProvider.GetUtcNow();
        user.UpdatedAtUtc = user.CreatedAtUtc;

        await _signUpRepository.SaveUserAsync(user, ct);

        //TODO CreatedAtAsync<Endpoint where user is available>()
        var response = Map.FromEntity(user);
        await Send.CreatedAtAsync("/users/", new { user.Id }, response, cancellation: ct);
    }
}
