using HalfChess.Client.Services;
using HalfChess.Client.Data;
using HalfChess.Client.ViewModels;
using HalfChess.Core.Interfaces;
using HalfChess.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HalfChess.Client.Repositories;
using System.Net.Http;

namespace HalfChess.Client.Infrastructure
{
    public static class DependencyConfig
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Logging
            services.AddLogging(builder => {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Local Database
            services.AddDbContext<GameReplayContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("GameReplayDb")));

            // Repositories
            services.AddScoped<IGameReplayRepository, GameReplayRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();

            // HTTP Services
            services.AddHttpClient<IGameService, GameService>(client => {
                client.BaseAddress = new Uri(configuration["ApiBaseUrl"] ?? "https://localhost:7039/api/");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            });

            // ViewModels
            services.AddSingleton<GameViewModel>();

            return services;
        }
    }
}