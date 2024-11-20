using CommunityToolkit.Mvvm.ComponentModel;
using HalfChess.Core.Domain;

namespace HalfChess.Client.ViewModels
{
    public class PieceViewModel : ObservableObject
    {
        private Position _position;

        public PieceType Type { get; }
        public PieceColor Color { get; }
        public string Image => $"/Assets/{Color}{Type}.png";

        public Position Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public PieceViewModel(Piece piece, Position position)
        {
            Type = piece.Type;
            Color = piece.Color;
            _position = position;
        }
    }
}