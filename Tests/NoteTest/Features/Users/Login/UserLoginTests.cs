
using System.Net;
using NotesWeb.Features.Users.Login;

namespace NoteTest.Features.Users.Login;

public sealed class LoginState : StateFixture
{
    public bool SignedUp = false;
}

public class UserLoginTests(App App, LoginState State) : TestBase<App, LoginState>, IAsyncLifetime
{
    private Request _validLoginRequest = new()
    {
        Email = "test@example.com",
        Password = "Testing123",
    };

    /// <summary>
    /// This sign up user to the server if it is not signed up.
    /// </summary>
    /// <returns></returns>
    private async Task SignUpAsync()
    {
        NotesWeb.Features.Users.SignUp.Request user = new()
        {
            Username = "TestUser",
            FullName = "Test Userson",
            Password = _validLoginRequest.Password,
            Email = _validLoginRequest.Email,
        };

        if (!State.SignedUp)
        {
            var (rsp, res) = await App.Client.POSTAsync<NotesWeb.Features.Users.SignUp.SignUpEndpoint, NotesWeb.Features.Users.SignUp.Request, NotesWeb.Features.Users.SignUp.Response>(user);
            // Make sure success
            Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
            State.SignedUp = true;
        }
    }

    [Fact]
    public async Task ValidUserLogin_LoginIsCompletedAndJWTReturned()
    {
        // SignUp user
        await SignUpAsync();

        var (rsp, res) = await App.Client.POSTAsync<UserLoginEndpoint, Request, Response>(_validLoginRequest);

        // Assert Ok
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        // Assert JWT
        Assert.NotNull(res);
    }

    [Fact]
    public async Task WrongPassword_ReturnUnauthorized()
    {
        // SignUp user
        await SignUpAsync();
        Request wrongPassword = _validLoginRequest;
        wrongPassword.Password = "ADifferentPassword";

        // Try to login
        var (rsp, _) = await App.Client.POSTAsync<UserLoginEndpoint, Request, ProblemDetails>(wrongPassword);

        // Assert unauthorized
        Assert.Equal(HttpStatusCode.Unauthorized, rsp.StatusCode);
    }

    [Fact]
    public async Task UnusedEmail_ReturnUnauthorized()
    {
        Request unusedEmail = _validLoginRequest;
        unusedEmail.Email = "ADifferent@email.com";

        // Try to login
        var (rsp, _) = await App.Client.POSTAsync<UserLoginEndpoint, Request, ProblemDetails>(unusedEmail);

        // Assert unauthorized
        Assert.Equal(HttpStatusCode.Unauthorized, rsp.StatusCode);
    }

    [Fact]
    public async Task NotAValidEmail_ReturnUnauthorized()
    {
        Request invalidEmail = _validLoginRequest;
        invalidEmail.Email = "ADifferentemail.com";

        // Try to login
        var (rsp, _) = await App.Client.POSTAsync<UserLoginEndpoint, Request, ProblemDetails>(invalidEmail);

        // Assert unauthorized
        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
    }
    [Fact]
    public async Task PasswordToLong_ReturnUnauthorized()
    {
        await SignUpAsync();
        Request toLongPassword = _validLoginRequest;
        toLongPassword.Password = "AVeryLoooooooooooooooooooooongPassword";

        // Try to login
        var (rsp, _) = await App.Client.POSTAsync<UserLoginEndpoint, Request, ProblemDetails>(toLongPassword);

        // Assert unauthorized
        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
    }
    [Fact]
    public async Task PasswordToShort_ReturnUnauthorized()
    {
        await SignUpAsync();
        Request toShortPassword = _validLoginRequest;
        toShortPassword.Password = "short";

        // Try to login
        var (rsp, _) = await App.Client.POSTAsync<UserLoginEndpoint, Request, ProblemDetails>(toShortPassword);

        // Assert unauthorized
        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
    }
}
