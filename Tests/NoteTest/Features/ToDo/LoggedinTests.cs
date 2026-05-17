using System.Net;

namespace NoteTest.Features.ToDo;

public class LoginState : StateFixture
{
    public bool Token = false;
    public Guid ListId = default;
    public Guid ItemId = default;
}

public class LoggedinTests(App app, LoginState state) : TestBase<App, LoginState>//, IAsyncLifetime
{

    protected App App = app;
    protected LoginState State = state;
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
        Assert.NotEqual(default, res.Id);

        return res.Id;
    }

    protected async Task<Guid> CreateAnItemAsync(Guid listId, string title)
    {
        var (rsp, res) = await App.Client.POSTAsync<
            NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.CreateToDoItemEndpoint,
            NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request,
            NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Response>(
                new NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request
                {
                    ListId = listId,
                    Title = title
                });
        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        Assert.NotNull(res);

        return res.Id;
    }
}
