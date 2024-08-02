using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoListExam.ToDoList;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Підключення БД
string connStr = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string was not provided!");
builder.Services.AddDbContext<ToDoListContext>(options => options.UseSqlServer(connStr));

// Підключення ідентифікації користувачів
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<ToDoListContext>();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = "1"; //Enter Google ClientId
        options.ClientSecret = "1"; //Enter Google ClientSecret
    })
    .AddGitHub(githubOptions =>
    {
        githubOptions.ClientId = "1"; //Enter GitHub ClientId
        githubOptions.ClientSecret = "1"; //Enter GitHub ClientSecret
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Використання аутентифікації та авторизації
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ToDoList}/{action=Index}/{id?}");
app.Run();