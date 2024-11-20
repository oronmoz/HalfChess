using System;
using System.Windows;
using HalfChess.Core.Interfaces;
using System.Collections.ObjectModel;

namespace HalfChess.Client.Views
{
    public partial class GameReplayDialog : Window
    {
        private readonly IGameReplayRepository _replayRepository;
        public ObservableCollection<GameReplaySummary> Games { get; } = new();

        public GameReplaySummary? SelectedGame => GamesList.SelectedItem as GameReplaySummary;

        public GameReplayDialog(IGameReplayRepository replayRepository)
        {
            InitializeComponent();
            _replayRepository = replayRepository;
            LoadGamesList();
        }

        private async void LoadGamesList()
        {
            try
            {
                var games = await _replayRepository.GetGameReplays();
                Games.Clear();
                foreach (var game in games)
                {
                    Games.Add(game);
                }
                GamesList.ItemsSource = Games;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading games list: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (GamesList.SelectedItem != null)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(
                    "Please select a game to load.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}