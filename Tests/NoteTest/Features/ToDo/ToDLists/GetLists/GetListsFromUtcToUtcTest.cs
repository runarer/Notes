
using System.Net;
using NotesWeb.Features.ToDo.ToDoLists.GetLists;

namespace NoteTest.Features.ToDo.ToDLists.GetLists;

// Test is on its own to since it writes to the DB and makes calles to check the writen content
public class GetListsFromUtcToUtcTest(App App, LoginState State) : LoggedinTests(App, State)
{

    [Fact]
    public async Task GetLists_GetListOfListsWithTimeSearch_ReturnsListWithinTimeMatch()
    {
        List<string> lists = ["Test list 3 days ago", "Test list 4 day ago"];
        await SetTokenAsync();

        var from = App.FakeTime.GetUtcNow().AddHours(-1);
        await PostList(lists[0]);
        var to = App.FakeTime.GetUtcNow().AddHours(2);

        App.FakeTime.Advance(TimeSpan.FromDays(3));
        await PostList(lists[1]);


        Assert.NotEqual(to, from);


        var (rsp, res) = await App.Client.GETAsync<GetListsEndpoint, Request, Response>(new Request
        {
            FromUtc = from,
            ToUtc = to
        });

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.Single(res.Lists);
        Assert.Equal(lists[0], res.Lists[0].Title);

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
}
