using System.Net;
using NotesWeb.Features.ToDo.ToDoLists.CreateList;

namespace NoteTest.Features.ToDo.ToDLists.CreateList;

public class CreateListTests(App App, LoginState State) : LoggedinTests(App, State)
{
    private readonly Request _validRequest = new()
    {
        Title = "A testing list"
    };

    [Fact]
    public async Task CreateList_WithValidInput_ListCreated()
    {
        // SignUp user
        await SetTokenAsync();
        var fakeTime = App.FakeTime.GetUtcNow();

        var (rsp, res) = await App.Client.POSTAsync<CreateListEndpoint, Request, Response>(_validRequest);

        Assert.Equal(HttpStatusCode.Created, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Equal(_validRequest.Title, res.Title);
        Assert.NotEqual(default, res.Id);
        Assert.Equal(fakeTime, res.CreatedAtUtc);
        Assert.Equal(res.CreatedAtUtc, res.UpdatedAtUtc);
    }

    [Fact]
    public async Task CreateList_TitleToShort_GEtPRoblemDetailsWithErrorMessage()
    {
        await SetTokenAsync();

        var invalidRequest = _validRequest;
        invalidRequest.Title = "t";

        var expected = new[] {
            ("title", "Title is to short")};



        var (rsp, res) = await App.Client.POSTAsync<CreateListEndpoint, Request, ProblemDetails>(invalidRequest);



        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task CreateList_TitleToLong_GEtPRoblemDetailsWithErrorMessage()
    {
        await SetTokenAsync();

        var invalidRequest = _validRequest;
        invalidRequest.Title = "A way to long title...................................................................................";

        var expected = new[] {
            ("title", "Title is to long")};



        var (rsp, res) = await App.Client.POSTAsync<CreateListEndpoint, Request, ProblemDetails>(invalidRequest);



        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }

    [Fact]
    public async Task CreateList_NoTitleProvided_GEtPRoblemDetailsWithErrorMessage()
    {
        await SetTokenAsync();

        var invalidRequest = _validRequest;
        invalidRequest.Title = "";

        var expected = new[] {
            ("title", "You need to provide a title")};



        var (rsp, res) = await App.Client.POSTAsync<CreateListEndpoint, Request, ProblemDetails>(invalidRequest);



        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));
    }
}
