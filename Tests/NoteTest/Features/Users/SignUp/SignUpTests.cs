
using System.Net;
using NotesWeb.Features.Users.SignUp;

namespace NoteTest.Features.Users.SignUp;

public class SignUpTests(App App) : TestBase<App>
{
    private Request _validUser = new()
    {
        Username = "Bobb",
        FullName = "Bob Bobby Bobson",
        Email = "bob@example.com",
        Password = "BobIsAwesome"
    };


    [Fact, Priority(1)]
    public async Task ValidUserSignup_ReturnsOkAndUserObject()
    {
        var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, Response>(_validUser);

        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        Assert.NotNull(res);
    }

    [Fact, Priority(2)]
    public async Task UsernameTaken_ReturnsOkAndUserObject()
    {
        var expected = new[] {
            ("username", "this username is taken!")};
        Request newUser = _validUser;
        newUser.Email = "F" + _validUser.Email;

        var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, ProblemDetails>(newUser);

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }
    [Fact, Priority(2)]
    public async Task EmailTaken_ReturnsOkAndUserObject()
    {
        var expected = new[] {
            ("email", "this email is already used!")};
        Request newUser = _validUser;
        newUser.Username = "F" + _validUser.Username;

        var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, ProblemDetails>(newUser);

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task InvalidEmail_ReturnsBadRequestAndProblemDetails()
    {
        var expected = new[] {
            ("email", "Email providet is not a valid email")};

        var invalidUser = _validUser;
        invalidUser.Email = "fffakkke";

        var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, ProblemDetails>(invalidUser);

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task UsernameToLong_ReturnsBadRequestAndProblemDetails()
    {
        var invalidUser = _validUser;
        invalidUser.Username = "Boobbbbbbbbbbbbbbbyyyyyyyyyyyyyyyyyyyyy";
        var expected = new[] {
            ("username", "Username is to long") };

        var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, ProblemDetails>(invalidUser);

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task PasswordToLong_ReturnsBadRequestAndProblemDetails()
    {
        var invalidUser = _validUser;
        invalidUser.Password = "Boobbbbbbbbbbbbbbbyyyyyyyyyyyyyyyyyyyyy";
        var expected = new[] {
            ("password", "Password is to long") };

        var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, ProblemDetails>(invalidUser);

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }
    [Fact]
    public async Task UsernameToShort_ReturnsBadRequestAndProblemDetails()
    {
        var invalidUser = _validUser;
        invalidUser.Username = "B";
        var expected = new[] {
            ("username", "Username is to short") };

        var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, ProblemDetails>(invalidUser);

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task PasswordToShort_ReturnsBadRequestAndProblemDetails()
    {
        var invalidUser = _validUser;
        invalidUser.Password = "Bo";
        var expected = new[] {
            ("password", "Password is to short") };

        var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, ProblemDetails>(invalidUser);

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }
}
