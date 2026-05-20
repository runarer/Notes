
using System.Net;
using NotesWeb.Features.ToDo.ToDoItems;
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
        var (rsp, res) = await App.Client.PATCHAsync<MoveToDoItemEndpoint, Request, ItemResponse>(request);

        // Assert item is in second list
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);
        Assert.Equal(secondList, res.ParentListId);
    }
    [Fact]
    public async Task MoveToDoItem_CreateOneListAddItemMoveItemToNonExistingList_ReturnNotFound()
    {
        await SetTokenAsync();
        // Create first list
        var firstList = await CreateAListAsync("First List");
        // Create second list
        var secondList = Guid.NewGuid();
        // Create item in first list
        var itemId = await CreateAnItemAsync(firstList, "Item to move");

        var request = new Request
        {
            ItemId = itemId,
            ToList = secondList

        };

        // Move item from first to second list
        var (rsp, _) = await App.Client.PATCHAsync<MoveToDoItemEndpoint, Request, Response>(request);

        // Assert item is in second list
        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
    }

    [Fact]
    public async Task MoveToDoItem_CreateAListMoveAnNonExistingItemFromItToAList_ReturnsNotFound()
    {
        await SetTokenAsync();
        // Create target list
        var secondList = Guid.NewGuid();
        // Create item in first list
        var itemId = Guid.NewGuid();

        var request = new Request
        {
            ItemId = itemId,
            ToList = secondList

        };

        // Move item from first to second list
        var (rsp, _) = await App.Client.PATCHAsync<MoveToDoItemEndpoint, Request, Response>(request);

        // Assert item is in second list
        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
    }

    [Fact]
    public async Task MoveToDoItem_CreateListsAndItemThenTryMoveItemWithAnotherAccount_ReturnForbidden()
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

        await SwitchUser();

        // Move item from first to second list
        var (rsp, _) = await App.Client.PATCHAsync<MoveToDoItemEndpoint, Request, ItemResponse>(request);

        // Assert item is in second list
        Assert.Equal(HttpStatusCode.Forbidden, rsp.StatusCode);

        await SwitchBackUser();
    }

    [Fact]
    public async Task MoveToDoItem_CreateListsAndItemThenTryMoveItemWithAnotherAccount_OwnTargetList_ReturnForbidden()
    {
        await SetTokenAsync();
        // Create first list
        var sourceList = await CreateAListAsync("First List");
        // Create item in first list
        var itemId = await CreateAnItemAsync(sourceList, "Item to move");

        await SwitchUser();

        // Create second list
        var targetList = await CreateAListAsync("Second List");

        var request = new Request
        {
            ItemId = itemId,
            ToList = targetList

        };

        // Move item from first to second list
        var (rsp, _) = await App.Client.PATCHAsync<MoveToDoItemEndpoint, Request, ItemResponse>(request);

        // Assert item is in second list
        Assert.Equal(HttpStatusCode.Forbidden, rsp.StatusCode);

        await SwitchBackUser();
    }

    [Fact]
    public async Task MoveToDoItem_CreateListsAndItemThenTryMoveItemWithAnotherAccount_OwnSourceListAndItem_ReturnForbidden()
    {
        await SetTokenAsync();
        // Create target list
        var targetList = await CreateAListAsync("Second List");

        await SwitchUser();

        // Create source list
        var sourceList = await CreateAListAsync("First List");
        // Create item in source list
        var itemId = await CreateAnItemAsync(sourceList, "Item to move");

        var request = new Request
        {
            ItemId = itemId,
            ToList = targetList

        };

        // Move item from first to second list
        var (rsp, _) = await App.Client.PATCHAsync<MoveToDoItemEndpoint, Request, ItemResponse>(request);

        // Assert item is in second list
        Assert.Equal(HttpStatusCode.Forbidden, rsp.StatusCode);

        await SwitchBackUser();
    }
}
