
using System.Net;
using FastEndpoints;
using NotesWeb.Features.Users.SignUp;

namespace NoteTest.Features.Users.SignUp;

public class SignUpTests(App App) : TestBase<App>
{
    [Fact, Priority(1)]
    public async Task ValidUserSignup_ReturnsOkAndUserObject()
    {
        var validUser = new Request()
        {
            Username = "Bobb",
            FullName = "Bob Bobby Bobson",
            Email = "bob@example.com",
            Password = "BobIsAwesome"
        };

        var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, Response>(validUser);

        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        Assert.NotNull(res);
    }
}
