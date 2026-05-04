
using System.Net;
using NotesWeb.Features.ToDo.ToDoItems.GetItem;

namespace NoteTest.Features.ToDo.ToDoItems.GetItem;

public class GetItemTests(App App, LoginState State) : LoggedinTests(App, State)
{
    private readonly NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request _validRequest = new()
    {
        Title = "A testing list"
    };
    [Fact]
    public async Task GetItem_CreateAnItemAndGetItBack_ItemIsReturned()
    {
        // SignUp user
        await SetTokenAsync();
        // Create a list
        var listId = await CreateAListAsync("Test list for adding items");
        _validRequest.ListId = listId;
        // Add an item
        var (rspPost, resPost) = await App.Client.POSTAsync<
            NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.CreateToDoItemEndpoint,
            NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request,
            NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Response>(_validRequest);
        Assert.Equal(HttpStatusCode.Created, rspPost.StatusCode);
        Assert.NotNull(resPost);


        // Get Item back
        var (rsp, res) = await App.Client.GETAsync<GetItemEndpoint, Request, Response>(
            new Request
            {
                ItemId = resPost.Id
            });


        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        // Assert same title
        Assert.Equal(_validRequest.Title, res.Title);
    }

    [Fact]
    public async Task GetItem_ItemDoesNotExist_ReturnsNotFound()
    {
        // SignUp user
        await SetTokenAsync();
        // Create a list
        var listId = await CreateAListAsync("Test list for adding items");
        _validRequest.ListId = listId;


        // Get Item from list
        var (rsp, res) = await App.Client.GETAsync<GetItemEndpoint, Request, Response>(
            new Request
            {
                ItemId = Guid.NewGuid()
            });


        // Assert response NotFound
        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
        Assert.Null(res);

    }
}
