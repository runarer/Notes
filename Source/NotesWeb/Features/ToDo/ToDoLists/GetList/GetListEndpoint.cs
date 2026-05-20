
using NotesWeb.Data;

namespace NotesWeb.Features.ToDo.ToDoLists.GetList;

public class GetListEndpoint(NoteBoardDBContext dbContext) : Endpoint<Request, Response, Mapper>
{
    private readonly NoteBoardDBContext _dbContext = dbContext;

    public override void Configure()
    {
        Get("/todo/{ListId}");
        PreProcessor<UserPreProcessor>();
        Roles("User");
        Claims("UserId");
        Summary(s =>
        {
            s.Summary = "Get List info";
            s.Description = "Get information of a list";
        });
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var list = await _dbContext.ToDoLists.FindAsync([request.ListId], ct);
        if (list is null)
            await Send.NotFoundAsync(ct);
        else if (list.UserId != request.UserId)
            await Send.ForbiddenAsync(ct);
        else
            await Send.OkAsync(Map.FromEntity(list), ct);

    }
}