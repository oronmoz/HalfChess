using HalfChess.Core.Game;
using HalfChess.Core.Interfaces;
using HalfChess.Data.Context;
using HalfChess.Data.Repositories;
using HalfChess.RazorWeb.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add database context
builder.Services.AddDbContext<HalfChessDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories and services
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddSingleton<IGameSessionManager, GameSessionManager>();

// Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<HalfChessDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Ensure we have a clean database
        if (app.Environment.IsDevelopment())
        {
            await context.Database.EnsureDeletedAsync();
            logger.LogInformation("Development database deleted");
        }

        // Apply migrations
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully");

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<GameHub>("/gameHub");

app.Run();