using NotesWeb.Entities;

namespace NotesWeb.Features.Users.SignUp;

public class Endpoint(TimeProvider timeProvider) : Endpoint<Request, Response, Mapper>
{

    private readonly TimeProvider _timeProvider = timeProvider;

    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        User user = Map.ToEntity(request);
        user.CreatedAtUtc = _timeProvider.GetUtcNow();
        user.UpdatedAtUtc = user.CreatedAtUtc;

        // TODO: Check if email is already used
        // TODO: Check if username is taken

        // Hash password

        // TODO: write user to DB

        //TODO CreatedAtAsync<Endpoint where user is available>()
        var response = Map.FromEntity(user);
        await Send.CreatedAtAsync("/users/", new { user.Id }, response, cancellation: ct);
    }
}
