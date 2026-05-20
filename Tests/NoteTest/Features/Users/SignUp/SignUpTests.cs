
using System.Net;
using NotesWeb.Features.Users.SignUp;

namespace NoteTest.Features.Users.SignUp;

public class SignUpTests(App App) : TestBase<App>
{
    private readonly Request _validUser = new()
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

    [Fact, Priority(1)]
    public async Task SignupSeveralUsers_ReturnsOkAndUserObject()
    {
        Request[] validUsers = [
        new() {Username = "User1", FullName = "First User", Email = "first@example.com", Password ="firstUser" },
        new() {Username = "User2", FullName = "Second User", Email = "second@example.com", Password ="secondUser" },
        new() {Username = "User3", FullName = "Third User", Email = "third@example.com", Password ="thirdUser" },
        new() {Username = "User4", FullName = "Forth User", Email = "forth@example.com", Password ="forthUser" },
        new() {Username = "User5", FullName = "Fifth User", Email = "fifth@example.com", Password ="fifthUser" },
        new() {Username = "User6", FullName = "Sixth User", Email = "sixth@example.com", Password ="sixthUser" },
        new() {Username = "User7", FullName = "Seventh User", Email = "seventh@example.com", Password ="seventhUser" },
        new() {Username = "User8", FullName = "Eigth User", Email = "eigth@example.com", Password ="eigthUser" },

    ];

        foreach (var user in validUsers)
        {
            var (rsp, res) = await App.Client.POSTAsync<SignUpEndpoint, Request, Response>(user);

            Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
            Assert.NotNull(res);
        }

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
