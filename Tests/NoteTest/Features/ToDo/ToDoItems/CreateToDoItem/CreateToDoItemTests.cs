
using System.Net;

using NotesWeb.Features.ToDo.ToDoItems.CreateToDoItem;

namespace NoteTest.Features.ToDo.ToDoItems.CreateToDoItem;

public class CreateToDoItemTests(App App, LoginState State) : LoggedinTests(App, State)
{
    private readonly Request _validRequest = new()
    {
        Title = "A testing list",
        Description = "A description for the testing list",
        Due = App.FakeTime.GetUtcNow().AddDays(1)
    };

    [Fact]
    public async Task CreateItem_WithValidInput_ItemCreated()
    {
        // SignUp user
        await SetTokenAsync();
        var fakeTime = App.FakeTime.GetUtcNow();
        var listId = await CreateAListAsync("Test list for adding items");
        _validRequest.ListId = listId;


        var (rsp, res) = await App.Client.POSTAsync<CreateToDoItemEndpoint, Request, Response>(_validRequest);

        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Equal(_validRequest.Title, res.Title);
        Assert.Equal(listId, res.ParentListId);
        Assert.NotEqual(default, res.ItemId);
        Assert.Equal(fakeTime, res.CreatedAtUtc);
        Assert.Equal(res.CreatedAtUtc, res.UpdatedAtUtc);
        Assert.False(res.Completed);
        Assert.NotNull(res.Description);
        Assert.Equal(_validRequest.Description, res.Description);
        Assert.NotNull(res.Due);
        Assert.Equal(_validRequest.Due, res.Due.Value);

        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
    }

    [Fact]
    public async Task CreateItem_ListIdDoNotExists_ReturnsNotFound()
    {
        await SetTokenAsync();
        _validRequest.ListId = Guid.NewGuid();

        var (rsp, _) = await App.Client.POSTAsync<CreateToDoItemEndpoint, Request, ProblemDetails>(_validRequest);

        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
    }

    [Fact]
    public async Task CreateItem_ValidInputWithNoDescriptionOrDueDate_ItemCreatedWithEmptyDescription()
    {
        await SetTokenAsync();
        var listId = await CreateAListAsync("Test list for adding items");
        var validRequest = _validRequest;
        validRequest.ListId = listId;
        validRequest.Description = null;
        validRequest.Due = null;

        var (rsp, res) = await App.Client.POSTAsync<CreateToDoItemEndpoint, Request, Response>(validRequest);

        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Equal(validRequest.Title, res.Title);
        Assert.Equal(listId, res.ParentListId);
        Assert.NotEqual(default, res.ItemId);
        Assert.Equal(App.FakeTime.GetUtcNow(), res.CreatedAtUtc);
        Assert.Equal(res.CreatedAtUtc, res.UpdatedAtUtc);
        Assert.False(res.Completed);
        Assert.Null(res.Description);
        Assert.Null(res.Due);
    }

    [Fact]
    public async Task CreateItem_DescriptionToLong_GetProblemDetailsWithErrorMessage()
    {
        await SetTokenAsync();

        var invalidRequest = _validRequest;
        invalidRequest.Description = "A way to long description.................................................................................................................................................................................................................";
        invalidRequest.ListId = Guid.NewGuid();

        var expected = new[] {
            ("description", "Description is to long")};

        var (rsp, res) = await App.Client.POSTAsync<CreateToDoItemEndpoint, Request, ProblemDetails>(invalidRequest);

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);
        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));

    }

    [Fact]
    public async Task CreateItem_DueDateInThePast_GetProblemDetailsWithErrorMessage()
    {
        await SetTokenAsync();
        var listId = await CreateAListAsync("Test list for adding items");

        var invalidRequest = _validRequest;
        invalidRequest.Due = App.FakeTime.GetUtcNow().AddDays(-1);
        invalidRequest.ListId = listId;

        var expected = new[] {
            ("due", "Due date cannot be in the past")};
        var (rsp, res) = await App.Client.POSTAsync<CreateToDoItemEndpoint, Request, ProblemDetails>(invalidRequest);

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));

    }

    [Fact]
    public async Task CreateItem_TitleToShort_GetProblemDetailsWithErrorMessage()
    {
        await SetTokenAsync();

        var invalidRequest = _validRequest;
        invalidRequest.Title = "t";
        invalidRequest.ListId = Guid.NewGuid();

        var expected = new[] {
            ("title", "Title is to short")};



        var (rsp, res) = await App.Client.POSTAsync<CreateToDoItemEndpoint, Request, ProblemDetails>(invalidRequest);



        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task CreateItem_TitleToLong_GetProblemDetailsWithErrorMessage()
    {
        await SetTokenAsync();

        var invalidRequest = _validRequest;
        invalidRequest.Title = "A way to long title...................................................................................";
        invalidRequest.ListId = Guid.NewGuid();

        var expected = new[] {
            ("title", "Title is to long")};



        var (rsp, res) = await App.Client.POSTAsync<CreateToDoItemEndpoint, Request, ProblemDetails>(invalidRequest);



        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task CreateItem_NoTitleProvided_GetProblemDetailsWithErrorMessage()
    {
        await SetTokenAsync();

        var invalidRequest = _validRequest;
        invalidRequest.Title = "";
        invalidRequest.ListId = Guid.NewGuid();

        var expected = new[] {
            ("title", "You need to provide a title")};



        var (rsp, res) = await App.Client.POSTAsync<CreateToDoItemEndpoint, Request, ProblemDetails>(invalidRequest);



        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }
    [Fact]
    public async Task CreateItem_ValidInputButListIdDoNotExists_ReturnsNotFound()
    {
        await SetTokenAsync();
        var listId = Guid.NewGuid();
        _validRequest.ListId = listId;


        var (rsp, _) = await App.Client.POSTAsync<CreateToDoItemEndpoint, Request, ProblemDetails>(_validRequest);


        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
    }
}
