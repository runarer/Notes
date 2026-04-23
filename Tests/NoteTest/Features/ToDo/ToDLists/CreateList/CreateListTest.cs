using System.Net;
using NotesWeb.Features.ToDo.ToDoLists.CreateList;

namespace NoteTest.Features.ToDo.ToDLists.CreateList;



public sealed class LoginState : StateFixture
{
    public bool Token = false;
}

public class UserLoginTests(App App, LoginState State) : TestBase<App, LoginState>, IAsyncLifetime
{

    /// <summary>
    /// This sign up user to the server if it is not signed up.
    /// </summary>
    /// <returns></returns>
    private async Task SetTokenAsync()
    {
        NotesWeb.Features.Users.SignUp.Request user = new()
        {
            Username = "TestUser2",
            FullName = "Test Userson",
            Password = "Testing123",
            Email = "test2@example.com",
        };

        if (!State.Token)
        {
            var (rsp1, re) = await App.Client.POSTAsync<
                NotesWeb.Features.Users.SignUp.SignUpEndpoint,
                NotesWeb.Features.Users.SignUp.Request,
                NotesWeb.Features.Users.SignUp.Response>(user);

            // Make sure success
            Assert.Equal(HttpStatusCode.Created, rsp1.StatusCode);

            var (rsp2, res) = await App.Client.POSTAsync<
                NotesWeb.Features.Users.Login.UserLoginEndpoint,
                NotesWeb.Features.Users.Login.Request,
                NotesWeb.Features.Users.Login.Response>(
                    new NotesWeb.Features.Users.Login.Request { Email = user.Email, Password = user.Password });

            // Assert Ok
            Assert.Equal(HttpStatusCode.OK, rsp2.StatusCode);
            // Assert JWT
            Assert.NotNull(res);

            App.Client.DefaultRequestHeaders.Authorization = new("Bearer", res.Token);

            State.Token = true;
        }
    }

    [Fact]
    public async Task CreateList_WithValidInput_ListCreated()
    {
        // SignUp user
        await SetTokenAsync();

    }
}
