// Several endpoints return this response, so it is shared across the ToDoItems features.

namespace NotesWeb.Features.ToDo.ToDoItems;

public class ItemResponse
{
    public Guid ItemId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTimeOffset? Due { get; set; }
    public bool Completed { get; set; }
    public Guid ParentListId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}