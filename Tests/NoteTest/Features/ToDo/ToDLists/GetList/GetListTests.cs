
using System.Net;
using NotesWeb.Features.ToDo.ToDoLists.GetList;

namespace NoteTest.Features.ToDo.ToDLists.GetList;

public class GetListTests(App App, LoginState State) : LoggedinTests(App, State)
{
    [Fact]
    public async Task GetList_ListDoesExists_GetListInformation()
    {
        // SignUp user
        await SetTokenAsync();
        var listTitle = "Test List to read back";
        var listId = await CreateAListAsync(listTitle);


        var (rsp, res) = await App.Client.GETAsync<GetListEndpoint, Request, Response>(
                new Request
                {
                    ListId = listId
                });


        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.Equal(listTitle, res.Title);
    }

    [Fact]
    public async Task GetList_ListDoesNotExists_GetNotFound()
    {
        // SignUp user
        await SetTokenAsync();


        var (rsp, _) = await App.Client.GETAsync<GetListEndpoint, Request, Response>(
                new Request
                {
                    ListId = Guid.NewGuid()
                });


        Assert.Equal(HttpStatusCode.NotFound, rsp.StatusCode);

    }
}