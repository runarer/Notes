/*using System.Net;
using NotesWeb.Features.ToDo.GetListsWithItems;

namespace NoteTest.Features.ToDo.GetListsWithItems;

public class GetListTests(App App, LoginState State) : LoggedinTests(App, State)
{

    private const string _searchTerm = "asdf";
    private readonly NotesWeb.Features.ToDo.ToDoLists.CreateList.Request[] _lists = [
        new (){ Title = "Testlist 1" },
        new (){ Title = "Testlist 2" },
        new (){ Title = "Testlist 3" },
        new (){ Title = "Testlist 4" },
    ];

    private readonly NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request[] _items1 = [
      new (){Title = "T1 Complete", Description = "Complete this item"},
      new (){Title = "T1 not completed", Description = "not complete this item"},
      new (){Title = "T1"+_searchTerm, Description = "SearchTerm in title"},
      new (){Title = "T1 not completed", Description = "Search for this" + _searchTerm},
      new (){Title = "T1", Description = "Item is due in 3 days", Due = App.FakeTime.GetUtcNow().AddDays(3)},
      new (){Title = "T1", Description = "Item is due in 5 days", Due = App.FakeTime.GetUtcNow().AddDays(5)},
    ];

    private readonly NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request[] _items2 = [
      new (){Title = "T2 Complete", Description = "Complete this item"},
      new (){Title = "T2 not completed", Description = "not complete this item"},
      new (){Title = "T2"+_searchTerm, Description = "SearchTerm in title"},
      new (){Title = "T2 not completed", Description = "Search for this" + _searchTerm},
      new (){Title = "T2", Description = "Item is due in 3 days", Due = App.FakeTime.GetUtcNow().AddDays(3)},
      new (){Title = "T2", Description = "Item is due in 5 days", Due = App.FakeTime.GetUtcNow().AddDays(5)},
    ];

    private readonly NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request[] _items3 = [
      new (){Title = "T3 Complete", Description = "Complete this item"},
      new (){Title = "T3 not completed", Description = "not complete this item"},
      new (){Title = "T3"+_searchTerm, Description = "SearchTerm in title"},
      new (){Title = "T3 not completed", Description = "Search for this" + _searchTerm},
      new (){Title = "T3", Description = "Item is due in 3 days", Due = App.FakeTime.GetUtcNow().AddDays(3)},
      new (){Title = "T3", Description = "Item is due in 5 days", Due = App.FakeTime.GetUtcNow().AddDays(5)},
    ];

    private readonly NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request[] _items4 = [
      new (){Title = "T4 Complete", Description = "Complete this item"},
      new (){Title = "T4 not completed", Description = "not complete this item"},
      new (){Title = "T4"+_searchTerm, Description = "SearchTerm in title"},
      new (){Title = "T4 not completed", Description = "Search for this" + _searchTerm},
      new (){Title = "T4", Description = "Item is due in 3 days", Due = App.FakeTime.GetUtcNow().AddDays(3)},
      new (){Title = "T4", Description = "Item is due in 5 days", Due = App.FakeTime.GetUtcNow().AddDays(5)},
    ];


    private async Task CreateListsAndItems()
    {
        await SetTokenAsync();

        // foreach (var list in _lists)
        // {
        //     var (rsp, res) = await App.Client.POSTAsync<
        //     NotesWeb.Features.ToDo.ToDoLists.CreateList.CreateListEndpoint,
        //     NotesWeb.Features.ToDo.ToDoLists.CreateList.Request,
        //     NotesWeb.Features.ToDo.ToDoLists.CreateList.Response
        //     >(list);

        //     Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        //     Assert.NotNull(res);
        // }

        var listid = _lists.Select(async list =>
        {
            var (rsp, res) = await App.Client.POSTAsync<
                NotesWeb.Features.ToDo.ToDoLists.CreateList.CreateListEndpoint,
                NotesWeb.Features.ToDo.ToDoLists.CreateList.Request,
                NotesWeb.Features.ToDo.ToDoLists.CreateList.Response
            >(list);

            Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
            Assert.NotNull(res);

            return res.ListId;
        }).ToArray();



    }



    [Fact, Priority(1)]
    public async Task GetListsWithItems_NoLists_ReturnsAnEmptyListInItems()
    {
        // SignUp user
        await SetTokenAsync();

        // Get list
        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, Response>(new Request
        {
        });

        // Assert list empty
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.Empty(res.Lists);
    }


    [Fact, Priority(2)]
    public async Task GetListsWithItems_CreateListsAndAddItemsThenEverythingBack_ReturnsListOfItems()
    {
        // SignUp user
        await SetTokenAsync();



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

        NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request[] items = [
                new() {ListId = listId,Title = "Test item 1"},
                new() {ListId = listId,Title = searchTerm+"Test item 2"},
                new() {ListId = listId,Title = "Test item 3", Description ="Some text"+searchTerm},
                new() {ListId = listId,Title = "Test item 4"+searchTerm},
                new() {ListId = listId,Title = "Test item 5", Description ="Some text"},

            ];
        foreach (var item in items)
            _ = await CreateAnItemAsync(item);


        // Get list
        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, Response>(new Request
        {
            Search = searchTerm,

        });


        // Assert all items has searchTerm
        // Assert.Equal(3, res.List.Length);
        // Assert.All(res.List, list =>
        // {
        //     Assert.True(
        //         list.Title.Contains(searchTerm) ||
        //         (list.Description is not null && list.Description.Contains(searchTerm)));
        // });

    }


    [Fact]
    public async Task GetListsItems_GetListOfItemsWithToDateBeforeFromDate_ReturnsProblemDetails()
    {
        var expected = new[] {
            ("fromUtc", "'from Utc' must be after 'To Utc'.")};

        await SetTokenAsync();

        var listId = await CreateAListAsync("Testing time error response");

        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, ProblemDetails>(
            new Request
            {
                FromUtc = App.FakeTime.GetUtcNow().AddDays(-10),
                ToUtc = App.FakeTime.GetUtcNow().AddDays(-12)
            });

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));

    }


    [Fact]
    public async Task GetListsItems_GetListOfItemsWithDueToDateBeforeDueFromDate_ReturnsProblemDetails()
    {
        var expected = new[] {
            ("dueFromUtc", "'due From Utc' must be after 'Due To Utc'.")};

        await SetTokenAsync();

        var listId = await CreateAListAsync("Testing time error response");

        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, ProblemDetails>(
            new Request
            {
                DueFromUtc = App.FakeTime.GetUtcNow().AddDays(10),
                DueToUtc = App.FakeTime.GetUtcNow().AddDays(8)
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



        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, ProblemDetails>(
            new Request
            {
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


        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, Response>(new Request
        {
            FromUtc = from,
            ToUtc = to
        });

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        // Assert.Single(res.List);
        // Assert.Equal(items[0], res.List[0].Title);

    }


    [Fact]
    public async Task GetListsItems_GetListOfItemssWithTimeSearchOnDueDate_ReturnsListWithinTimeMatch()
    {
        await SetTokenAsync();

        // Create a list and add items with due date to it
        var listId = await CreateAListAsync("Testing from and to Utc");
        NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request[] items = [
            new(){ListId = listId, Title = "Test item due in 3 days", Due = App.FakeTime.GetUtcNow().AddDays(3)},
            new(){ListId = listId, Title = "Test item due in 6 days", Due = App.FakeTime.GetUtcNow().AddDays(6)},
            new(){ListId = listId, Title = "Test item due 12 days", Due = App.FakeTime.GetUtcNow().AddDays(12)},
            new(){ListId = listId, Title = "Test item due in 4 days", Due = App.FakeTime.GetUtcNow().AddDays(5)},
        ];
        foreach (var item in items)
            _ = await CreateAnItemAsync(item);

        // Create testing time window
        var from = App.FakeTime.GetUtcNow().AddDays(4);
        var to = App.FakeTime.GetUtcNow().AddDays(8);
        Assert.NotEqual(to, from);

        // These are the titles of expected return items
        string[] expected = [.. items.Where(item => item.Due >= from && item.Due <= to).Select(item => item.Title)];


        // Act: Get items in time window
        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, Response>(new Request
        {
            DueFromUtc = from,
            DueToUtc = to
        });

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        //Check items
        // Assert.Equal(expected.Length, res.List.Length);
        // Assert.Equivalent(expected, res.List.Select(e => e.Title));
    }
}*/