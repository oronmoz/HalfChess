using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace HalfChess.Client.Services
{
    public class GameService : IGameService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GameService> _logger;
        private const string BaseUrl = "api/game";

        public GameService(HttpClient httpClient, ILogger<GameService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<GameStartResponse> StartGame(GameStartRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/start", request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<GameStartResponse>()
                    ?? throw new InvalidOperationException("Invalid response from server");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error starting game: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting game: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<MoveResponse> MakeMove(Guid gameId, MoveRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{gameId}/move", request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<MoveResponse>()
                    ?? throw new InvalidOperationException("Invalid response from server");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error making move for game {GameId}: {Message}", gameId, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making move for game {GameId}: {Message}", gameId, ex.Message);
                throw;
            }
        }

        public async Task<GameStateResponse> GetGameState(Guid gameId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{gameId}/state");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<GameStateResponse>()
                    ?? throw new InvalidOperationException("Invalid response from server");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error getting state for game {GameId}: {Message}", gameId, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting state for game {GameId}: {Message}", gameId, ex.Message);
                throw;
            }
        }

        public async Task ResignGame(Guid gameId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{BaseUrl}/{gameId}/resign", null);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error resigning game {GameId}: {Message}", gameId, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resigning game {GameId}: {Message}", gameId, ex.Message);
                throw;
            }
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<T>();
                if (result == null)
                    throw new InvalidOperationException("Received null response from server");
                return result;
            }
            catch (JsonException ex)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError(ex, "Error deserializing response: {Content}", content);
                throw new InvalidOperationException("Error processing server response", ex);
            }
        }
    }

    public static class GameServiceExtensions
    {
        public static IServiceCollection AddGameService(this IServiceCollection services, string baseUrl)
        {
            services.AddHttpClient<IGameService, GameService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            return services;
        }
    }
}