using System.Net;
using System.Net.Http.Headers;
using FastEndpoints.Security;
using NotesWeb.Features.ToDo.ToDoItems;

namespace NoteTest.Features.ToDo;

public class LoginState : StateFixture
{
    public bool Token = false;
    public bool SecondUserCreated = false;
    public Guid ListId = default;
    public Guid ItemId = default;
}

public class LoggedinTests(App app, LoginState state) : TestBase<App, LoginState>//, IAsyncLifetime
{

    protected App App = app;
    protected LoginState State = state;
    private AuthenticationHeaderValue? _oldAuthHeader = null;
    /// <summary>
    /// This sign up user to the server if it is not signed up.
    /// </summary>
    /// <returns></returns>
    protected async Task SetTokenAsync()
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
            var (rsp1, _) = await App.Client.POSTAsync<
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

    protected async Task SwitchUser()
    {
        // Another user, 
        NotesWeb.Features.Users.SignUp.Request newUser = new()
        {
            Username = "ForbiddenUser",
            FullName = "Test Userson",
            Password = "Testing123",
            Email = "testforbidden@example.com",
        };

        // Create a second user if not created
        if (!State.SecondUserCreated)
        {
            // Create second user
            var (c2u_rsp, _) = await App.Client.POSTAsync<
                NotesWeb.Features.Users.SignUp.SignUpEndpoint,
                NotesWeb.Features.Users.SignUp.Request,
                NotesWeb.Features.Users.SignUp.Response>(newUser);
            Assert.Equal(HttpStatusCode.Created, c2u_rsp.StatusCode);
            State.SecondUserCreated = true;
        }

        // Save old auth
        _oldAuthHeader = App.Client.DefaultRequestHeaders.Authorization;
        // Login second user and switch jwt
        var (l2u_rsp, l2u_res) = await App.Client.POSTAsync<
            NotesWeb.Features.Users.Login.UserLoginEndpoint,
            NotesWeb.Features.Users.Login.Request,
            NotesWeb.Features.Users.Login.Response>(
                new NotesWeb.Features.Users.Login.Request { Email = newUser.Email, Password = newUser.Password });
        Assert.Equal(HttpStatusCode.OK, l2u_rsp.StatusCode);
        Assert.NotNull(l2u_res);
        App.Client.DefaultRequestHeaders.Authorization = new("Bearer", l2u_res.Token);
    }

    protected async Task SwitchBackUser()
    {
        if (State.SecondUserCreated && _oldAuthHeader != null)
            App.Client.DefaultRequestHeaders.Authorization = _oldAuthHeader;
    }


    protected async Task SetList()
    {
        await SetTokenAsync();
        State.ListId = await CreateAListAsync("Testing list");
    }

    protected async Task<Guid> CreateAListAsync(string title)
    {
        var (rsp, res) = await App.Client.POSTAsync<
            NotesWeb.Features.ToDo.ToDoLists.CreateList.CreateListEndpoint,
            NotesWeb.Features.ToDo.ToDoLists.CreateList.Request,
            NotesWeb.Features.ToDo.ToDoLists.CreateList.Response>(
                new NotesWeb.Features.ToDo.ToDoLists.CreateList.Request
                {
                    Title = title
                });
        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        Assert.NotNull(res);
        Assert.NotEqual(default, res.ListId);

        return res.ListId;
    }

    protected async Task<Guid> CreateAnItemAsync(Guid listId, string title)
    {
        var (rsp, res) = await App.Client.POSTAsync<
            NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.CreateToDoItemEndpoint,
            NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request,
            ItemResponse>(
                new NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request
                {
                    ListId = listId,
                    Title = title
                });
        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        Assert.NotNull(res);

        return res.ItemId;
    }
    protected async Task<Guid> CreateAnItemAsync(NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request item)
    {
        var (rsp, res) = await App.Client.POSTAsync<
            NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.CreateToDoItemEndpoint,
            NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request,
            ItemResponse>(item);
        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        Assert.NotNull(res);

        return res.ItemId;
    }
}
