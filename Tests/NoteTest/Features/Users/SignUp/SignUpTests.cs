
using System.Net;
using NotesWeb.Entities;
using NotesWeb.Features.Users.SignUp;

namespace NoteTest.Features.Users.SignUp;

public class SignUpTests(App App) : TestBase<App>
{
    [Fact]
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

    [Fact]
    public async Task InvalidSignup_ReturnsBadRequestAndProblemDetails()
    {
        var expected = new[] {
            ("username", "Username to short"),
            ("email", "Email providet not valid"),
            ("password", "Password to short") };

        var invalidUser = new Request()
        {
            Username = "B",
            Email = "fffakkke",
            Password = "tw"
        };

        var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, FastEndpoints.ProblemDetails>(invalidUser);

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.Equal(3, res.Errors.Count());
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }
}
