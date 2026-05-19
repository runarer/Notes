
using Microsoft.EntityFrameworkCore;
using NotesWeb.Data.Interfaces;
using NotesWeb.Entities;

namespace NotesWeb.Data;

public class NoteBoardDBContext(DbContextOptions<NoteBoardDBContext> options) : DbContext(options),
    IUserAccess,
    IToDoListAccess,
    IToDoItemAccess
{
    public DbSet<User> Users { get; set; }
    public DbSet<ToDoItem> ToDoItems { get; set; }
    public DbSet<ToDoList> ToDoLists { get; set; }



    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     // Sets guid properties to be generated on add using EF's built-in sequential generator
    //     modelBuilder.Entity<User>()
    //         .Property(e => e.UserId)
    //         .ValueGeneratedOnAdd() // Tells EF to expect a value on insert
    //         .HasValueGenerator<SequentialGuidValueGenerator>(); // Use EF's built-in generator

    //     modelBuilder.Entity<ToDoItem>()
    //     .Property(e => e.ItemId)
    //     .ValueGeneratedOnAdd()
    //     .HasValueGenerator<SequentialGuidValueGenerator>();

    //     modelBuilder.Entity<ToDoList>()
    //     .Property(e => e.ListId)
    //     .ValueGeneratedOnAdd()
    //     .HasValueGenerator<SequentialGuidValueGenerator>();
    // }

    //* Interface implementasions, these can be splitt into partial classes or its own classes
    //* with this class injected.

    // Note TryFind methods need to pass lists for cancellationToken to work.

    //* User access */
    public async Task<bool> UsernameTakenAsync(string username, CancellationToken cancellationToken) =>
        await Users.AnyAsync(user => user.Username == username, cancellationToken);

    public async Task<bool> EmailTakenAsync(string email, CancellationToken cancellationToken) =>
        await Users.AnyAsync(user => user.Email == email, cancellationToken);

    public async Task<User?> TryFindUserByEmailAsync(string email, CancellationToken cancellationToken) =>
        await Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

    public async Task AddUserAsync(User user, CancellationToken cancellationToken) =>
        await Users.AddAsync(user, cancellationToken);

    public async Task<User?> TryFindUserByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await Users.FindAsync([id], cancellationToken);

    //* List Access */
    public async Task<ToDoList?> TryFindToDoListByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await ToDoLists.FirstOrDefaultAsync(list => list.Id == id, cancellationToken);

    public async Task AddToDoListAsync(ToDoList list, CancellationToken cancellationToken) =>
        await ToDoLists.AddAsync(list, cancellationToken);

    //* Item Access */
    public async Task<ToDoItem?> TryFindToDoItemByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await ToDoItems.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);


    public async Task AddToDoItemAsync(ToDoList list, CancellationToken cancellationToken) =>
        await ToDoLists.AddAsync(list, cancellationToken);
}
