using HalfChess.Core.Game;
using HalfChess.Core.Interfaces;
using HalfChess.Data.Context;
using HalfChess.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HalfChess.WebAPI.Infrastructure
{
    public static class DependencyConfig
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<HalfChessDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Core Game Services
            services.AddSingleton<IGameSessionManager, GameSessionManager>();

            // Repositories
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();

            // API Specific
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });

            return services;
        }
    }
}