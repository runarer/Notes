
using System.Net;
using NotesWeb.Features.ToDo.ToDoItems.GetListItems;

namespace NoteTest.Features.ToDo.ToDoItems.GetList;

public class GetListTests(App App, LoginState State) : LoggedinTests(App, State)
{

    [Fact]
    public async Task GetListItems_CreateAListAndAddItemsThenGetList_ReturnsListOfItems()
    {
        // SignUp user
        await SetTokenAsync();
        // Create a list
        var listId = await CreateAListAsync("List for testing GetList");
        // Add several items
        string[] items = ["Test item 1", "Test item 2", "Test item 3"];
        foreach (var item in items)
            _ = await CreateAnItemAsync(listId, item);


        // Get list
        var (rsp, res) = await App.Client.GETAsync<GetListItemsEndpoint, Request, Response>(new Request
        {
            ListId = listId
        });


        // Assert all items and only these items are in the list
        Assert.Equivalent(items, res.List.Select(e => e.Title));
        Assert.Equal(items.Length, res.List.Length);
    }

    [Fact]
    public async Task GetListItems_CreateAListDoNotAddItems_ReturnsAnEmptyList()
    {
        // SignUp user
        await SetTokenAsync();
        // Create a list
        var listId = await CreateAListAsync("List for testing GetList");

        // Get list
        var (rsp, res) = await App.Client.GETAsync<GetListItemsEndpoint, Request, Response>(new Request
        {
            ListId = listId
        });

        // Assert list empty
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.Empty(res.List);
    }

    [Fact]
    public async Task GetListItems_ListDoNotExist_ReturnNotFound()
    {
        // SignUp user
        await SetTokenAsync();
        // Get list
        var (rsp, _) = await App.Client.GETAsync<GetListItemsEndpoint, Request, Response>(new Request
        {
            ListId = Guid.NewGuid()
        });
        // Assert NotFound
        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
    }


    [Fact]
    public async Task GetListItems_CreateAListAndAddItemsThenGetListWithSearchTerm_ReturnsListOfMatchingItems()
    {
        // SignUp user
        await SetTokenAsync();
        // Create a list
        var listId = await CreateAListAsync("List for testing GetList");
        var searchTerm = "asdf";
        // Add several items
        string[] items = [
            "Test item 1",
            searchTerm + " Test item 2",
            "Test" + searchTerm + "item 3",
            "Test item 4 " + searchTerm ];
        foreach (var item in items)
            _ = await CreateAnItemAsync(listId, item);


        // Get list
        var (rsp, res) = await App.Client.GETAsync<GetListItemsEndpoint, Request, Response>(new Request
        {
            ListId = listId
        });


        // Assert all items has searchTerm
        Assert.All(res.List, list => list.Title.Contains(searchTerm));
    }

    [Fact]
    public async Task GetListsItems_GetListOfItemsWithToDateBeforeFromDate_ReturnsProblemDetails()
    {
        var expected = new[] {
            ("fromUtc", "'from Utc' must be after 'To Utc'.")};

        await SetTokenAsync();

        var listId = await CreateAListAsync("Testing time error response");

        var (rsp, res) = await App.Client.GETAsync<GetListItemsEndpoint, Request, ProblemDetails>(
            new Request
            {
                ListId = listId,
                FromUtc = App.FakeTime.GetUtcNow().AddDays(-10),
                ToUtc = App.FakeTime.GetUtcNow().AddDays(-12)
            });

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));

    }

    [Fact]
    public async Task GetListsItems_GetListOfItemsWithFromDateInTheFuture_ReturnsProblemDetails()
    {
        var expected = new[] {
            ("fromUtc", "Date 'from Utc' must be in the past!")};

        await SetTokenAsync();

        var listId = await CreateAListAsync("Testing time error response");



        var (rsp, res) = await App.Client.GETAsync<GetListItemsEndpoint, Request, ProblemDetails>(
            new Request
            {
                ListId = listId,
                FromUtc = App.FakeTime.GetUtcNow().AddDays(3)
            });


        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));

    }

    [Fact]
    public async Task GetListsItems_GetListOfItemssWithTimeSearch_ReturnsListWithinTimeMatch()
    {
        List<string> items = ["Test item 3 days ago", "Test item 4 day ago"];
        await SetTokenAsync();

        var listId = await CreateAListAsync("Testing from and to Utc");

        var from = App.FakeTime.GetUtcNow().AddHours(-1);
        _ = await CreateAnItemAsync(listId, items[0]);
        var to = App.FakeTime.GetUtcNow().AddHours(2);

        App.FakeTime.Advance(TimeSpan.FromDays(3));
        _ = await CreateAnItemAsync(listId, items[1]);


        Assert.NotEqual(to, from);


        var (rsp, res) = await App.Client.GETAsync<GetListItemsEndpoint, Request, Response>(new Request
        {
            ListId = listId,
            FromUtc = from,
            ToUtc = to
        });

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.Single(res.List);
        Assert.Equal(items[0], res.List[0].Title);

    }
}