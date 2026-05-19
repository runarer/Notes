
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

    //* User access */
    public async Task<bool> UsernameTakenAsync(string username) =>
        await Users.AnyAsync(user => user.Username == username);

    public async Task<bool> EmailTakenAsync(string email) =>
        await Users.AnyAsync(user => user.Email == email);

    public async Task CreateUserAsync(User user) =>
        await Users.AddAsync(user);

    public async Task<User?> TryFindByIdAsync(Guid id) =>
        await Users.FindAsync(id);

    public async Task<User?> TryFindUserByEmailAsync(string email) =>
        await Users.FirstOrDefaultAsync(user => user.Email == email);


    //* List Access */
    public async Task<ToDoList?> TryFindToDoListById(Guid id) =>
        await ToDoLists.FindAsync(id);

    //* Item Access */
    public async Task<ToDoItem?> TryFindToDoItemById(Guid id) =>
        await ToDoItems.FindAsync(id);
}
