using TodoFrontend.Models;

namespace TodoFrontend.Clients;

public class ItemClient
{
    private readonly List<TodoItem> _items = [
        new(){ Id = 0, Title = "Fix tile in basement", Due = DateTimeOffset.UtcNow.AddDays(7),Description="", ListId=0, Order=0},
        new(){ Id = 0, Title = "Fix tile in basement", Due = DateTimeOffset.UtcNow.AddDays(7),Description="", ListId=0, Order=0},
        new(){ Id = 0, Title = "Fix tile in basement", Due = DateTimeOffset.UtcNow.AddDays(7),Description="", ListId=0, Order=0},
        new(){ Id = 0, Title = "Fix tile in basement", Due = DateTimeOffset.UtcNow.AddDays(7),Description="", ListId=0, Order=0},
        new(){ Id = 0, Title = "Fix tile in basement", Due = DateTimeOffset.UtcNow.AddDays(7),Description="", ListId=0, Order=0},
    ];
    public async Task<List<TodoItem>> GetItemsAsync()
    {
        return _items;
    }
    // When connected to api, add filtering function here
    public async Task<List<TodoItem>> GetItemsAsync(TodoList list)
    {
        return list.Items;
    }

    public async Task<TodoItem> CreateItemAsync(string title, DateTimeOffset due, string description)
    {
        var newItem = new TodoItem()
        {
            Title = title,
            Description = description,
            Due = due,
            ListId = null,
            Order = _items.Where(item => item.ListId is null)
                          .Select(item => item.Order)
                          .DefaultIfEmpty(0)
                          .Max()
        };
        _items.Add(newItem);
        return newItem;
    }

    public async Task<TodoItem> EditItemAsync(
        TodoItem item,
        string? title = null,
        string? description = null,
        DateTimeOffset? due = null,
        bool? completed = null,
        TodoList? list = null,
        int? order = null)
    {
        if (title is not null)
            item.Title = title;
        if (description is not null)
            item.Description = description;
        if (due.HasValue)
            item.Due = due.Value;
        if (completed.HasValue)
            item.Completed = completed.Value;
        if (list is not null)
            item.ListId = list.Id;
        if (order.HasValue)
            item.Order = order.Value;
        return item;
    }
}