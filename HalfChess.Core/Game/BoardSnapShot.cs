using HalfChess.Core.Domain;
using HalfChess.Core.Interfaces;

namespace HalfChess.Core.Game
{
    public interface IReadOnlyBoard
    {
        Piece? GetPiece(Position position);
        bool IsOccupied(Position position);
        bool IsCheck(PieceColor color);
        IEnumerable<Position> GetAllPiecePositions(PieceColor color);
    }

    public class BoardSnapshot : IReadOnlyBoard
    {
        private readonly IBoard _board;

        internal BoardSnapshot(IBoard board)
        {
            _board = board;
        }

        public Piece? GetPiece(Position position) => _board.GetPiece(position);
        public bool IsOccupied(Position position) => _board.IsOccupied(position);
        public bool IsCheck(PieceColor color) => _board.IsCheck(color);
        public IEnumerable<Position> GetAllPiecePositions(PieceColor color) =>
            _board.GetAllPiecePositions(color);
    }
}