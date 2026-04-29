
using System.Net;

using NotesWeb.Features.ToDo.ToDoLists.RenameList;

namespace NoteTest.Features.ToDo.ToDLists.RenameList;



public class RenameListTests(App App, LoginState State) : LoggedinTests(App, State)
{
    [Fact]
    public async Task RenameList_ListExistsAndAValidInput_ListRenamed()
    {
        // Create a list then create a request for renaming
        await SetTokenAsync();
        var listId = await CreateAListAsync("Test List rename valid");
        var validRequest = new Request
        {
            Title = "Renamed List with valid title",
            ListId = listId
        };

        var fakeTime = App.FakeTime.GetUtcNow();

        var (rsp, res) = await App.Client.PATCHAsync<RenameListEndpoint, Request, Response>(validRequest);

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Equal(validRequest.Title, res.Title);
        Assert.NotEqual(default, res.Id);
        Assert.Equal(fakeTime, res.UpdatedAtUtc);
    }

    [Fact]
    public async Task RenameList_TitleToShort_GetProblemDetailsWithErrorMessage()
    {
        // Create a list then create a request for renaming
        await SetTokenAsync();
        var invalidRequest = new Request
        {
            Title = "2s",
            ListId = Guid.NewGuid()
        };

        var expected = new[] {
            ("title", "Title is to short")};



        var (rsp, res) = await App.Client.PATCHAsync<RenameListEndpoint, Request, ProblemDetails>(invalidRequest);



        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task RenameList_TitleToLong_GetProblemDetailsWithErrorMessage()
    {
        // Create a list then create a request for renaming
        await SetTokenAsync();
        var invalidRequest = new Request
        {
            Title = "A way to long title...................................................................................",
            ListId = Guid.NewGuid()
        };

        var expected = new[] {
            ("title", "Title is to long")};



        var (rsp, res) = await App.Client.PATCHAsync<RenameListEndpoint, Request, ProblemDetails>(invalidRequest);



        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task RenameList_NoTitleProvided_GetProblemDetailsWithErrorMessage()
    {
        // Create a list then create a request for renaming
        await SetTokenAsync();
        var invalidRequest = new Request
        {
            Title = "",
            ListId = Guid.NewGuid()
        };

        var expected = new[] {
            ("title", "You need to provide a title")};



        var (rsp, res) = await App.Client.PATCHAsync<RenameListEndpoint, Request, ProblemDetails>(invalidRequest);



        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task RenameList_TitleValidButListIdIsWrong_Return404()
    {
        // Create a list then create a request for renaming
        await SetTokenAsync();
        var invalidRequest = new Request
        {
            Title = "A valid list name",
            ListId = Guid.NewGuid()
        };



        var (rsp, _) = await App.Client.PATCHAsync<RenameListEndpoint, Request, ProblemDetails>(invalidRequest);



        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);
    }
}