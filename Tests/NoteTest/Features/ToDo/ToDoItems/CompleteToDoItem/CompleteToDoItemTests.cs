
using System.Net;
using NotesWeb.Features.ToDo.ToDoItems.CompleteToDoItem;

namespace NoteTest.Features.ToDo.ToDoItems.CompleteToDoItem;

public class CompleteToDoItemTests(App App, LoginState State) : LoggedinTests(App, State)
{

    [Fact]
    public async Task CompleteToDoItem_ItemIsCreatedAndCompleted_ItemCompleted()
    {
        // SignUp user
        await SetTokenAsync();

        // Create a list
        var listId = await CreateAListAsync("List for testing complet item");
        // Add an item
        string item = "Test item 1";
        var itemId = await CreateAnItemAsync(listId, item);
        //Advance time so we can test updatetime
        App.FakeTime.Advance(TimeSpan.FromHours(2));
        var fakeTime = App.FakeTime.GetUtcNow();
        // Complete item
        var rspComplete = await App.Client.PATCHAsync<CompleteToDoItemEndpoint, Request>(new Request
        {
            ListId = listId,
            ItemId = itemId
        });


        Assert.Equal(HttpStatusCode.OK, rspComplete.StatusCode);
        // Get Item 
        var (rsp, res) = await App.Client.GETAsync<
        NotesWeb.Features.ToDo.ToDoItems.GetItem.GetItemEndpoint,
        NotesWeb.Features.ToDo.ToDoItems.GetItem.Request,
        NotesWeb.Features.ToDo.ToDoItems.GetItem.Response>(
            new NotesWeb.Features.ToDo.ToDoItems.GetItem.Request
            {
                ListId = listId,
                ItemId = itemId
            });
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);
        // Assert it's completed
        Assert.True(res.Completed);
        // Assert updated time is correct
        Assert.NotEqual(res.CreatedAtUtc, res.UpdatedAtUtc);
        Assert.Equal(fakeTime, res.UpdatedAtUtc);


    }

    [Fact]
    public async Task CompleteToDoItem_ItemIsCreatedAndCompletedThenUncompleted_ItemIsUncompleted()
    {
        // SignUp user
        await SetTokenAsync();

        // Create a list
        var listId = await CreateAListAsync("List for testing uncomplete");
        // Add an item
        string item = "Test item 1";
        var itemId = await CreateAnItemAsync(listId, item);

        // Complete item -> other tests assert item is set to completed
        var rspComplete = await App.Client.PATCHAsync<CompleteToDoItemEndpoint, Request>(new Request
        {
            ListId = listId,
            ItemId = itemId
        });
        Assert.Equal(HttpStatusCode.OK, rspComplete.StatusCode);

        //Advance time so we can test updatetime
        App.FakeTime.Advance(TimeSpan.FromHours(2));
        var fakeTime = App.FakeTime.GetUtcNow();



        var rspUncomplete = await App.Client.PATCHAsync<CompleteToDoItemEndpoint, Request>(new Request
        {
            ListId = listId,
            ItemId = itemId,
            Query = new() { Completed = false }
        });




        Assert.Equal(HttpStatusCode.OK, rspUncomplete.StatusCode);
        // Get Item 
        var (rsp, res) = await App.Client.GETAsync<
        NotesWeb.Features.ToDo.ToDoItems.GetItem.GetItemEndpoint,
        NotesWeb.Features.ToDo.ToDoItems.GetItem.Request,
        NotesWeb.Features.ToDo.ToDoItems.GetItem.Response>(
            new NotesWeb.Features.ToDo.ToDoItems.GetItem.Request
            {
                ListId = listId,
                ItemId = itemId
            });
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);
        // Assert it's not completed
        Assert.False(res.Completed);
        // Assert updated time is correct
        Assert.NotEqual(res.CreatedAtUtc, res.UpdatedAtUtc);
        Assert.Equal(fakeTime, res.UpdatedAtUtc);
    }

    [Fact]
    public async Task CompleteToDoItem_ItemDoNotExist_ReturnsNotFound()
    {
        await SetTokenAsync();

        // Create a list
        var listId = await CreateAListAsync("List for testing uncomplete");


        // Complete a non-existing item in the list
        var rsp = await App.Client.PATCHAsync<CompleteToDoItemEndpoint, Request>(new Request
        {
            ListId = listId,
            ItemId = Guid.NewGuid()
        });


        // Assert 404
        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
    }

    [Fact]
    public async Task CompleteToDoItem_ListDoesNotExist_ReturnsNotFound()
    {
        await SetTokenAsync();


        // Complete an item in a non-existing list
        var rsp = await App.Client.PATCHAsync<CompleteToDoItemEndpoint, Request>(new Request
        {
            ListId = Guid.NewGuid(),
            ItemId = Guid.NewGuid()
        });


        // Assert 404
        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
    }
}
