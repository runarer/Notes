using System.Net;
using NotesWeb.Features.ToDo.GetListsWithItems;

namespace NoteTest.Features.ToDo.GetListsWithItems;

// This test is on its own as it messes with server time
public class GetListsWithItemsTimeTestsTests(App App, LoginState State) : LoggedinTests(App, State)
{
    [Fact]
    public async Task GetListsWithItems_CreateListsAndAddItemsWithDueDate_ReturnsListOfItemsWithinCorrectDueDate()
    {
        await SetTokenAsync();
        // Create two list and add two item to each with a few days between
        var list1 = "Test list 1 time";
        var listId1 = await CreateAListAsync(list1);

        var list2 = "Test list 2 time";
        var listId2 = await CreateAListAsync(list2);

        var item1 = "Test item 1 time";
        _ = await CreateAnItemAsync(listId1, item1);

        App.FakeTime.Advance(TimeSpan.FromDays(2));

        var item2 = "Test item 2 time";
        _ = await CreateAnItemAsync(listId1, item2);

        App.FakeTime.Advance(TimeSpan.FromDays(1));

        var item3 = "Test item 2 time";
        _ = await CreateAnItemAsync(listId2, item3);

        App.FakeTime.Advance(TimeSpan.FromDays(2));

        var item4 = "Test item 2 time";
        _ = await CreateAnItemAsync(listId2, item4);


        // Look for items added four and three days ago.
        var from = App.FakeTime.GetUtcNow().AddDays(-4);
        var to = App.FakeTime.GetUtcNow().AddDays(-2);

        // SignUp user and add items
        await SetTokenAsync();


        // Get lists
        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, Response>(new Request
        {
            FromUtc = from,
            ToUtc = to
        });
        // Assert
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        // List returned are as expected
        Assert.Equal(2, res.Lists.Length);
        var firstList = res.Lists.Single(list => list.Title == list1);
        var secondList = res.Lists.Single(list => list.Title == list2);

        Assert.Single(firstList.Items);
        Assert.Single(secondList.Items);

        var firstListItem = firstList.Items.Single(item => item.Title == item2);

        var secondListItem = firstList.Items.Single(item => item.Title == item3);

        Assert.Equal(item2, firstListItem.Title);

        Assert.Equal(item3, secondListItem.Title);
    }
}