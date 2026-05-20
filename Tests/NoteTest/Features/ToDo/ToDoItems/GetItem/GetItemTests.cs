
using System.Net;
using NotesWeb.Features.ToDo.ToDoItems;
using NotesWeb.Features.ToDo.ToDoItems.GetItem;

namespace NoteTest.Features.ToDo.ToDoItems.GetItem;

public class GetItemTests(App App, LoginState State) : LoggedinTests(App, State)
{
    [Fact]
    public async Task GetItem_CreateAnItemAndGetItBack_ItemIsReturned()
    {
        // SignUp user
        await SetTokenAsync();
        // Create a list
        var listId = await CreateAListAsync("Test list for adding items");
        // Add an item
        var itemTitle = "A testing list";
        var itemId = await CreateAnItemAsync(listId, itemTitle);


        // Get Item back
        var (rsp, res) = await App.Client.GETAsync<GetItemEndpoint, Request, ItemResponse>(
            new Request
            {
                ItemId = itemId
            });


        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        // Assert same title
        Assert.Equal(itemTitle, res.Title);
    }

    [Fact]
    public async Task GetItem_ItemDoesNotExist_ReturnsNotFound()
    {
        // SignUp user
        await SetTokenAsync();

        // Get Item 
        var (rsp, res) = await App.Client.GETAsync<GetItemEndpoint, Request, ItemResponse>(
            new Request
            {
                ItemId = Guid.NewGuid()
            });


        // Assert response NotFound
        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
        Assert.Null(res);

    }


    [Fact]
    public async Task GetItem_CreateAnItemAndAccessItWithAnotherAccount_ReturnForbidden()
    {
        // SignUp user
        await SetTokenAsync();
        // Create a list and an item.

        var itemTitle = "A testing list";
        var listId = await CreateAListAsync("Test list for adding items");
        var itemId = await CreateAnItemAsync(listId, itemTitle);

        await SwitchUser();

        // Get Item back
        var (rsp, _) = await App.Client.GETAsync<GetItemEndpoint, Request, ItemResponse>(
            new Request
            {
                ItemId = itemId
            });


        Assert.Equal(HttpStatusCode.Forbidden, rsp.StatusCode);

        await SwitchBackUser();
    }
}
