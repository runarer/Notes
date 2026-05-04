using System;

namespace NotesWeb.Features.ToDo.ToDoItems.GetItem;

public class Request : UserRequest
{
    public Guid ListId { get; set; }
    public Guid ItemId { get; set; }
}

public class Response
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public bool Completed { get; set; }
    public Guid ParentListId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}