using NotesWeb.Entities;

namespace NotesWeb.Features.ToDo.ToDoItems;

// Mapper for endpoints that return an ItemResponse, but have different request types. 
public class ItemMapper<TRequest> : Mapper<TRequest, ItemResponse, ToDoItem>
    where TRequest : notnull
{
    public override ItemResponse FromEntity(ToDoItem e) => new()
    {
        ItemId = e.Id,
        Title = e.Title,
        Completed = e.Completed,
        ParentListId = e.ParentListId,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc,
        Description = e.Description,
        Due = e.Due,
    };
}