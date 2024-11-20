using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HalfChess.Client.Data;
using HalfChess.Client.Repositories;
using HalfChess.Client.Services;
using HalfChess.Client.ViewModels;
using HalfChess.Client.Views;
using HalfChess.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HalfChess.Client
{
    public partial class MainWindow : Window
    {
        private readonly GameViewModel _gameViewModel;
        private readonly IGameService _gameService;
        private readonly IServiceProvider _serviceProvider;
        private readonly SolidColorBrush lightSquareColor = new(Colors.White);
        private readonly SolidColorBrush darkSquareColor = new(Colors.LightGray);

        public MainWindow()
        {
            InitializeComponent();

            // Setup services
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            // Initialize services
            _gameService = _serviceProvider.GetRequiredService<IGameService>();
            _gameViewModel = _serviceProvider.GetRequiredService<GameViewModel>();

            // Set DataContext
            DataContext = _gameViewModel;

            InitializeChessBoard();
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

        private void InitializeChessBoard()
        {
            // Create the chess board squares
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    var square = new Border
                    {
                        Background = (row + col) % 2 == 0 ? lightSquareColor : darkSquareColor
                    };

                    // Add coordinate labels
                    if (row == 0)
                    {
                        var label = new TextBlock
                        {
                            Text = ((char)('e' + col)).ToString(),
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(0, 2, 2, 0)
                        };
                        square.Child = label;
                    }

                    if (col == 0)
                    {
                        var label = new TextBlock
                        {
                            Text = (8 - row).ToString(),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(2, 2, 0, 0)
                        };
                        square.Child = label;
                    }

                    Grid.SetRow(square, row);
                    Grid.SetColumn(square, col);
                    ChessBoard.Children.Add(square);
                }
            }
        }

        private async void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Starting new game...";
                StartGameButton.IsEnabled = false;

                await _gameViewModel.StartGameCommand.ExecuteAsync(null);

                StatusText.Text = "Game started!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error starting game: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StatusText.Text = "Error starting game";
                StartGameButton.IsEnabled = true;
            }
        }

        private void LoadReplayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var replayRepository = _serviceProvider.GetRequiredService<IGameReplayRepository>();
                var dialog = new GameReplayDialog(replayRepository);

                if (dialog.ShowDialog() == true && dialog.SelectedGame != null)
                {
                    StatusText.Text = $"Loading replay of game {dialog.SelectedGame.GameId}...";
                    LoadReplayButton.IsEnabled = false;

                    // TODO: Implement replay loading in GameViewModel
                    StatusText.Text = "Replay loaded";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading replay: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StatusText.Text = "Error loading replay";
            }
            finally
            {
                LoadReplayButton.IsEnabled = true;
            }
        }
    }
}