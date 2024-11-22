using HalfChess.Client.Data;
using HalfChess.Client.Infrastructure;
using HalfChess.Client.Repositories;
using HalfChess.Client.Services;
using HalfChess.Client.ViewModels;
using HalfChess.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;

namespace HalfChess.Client
{
    public partial class App : Application
    {
        private IServiceProvider ServiceProvider { get; set; }
        private IConfiguration Configuration { get; set; }
        public static int CurrentPlayerId { get; private set; } = 1;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)  // Use AppContext.BaseDirectory instead
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  // Make optional for now
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var services = new ServiceCollection();
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            // Initialize MainWindow
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Add configuration
            services.AddSingleton(Configuration);

            // Register HttpClient services
            services.AddHttpClient<IGameService, GameService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/api/");
            });

            // Register DbContext
            services.AddDbContext<GameReplayContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("GameReplayDb") ?? "Data Source=GameReplay.db",
                    b => b.MigrationsAssembly("HalfChess.Client")));

            // Register services
            services.AddSingleton<GameViewModel>();
            services.AddScoped<IGameReplayRepository, GameReplayRepository>();

            // Register MainWindow
            services.AddTransient<MainWindow>();

            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });
        }

        public static IServiceProvider GetServiceProvider()
        {
            var app = Current as App;
            return app?.ServiceProvider ?? throw new InvalidOperationException("ServiceProvider not initialized");
        }
    }
}