
using Microsoft.EntityFrameworkCore;
using NotesWeb.Entities;

namespace NotesWeb.Data;

public class NoteBoardDBContext(DbContextOptions<NoteBoardDBContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    // public DbSet<ToDoItem> ToDoItems {get; set;}
    // public DbSet<ToDoList> ToDoList { get; set;}
    // public DbSet<Notes> Notes {get; set;}

}
