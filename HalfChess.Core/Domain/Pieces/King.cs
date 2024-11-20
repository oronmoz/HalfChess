using HalfChess.Core.Interfaces;

namespace HalfChess.Core.Domain.Pieces
{
    public class King : Piece
    {
        public King(PieceColor color, Position position)
            : base(color, PieceType.King, position)
        {
        }

        public override IEnumerable<Position> GetLegalMoves(IBoard board)
        {
            var moves = new List<Position>();

            // All possible king moves
            var directions = new[]
            {
                (1, 0), (1, 1), (0, 1), (-1, 1),
                (-1, 0), (-1, -1), (0, -1), (1, -1)
            };

            foreach (var (fileOffset, rankOffset) in directions)
            {
                var newFile = Position.File + fileOffset;
                var newRank = Position.Rank + rankOffset;

                if (newFile >= 0 && newFile < 4 && newRank >= 0 && newRank < 8)  // Check within half-board bounds
                {
                    var newPos = new Position(newFile, newRank);
                    if (IsValidMove(newPos, board))
                    {
                        // Additional check: Don't move into check
                        var tempBoard = board.Clone();  // We'll need to implement IBoard.Clone()
                        tempBoard.MovePiece(Position, newPos);
                        if (!tempBoard.IsCheck(Color))
                        {
                            moves.Add(newPos);
                        }
                    }
                }
            }

            return moves;
        }
        public override Piece Clone() =>
    new King(Color, Position);
    }
}