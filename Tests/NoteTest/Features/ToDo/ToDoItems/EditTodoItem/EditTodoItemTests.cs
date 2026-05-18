
using System.Net;
using NotesWeb.Features.ToDo.ToDoItems.EditToDoItem;

namespace NoteTest.Features.ToDo.ToDoItems.EditToDoItem;

public class EditToDoItemTests(App App, LoginState State) : LoggedinTests(App, State)
{

    [Fact]
    public async Task EditToDoItem_EditAnItem_ItemIsEdited()
    {
        await SetTokenAsync();
        // Create first list
        var list = await CreateAListAsync("First List");

        // Create item in first list
        var itemId = await CreateAnItemAsync(list, "Item to edit");

        var newTitle = "New title";
        var newDescription = "New description";
        var newDue = DateTimeOffset.UtcNow.AddDays(1);
        var request = new Request
        {
            ItemId = itemId,
            Title = newTitle,
            Description = newDescription,
            Due = newDue
        };

        // Edit item
        var (rsp, res) = await App.Client.PATCHAsync<EditToDoItemEndpoint, Request, Response>(request);

        // Assert item is now edited
        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);
        Assert.Equal(newTitle, res.Title);
        Assert.NotNull(res.Description);
        Assert.Equal(newDescription, res.Description);
        Assert.NotNull(res.Due);
        Assert.Equal(newDue, res.Due);
    }

    [Theory]
    [InlineData("", "You need to provide a title")]
    [InlineData("s", "Title is to short")]
    [InlineData("ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss", "Title is to long")]
    public async Task EditToDoItem_CreateAnItemEditItWithInvalidTitles_ReturnsProblemDetailsWithErrorMessage(string title, string error)
    {

        await SetTokenAsync();
        var expected = new[] {
            ("title", error)};
        // Create first list
        var list = await CreateAListAsync("List for invalids");

        // Create item in first list
        var itemId = await CreateAnItemAsync(list, "Item to move");

        // Edit item
        var (rsp, res) = await App.Client.PATCHAsync<EditToDoItemEndpoint, Request, ProblemDetails>(
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
    public async Task EditToDoItem_ItemDoesNotExist_ReturnNotFound()
    {
        // Edit item
        var (rsp, _) = await App.Client.PATCHAsync<EditToDoItemEndpoint, Request, Response>(new Request
        {
            ItemId = Guid.NewGuid(),
            Title = "Test"
        });
        // Assert NotFound
        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
    }
}