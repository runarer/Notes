
using System.Net;
using NotesWeb.Features.ToDo.ToDoLists.GetLists;

namespace NoteTest.Features.ToDo.ToDLists.GetLists;

public class GetListsTests(App App, LoginState State) : LoggedinTests(App, State)
{

    [Fact, Priority(0)]
    public async Task GetLists_GetListOfListsForUserWithNoList_AnEmptyListIsReturned()
    {
        var request = new Request { };
        await SetTokenAsync();



        var (rsp, res) = await App.Client.GETAsync<GetListsEndpoint, Request, Response>(request);



        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Empty(res.Lists);
    }


    [Fact, Priority(5)]
    public async Task GetLists_GetListOfListWithFromDateInTheFuture_ReturnsProblemDetails()
    {
        var expected = new[] {
            ("fromUtc", "Date 'from Utc' must be in the past!")};

        await SetTokenAsync();

        var (rsp, res) = await App.Client.GETAsync<GetListsEndpoint, Request, ProblemDetails>(new Request { FromUtc = App.FakeTime.GetUtcNow().AddDays(3) });

        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Single(res.Errors);
        Assert.Equivalent(expected, res.Errors.Select(e => (e.Name, e.Reason)));

    }

    [Fact, Priority(4)]
    public async Task GetLists_GetListOfListWithToDateBeforeFromDate_ReturnsProblemDetails()
    {
        var expected = new[] {
            ("fromUtc", "'from Utc' must be after 'To Utc'.")};

        await SetTokenAsync();

        var (rsp, res) = await App.Client.GETAsync<GetListsEndpoint, Request, ProblemDetails>(
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

    [Fact, Priority(2)]
    public async Task GetLists_GetListOfListsForUserWithThreeLists_AListWithThreeElementsIsReturned()
    {
        await SetTokenAsync();

        //Create some lists
        List<string> lists = ["Test list 1", "Test list 2", "Test list 3"];
        await PostLists(lists);

        // Get List of lists
        var (rsp, res) = await App.Client.GETAsync<GetListsEndpoint, Request, Response>(new Request { });



        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.Equivalent(lists, res.Lists.Select(e => e.Title));
    }

    [Fact, Priority(3)]
    public async Task GetLists_GetListOfListWithSearchQuery_ReturnsListWithMatch()
    {
        await SetTokenAsync();

        var searchTerm = "abcdefg";
        await PostLists(["test 1" + searchTerm, searchTerm + " test 2"]);

        var (rsp, res) = await App.Client.GETAsync<GetListsEndpoint, Request, Response>(new Request { Search = searchTerm });

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.NotNull(res);

        Assert.All(res.Lists, list => list.Title.Contains(searchTerm));
    }

    private async Task PostList(string list)
    {

        NotesWeb.Features.ToDo.ToDoLists.CreateList.Request listReq = new() { Title = list };
        var (rs, re) = await App.Client.POSTAsync<
            NotesWeb.Features.ToDo.ToDoLists.CreateList.CreateListEndpoint,
            NotesWeb.Features.ToDo.ToDoLists.CreateList.Request,
            NotesWeb.Features.ToDo.ToDoLists.CreateList.Response>(listReq);
        Assert.Equal(HttpStatusCode.Created, rs.StatusCode);
        Assert.NotNull(re);
    }

    private async Task PostLists(List<string> lists)
    {
        foreach (var list in lists)
            await PostList(list);
    }
}
