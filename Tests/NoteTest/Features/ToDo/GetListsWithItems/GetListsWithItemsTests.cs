using System.Net;
using NotesWeb.Features.ToDo.GetListsWithItems;

namespace NoteTest.Features.ToDo.GetListsWithItems;

public class GetListTests(App App, LoginState State) : LoggedinTests(App, State)
{

    private async Task<List<(Guid, List<Guid>)>> AddListsAndItems(Response lists)
    {
        await SetTokenAsync();

        List<(Guid, List<Guid>)> addedListAndItems = [];

        foreach (var list in lists.Lists)
        {
            // Create the list
            var listId = await CreateAListAsync(list.Title);

            List<Guid> addedItems = [];
            // Add items to list
            foreach (var item in list.Items)
            {
                var itemId = await CreateAnItemAsync(new NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem.Request
                {
                    ListId = listId,
                    Title = item.Title,
                    Description = item.Description,
                    Due = item.Due
                });
                addedItems.Add(itemId);

            }
            addedListAndItems.Add((listId, addedItems));
        }
        return addedListAndItems;
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
        // All of this should be returned
        var listsAndItems = new Response
        {
            Lists = [
                new (){
                    Title = "Test List 1",
                    Items = [
                        new() { Title = "Test Item 1-1"},
                        new() { Title = "Test Item 1-2"}
                ]},
                new (){
                    Title = "Test List 2",
                    Items = [
                        new() {Title = "Test Item 2-1"}
                    ]
                }
            ]
        };

        // SignUp user and add items
        await SetTokenAsync();
        _ = await AddListsAndItems(listsAndItems);


        // Get lists
        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, Response>(new Request
        {
        });
        // Assert
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        // List returned are as expected
        Assert.Equal(2, res.Lists.Length);
        Assert.Equivalent(listsAndItems.Lists.Select(list => list.Title), res.Lists.Select(e => e.Title));

        // items in "Test List 1" matches items returned in list with title "Test List 1"
        var items1 = res.Lists.Single(list => list.Title == "Test List 1").Items;
        Assert.Equal(2, items1.Length);
        Assert.Equivalent(listsAndItems.Lists.Single(list => list.Title == "Test List 1").Items.Select(item => item.Title), items1.Select(e => e.Title));

        // items in "Test List 2" matches items returned in 
        var items2 = res.Lists.Single(list => list.Title == "Test List 2").Items;
        Assert.Single(items2);
        Assert.Equivalent(listsAndItems.Lists.Single(list => list.Title == "Test List 2").Items.Select(item => item.Title), items2.Select(e => e.Title));

    }


    [Fact, Priority(3)]
    public async Task GetListsWithItems_CreateListsAndAddItemsWithDueDate_ReturnsListOfItemsWithinCorrectDueDate()
    {
        var expectedList = "Test List Due 1";
        var expectedItem = "Test Item Due 1-2";
        // All of this should be returned
        var listsAndItems = new Response
        {
            Lists = [
                new (){
                    Title = expectedList,
                    Items = [
                        new() { Title = "Test Item Due 1-1", Due = App.FakeTime.GetUtcNow().AddDays(1)},
                        new() { Title = expectedItem, Due = App.FakeTime.GetUtcNow().AddDays(3)}
                ]},
                new (){
                    Title = "Test List Due 2",
                    Items = [
                        new() {Title = "Test Item Due 2-1", Due = App.FakeTime.GetUtcNow().AddDays(5)}
                    ]
                }
            ]
        };
        var from = App.FakeTime.GetUtcNow().AddDays(2);
        var to = App.FakeTime.GetUtcNow().AddDays(4);

        // SignUp user and add items
        await SetTokenAsync();
        _ = await AddListsAndItems(listsAndItems);


        // Get lists
        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, Response>(new Request
        {
            DueFromUtc = from,
            DueToUtc = to
        });
        // Assert
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        // List returned are as expected
        Assert.Single(res.Lists);
        Assert.Equal(expectedList, res.Lists[0].Title);

        Assert.Single(res.Lists[0].Items);
        Assert.Equal(expectedItem, res.Lists[0].Items[0].Title);
    }


