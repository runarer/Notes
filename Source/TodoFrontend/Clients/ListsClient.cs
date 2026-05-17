using TodoFrontend.Models;

namespace TodoFrontend.Clients;

public class ListsClient
{
    private readonly List<TodoList> _lists = [
            new (){  Id = 0, Title = "House repairs", },
            new (){  Id = 1, Title = "Pet care", },
            new (){  Id = 2, Title = "Shopping", },
            new (){  Id = 3, Title = "Gardening", },
            new (){  Id = 4, Title = "Summer planning", },
        ];
    public async Task<List<TodoListInfo>> GetListsAsync()
    {
        return [.. _lists.Select(list => (TodoListInfo)list)];
    }

    public async Task<List<TodoList>> GetListsWithItemsAsync()
    {
        return _lists;
    }

    // Add filtering after api connection

    public async Task RemoveListAsync(TodoListInfo list) => _lists.RemoveAll(l => l.Id == list.Id);

    public async Task<TodoList> CreateListAsync(string title)
    {
        if (string.IsNullOrEmpty(title))
            throw new ArgumentException("Need a title");

        var newList = new TodoList() { Id = _lists.Max(list => list.Id), Title = title };
        _lists.Add(newList);
        return newList;
    }

    public async Task<TodoList> RenameListAsync(TodoList list, string newTitle)
    {
        list.Title = newTitle;
        return list;
    }

    public async Task<TodoList?> GetListByIdAsync(int id)
    {
        return _lists.Find(list => list.Id == id);
    }

    public async Task<TodoList?> GetListWithItemsByIdAsync(int id)
    {
        return _lists.Find(list => list.Id == id);
    }

    public async Task AddItemToListAsync(TodoList list, TodoItem item)
    {
        item.ListId = list.Id;
        item.Order = list.Items.Max(item => item.Order) + 1;
        list.Items.Add(item);
    }
}