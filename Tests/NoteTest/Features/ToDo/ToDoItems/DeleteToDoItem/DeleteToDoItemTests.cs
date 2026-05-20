
using System.Net;
using NotesWeb.Features.ToDo.ToDoItems;
using NotesWeb.Features.ToDo.ToDoItems.DeleteToDoItem;

namespace NoteTest.Features.ToDo.ToDoItems.DeleteToDoItem;

public class DeleteToDoItemTests(App App, LoginState State) : LoggedinTests(App, State)
{
    [Fact]
    public async Task DeleteItem_CreateAndItemDeleteItem_ItemIsGone()
    {
        // SignUp user
        await SetTokenAsync();

        // Add and item
        var listId = await CreateAListAsync("Test list for deleting");
        var item = await CreateAnItemAsync(listId, "Delete item");


        // Delete Item
        var rsp = await App.Client.DELETEAsync<DeleteToDoItemEndpoint, Request>(new Request { ItemId = item });


        Assert.Equal(HttpStatusCode.NoContent, rsp.StatusCode);

        // Get list content
        var (rspGet, resGet) = await App.Client.GETAsync<
            NotesWeb.Features.ToDo.ToDoItems.GetListItems.GetListItemsEndpoint,
            NotesWeb.Features.ToDo.ToDoItems.GetListItems.Request,
            NotesWeb.Features.ToDo.ToDoItems.GetListItems.Response>(
                new NotesWeb.Features.ToDo.ToDoItems.GetListItems.Request
                {
                    ListId = listId
                });
        Assert.Equal(HttpStatusCode.OK, rspGet.StatusCode);

        // Assert list empty
        Assert.Empty(resGet.List);
    }

    [Fact]
    public async Task DeleteItem_CreateAndItemSwitchUserTryDeleteItem_ReturnForbiddenItemNotGone()
    {
        // SignUp user
        await SetTokenAsync();

        // Add and item
        var listId = await CreateAListAsync("Test list for deleting");
        var itemId = await CreateAnItemAsync(listId, "Delete item");

        await SwitchUser();

        // Delete Item
        var del_rsp = await App.Client.DELETEAsync<DeleteToDoItemEndpoint, Request>(new Request { ItemId = itemId });
        Assert.Equal(HttpStatusCode.Forbidden, del_rsp.StatusCode);

        await SwitchBackUser();

        // Make sure item still there
        var (rsp, res) = await App.Client.GETAsync<
                NotesWeb.Features.ToDo.ToDoItems.GetItem.GetItemEndpoint,
                NotesWeb.Features.ToDo.ToDoItems.GetItem.Request, ItemResponse>(
            new NotesWeb.Features.ToDo.ToDoItems.GetItem.Request
            {
                ItemId = itemId
            });

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);
    }

    [Fact]
    public async Task DeleteItem_DeleteAnItemThatDoNotExists_ReturnNotFound()
    {
        // SignUp user
        await SetTokenAsync();

        // Delete Item
        var rsp = await App.Client.DELETEAsync<DeleteToDoItemEndpoint, Request>(new Request { ItemId = Guid.NewGuid() });

        // Check return
        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);

    }
}
