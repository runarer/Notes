
using System.Net;
using NotesWeb.Features.ToDo.ToDoItems.RenameToDoItem;

namespace NoteTest.Features.ToDo.ToDoItems.RenameToDoItem;

public class RenameToDoItemTests(App App, LoginState State) : LoggedinTests(App, State)
{

    [Fact, Priority(1)]
    public async Task RenameToDoItem_CreateAnItemRenameIt_ItemIsRenamed()
    {
        await SetTokenAsync();
        // Create first list
        var list = await CreateAListAsync("First List");

        // Create item in first list
        var itemId = await CreateAnItemAsync(list, "Item to move");

        var newTitle = "New title";
        var request = new Request
        {
            ItemId = itemId,
            Title = newTitle
        };

        // Rename item
        var (rsp, res) = await App.Client.PATCHAsync<RenameToDoItemEndpoint, Request, Response>(request);

        // Assert item is now renamed
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.Equal(newTitle, res.Title);
    }

    [Theory, Priority(2)]
    [InlineData("", "You need to provide a title")]
    [InlineData("s", "Title is to short")]
    [InlineData("ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss", "Title is to long")]
    public async Task RenameToDoItem_CreateAnItemRenameItWithInvalidTitles_ReturnsProblemDetailsWithErrorMessage(string title, string error)
    {

        await SetTokenAsync();
        var expected = new[] {
            ("title", error)};
        // Create first list
        var list = await CreateAListAsync("List for invalids");

        // Create item in first list
        var itemId = await CreateAnItemAsync(list, "Item to move");

        // Rename item
        var (rsp, res) = await App.Client.PATCHAsync<RenameToDoItemEndpoint, Request, ProblemDetails>(
            new Request
            {
                ItemId = itemId,
                Title = title
            });

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));

    }

    [Fact]
    public async Task RenameToDoItem_ItemDoesNotExist_ReturnNotFound()
    {
        // Rename item
        var (rsp, _) = await App.Client.PATCHAsync<RenameToDoItemEndpoint, Request, Response>(new Request
        {
            ItemId = Guid.NewGuid(),
            Title = "Test"
        });
        // Assert NotFound
        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
    }
}