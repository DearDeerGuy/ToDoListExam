using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ToDoListExam.ToDoList
{
    public class ToDoListContext: IdentityDbContext<IdentityUser>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<ToDoListItem> ToDoListItems { get; set; }
        public ToDoListContext(DbContextOptions<ToDoListContext> options) : base(options) { Database.EnsureCreated(); }
    }
}
