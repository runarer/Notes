
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NotesWeb.Entities;

namespace NotesWeb.Data;

public class NoteBoardDBContext(DbContextOptions<NoteBoardDBContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ToDoItem> ToDoItems { get; set; }
    public DbSet<ToDoList> ToDoLists { get; set; }
    // public DbSet<Notes> Notes {get; set;}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Sets guid properties to be generated on add using EF's built-in sequential generator
        modelBuilder.Entity<User>()
            .Property(e => e.UserId)
            .ValueGeneratedOnAdd() // Tells EF to expect a value on insert
            .HasValueGenerator<SequentialGuidValueGenerator>(); // Use EF's built-in generator

        modelBuilder.Entity<ToDoItem>()
        .Property(e => e.ItemId)
        .ValueGeneratedOnAdd()
        .HasValueGenerator<SequentialGuidValueGenerator>();

        modelBuilder.Entity<ToDoList>()
        .Property(e => e.ListId)
        .ValueGeneratedOnAdd()
        .HasValueGenerator<SequentialGuidValueGenerator>();
    }

}