    [Fact, Priority(3)]
    public async Task GetListsWithItems_CreateListsAndAddItemsWithDueDateButFromIsAfterTo_ReturnsProblemDetails()
    {
        var expected = new[] {
            ("dueFromUtc", "'due From Utc' must be after 'Due To Utc'.")};
        var from = App.FakeTime.GetUtcNow().AddDays(2);
        var to = App.FakeTime.GetUtcNow().AddDays(4);

        // SignUp user and add items
        await SetTokenAsync();


        // Get lists
        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, ProblemDetails>(new Request
        {
            DueFromUtc = to,
            DueToUtc = from
        });


        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));

    }



    [Fact, Priority(3)]
    public async Task GetListsWithItems_CreateListsAndAddItemsAndCompleteOne_ReturnsListOfCompletedItems()
    {
        var expectedList = "Test List Completed 1";
        var expectedItem = "Test Item Completed 1-2";
        // All of this should be returned
        var listsAndItems = new Response
        {
            Lists = [
                new (){
                    Title = expectedList,
                    Items = [
                        new() { Title = "Test Item Completed 1-1"},
                        new() { Title = expectedItem}
                ]},
                new (){
                    Title = "Test List Due 2",
                    Items = [
                        new() {Title = "Test Item Completed 2-1"}
                    ]
                }
            ]
        };

        // SignUp user and add items
        await SetTokenAsync();
        var addedlistsAndItems = await AddListsAndItems(listsAndItems);

        // Complete item
        var rspComplete = await App.Client.PATCHAsync<
            NotesWeb.Features.ToDo.ToDoItems.CompleteToDoItem.CompleteToDoItemEndpoint,
            NotesWeb.Features.ToDo.ToDoItems.CompleteToDoItem.Request>(new
            NotesWeb.Features.ToDo.ToDoItems.CompleteToDoItem.Request
            {
                ItemId = addedlistsAndItems[0].Item2[1] //First list second item
            });
        Assert.Equal(HttpStatusCode.OK, rspComplete.StatusCode);



        // Get lists
        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, Response>(new Request
        {
            Completed = true
        });
        // Assert
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        // List returned are as expected
        Assert.Single(res.Lists);
        Assert.Equivalent(expectedList, res.Lists[0].Title);

        Assert.Single(res.Lists[0].Items);
        Assert.Equivalent(expectedItem, res.Lists[0].Items[0].Title);
    }


    [Fact, Priority(3)]
    public async Task GetListsWithItems_CreateListsAndAddItemsThenSearch_ReturnsListOfItemsMatchingSearchterm()
    {
        var searchterm = "adf";
        // All of this should be returned
        var listsAndItems = new Response
        {
            Lists = [
                new (){
                    Title = "Test List 1",
                    Items = [
                        new() { Title = "Test Item 1-1"},
                        new() { Title = "Test Item 1-2"+searchterm}
                ]},
                new (){
                    Title = "Test List 2",
                    Items = [
                        new() {Title = "Test Item 2-1",Description = searchterm+"Test description"}
                    ]
                }
            ]
        };

        // SignUp user and add items
        await SetTokenAsync();
        _ = await AddListsAndItems(listsAndItems);


        // Get lists
        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, Response>(new Request
        {
            Search = searchterm
        });
        // Assert
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        // List returned are as expected
        Assert.Equal(2, res.Lists.Length);
        Assert.Equivalent(listsAndItems.Lists.Select(list => list.Title), res.Lists.Select(e => e.Title));

        // Make sure there is only one item in first list, this items title matches the one with searchterm in its title
        var items1 = res.Lists.Single(list => list.Title == "Test List 1").Items;
        Assert.Single(items1);
        Assert.Equivalent(
            listsAndItems
                .Lists.Single(list => list.Title == "Test List 1")
                .Items.Where(item => item.Title.Contains(searchterm))
                .Select(item => item.Title),
            items1.Select(e => e.Title));

        // Make sure there is only one item in second list, this items title matches the one with searchterm in its description
        var items2 = res.Lists.Single(list => list.Title == "Test List 2").Items;
        Assert.Single(items2);
        Assert.Equivalent(
            listsAndItems
                .Lists.Single(list => list.Title == "Test List 2")
                .Items.Where(item => item.Description!.Contains(searchterm))
                .Select(item => item.Title),
            items2.Select(e => e.Title));

    }
    [Fact, Priority(3)]
    public async Task GetListsWithItems_CreateListsAndAddItemsWithDateButFromIsAfterTo_ReturnsProblemDetails()
    {
        var expected = new[] {
            ("fromUtc", "'due From Utc' must be after 'Due To Utc'.")};
        var from = App.FakeTime.GetUtcNow().AddDays(2);
        var to = App.FakeTime.GetUtcNow().AddDays(4);

        // SignUp user and add items
        await SetTokenAsync();


        // Get lists
        var (rsp, res) = await App.Client.GETAsync<GetListsWithItemsEndpoint, Request, ProblemDetails>(new Request
        {
            FromUtc = to,
            ToUtc = from
        });


        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));

    }
}