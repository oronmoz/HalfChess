using HalfChess.Client.Data;
using HalfChess.Client.Repositories;
using HalfChess.Client.Services;
using HalfChess.Client.ViewModels;
using HalfChess.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace HalfChess.Client
{
    public partial class App : Application
    {
        private IServiceProvider ServiceProvider { get; set; }
        public static int CurrentPlayerId { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure services
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Initialize the database
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<GameReplayContext>();
                try
                {
                    // Check if database exists
                    if (!context.Database.GetPendingMigrations().Any())
                    {
                        // Apply any pending migrations
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Database initialization error: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // TODO: Load this from settings or login
            CurrentPlayerId = 1;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register HttpClient services
            services.AddHttpClient<IGameService, GameService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7001/api/");
            });

            // Register DbContext
            services.AddDbContext<GameReplayContext>(options =>
                options.UseSqlite(
                    "Data Source=GameReplay.db",
                    b => b.MigrationsAssembly("HalfChess.Client")));

            // Register repositories and view models
            services.AddSingleton<GameViewModel>();
            services.AddScoped<IGameReplayRepository, GameReplayRepository>();

            // Add logging
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
            });
        }

        public static IServiceProvider GetServiceProvider()
        {
            var app = Current as App;
            return app?.ServiceProvider ?? throw new InvalidOperationException("ServiceProvider not initialized");
        }
    }
}