using NotesWeb.Entities;
using NotesWeb.Features.Users.SignUp.Persistence;

namespace NotesWeb.Features.Users.SignUp;

public class Endpoint(TimeProvider timeProvider, ISignUpRepository signUpRepository) : Endpoint<Request, Response, Mapper>
{

    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly ISignUpRepository _signUpRepository = signUpRepository;

    public override void Configure()
    {
        Post("/users");
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

        // TODO: write user to DB
        user.CreatedAtUtc = _timeProvider.GetUtcNow();
        user.UpdatedAtUtc = user.CreatedAtUtc;

        await _signUpRepository.SaveUserAsync(user, ct);

        //TODO CreatedAtAsync<Endpoint where user is available>()
        var response = Map.FromEntity(user);
        await Send.CreatedAtAsync("/users/", new { user.Id }, response, cancellation: ct);
    }
}
