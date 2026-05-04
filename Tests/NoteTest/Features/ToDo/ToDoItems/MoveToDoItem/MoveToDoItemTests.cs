
using System.Net;
using NotesWeb.Features.ToDo.ToDoItems.MoveToDoItem;

namespace NoteTest.Features.ToDo.ToDoItems.MoveToDoItem;

public class MoveToDoItemTests(App App, LoginState State) : LoggedinTests(App, State)
{
    [Fact]
    public async Task MoveToDoItem_CreateTwoListsAddItemToFirstMoveItemToSecond()
    {
        await SetTokenAsync();
        // Create first list
        var firstList = await CreateAListAsync("First List");
        // Create second list
        var secondList = await CreateAListAsync("Second List");
        // Create item in first list
        var itemId = await CreateAnItemAsync(firstList, "Item to move");

        var request = new Request
        {
            ItemId = itemId,
            ToList = secondList

        };

        // Move item from first to second list
        var (rsp, res) = await App.Client.PATCHAsync<MoveToDoItemEndpoint, Request, Response>(request);

        // Assert item is in second list
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);
        Assert.Equal(secondList, res.ParentListId);
    }
    [Fact]
    public async Task MoveToDoItem_CreateOneListAddItemMoveItemToNonExistingList_ReturnProblemDetails()
    {
        await SetTokenAsync();
        // Create first list
        // Create item in first list
        // Move item form first list to non-existing list
        // Assert problem details
    }

    [Fact]
    public async Task MoveToDoItem_MoveAnItemFromAnNonExistingList_ReturnNotFound()
    {
        await SetTokenAsync();
        // Move Item from list to list
        // Assert NotFound
    }

    [Fact]
    public async Task MoveToDoItem_CreateAListMoveAnNonExistingItemFromItToAList_ReturnsNotFound()
    {
        await SetTokenAsync();
        // Create list
        // Move Item
        // Assert NotFound
    }
}
