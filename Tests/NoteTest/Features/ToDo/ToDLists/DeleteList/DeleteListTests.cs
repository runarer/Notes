
using System.Net;
using NotesWeb.Features.ToDo.ToDoLists.DeleteList;
namespace NoteTest.Features.ToDo.ToDLists.DeleteList;

public class DeleteListTests(App App, LoginState State) : LoggedinTests(App, State)
{


    [Fact]
    public async Task DeleteList_ListDoesExists_GetNoContentAndListIsGone()
    {
        // SignUp user
        await SetTokenAsync();
        var listId = await CreateAListAsync("Test List to delete");


        var rsp = await App.Client.DELETEAsync<DeleteListEndpoint, Request>(new Request
        {
            ListId = listId
        });



        Assert.Equal(HttpStatusCode.NoContent, rsp.StatusCode);
        var (rspDeleted, _) = await App.Client.GETAsync<
            NotesWeb.Features.ToDo.ToDoLists.GetList.GetListEndpoint,
            NotesWeb.Features.ToDo.ToDoLists.GetList.Request,
            NotesWeb.Features.ToDo.ToDoLists.GetList.Response>(
                new NotesWeb.Features.ToDo.ToDoLists.GetList.Request
                {
                    ListId = listId
                });
        Assert.Equal(HttpStatusCode.NotFound, rspDeleted.StatusCode);


    }

    [Fact]
    public async Task DeleteList_ListDoesNotExists_GetNoContent()
    {
        await SetTokenAsync();

        var rsp = await App.Client.DELETEAsync<DeleteListEndpoint, Request>(new Request
        {
            ListId = Guid.NewGuid()
        });

        Assert.Equal(HttpStatusCode.NoContent, rsp.StatusCode);
    }
}
