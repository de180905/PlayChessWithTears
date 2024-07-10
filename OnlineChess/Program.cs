using OnlineChess.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineChess.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("OnlineChessDbContextConnection") ?? throw new InvalidOperationException("Connection string 'OnlineChessDbContextConnection' not found.");

builder.Services.AddDbContextFactory<OnlineChessDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<OnlineChessDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<OnlineChessUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<OnlineChessDbContext>();

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ChessHubExtensions>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = context =>
    {
        context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
        context.Context.Response.Headers.Add("Expires", "-1");
    }
});

app.UseMiddleware<IsInRoomMiddleware>();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<ChessHub>("/hubs/chessHub");
app.Run();
