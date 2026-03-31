using NotesWeb.Entities;

namespace NotesWeb.Features.Users.SignUp;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public override void Configure()
    {
        Post("/author/signup");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        User user = Map.ToEntity(request);
    }
}
