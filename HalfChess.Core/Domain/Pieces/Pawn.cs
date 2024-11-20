using HalfChess.Core.Interfaces;

namespace HalfChess.Core.Domain.Pieces
{
    public class Pawn : Piece
    {
        private bool _hasMoved;

        public Pawn(PieceColor color, Position position) 
            : base(color, PieceType.Pawn, position)
        {
        }

        public override IEnumerable<Position> GetLegalMoves(IBoard board)
        {
            var moves = new List<Position>();
            int direction = Color == PieceColor.White ? 1 : -1;

            // Forward move
            var forwardPos = new Position(Position.File, Position.Rank + direction);
            if (forwardPos.Rank >= 0 && forwardPos.Rank < 8 && !board.IsOccupied(forwardPos))
            {
                moves.Add(forwardPos);

                // Double move from starting position
                if (!_hasMoved)
                {
                    var doublePos = new Position(Position.File, Position.Rank + (2 * direction));
                    if (!board.IsOccupied(doublePos))
                    {
                        moves.Add(doublePos);
                    }
                }
            }

            // Captures
            var captureFiles = new[] { Position.File - 1, Position.File + 1 };
            foreach (var file in captureFiles)
            {
                if (file >= 0 && file < 4)  // Check within half-board bounds
                {
                    var capturePos = new Position(file, Position.Rank + direction);
                    var targetPiece = board.GetPiece(capturePos);
                    if (targetPiece != null && targetPiece.Color != Color)
                    {
                        moves.Add(capturePos);
                    }
                }
            }

            // Special Half Chess rule: Horizontal movement
            var horizontalFiles = new[] { Position.File - 1, Position.File + 1 };
            foreach (var file in horizontalFiles)
            {
                if (file >= 0 && file < 4)  // Check within half-board bounds
                {
                    var horizontalPos = new Position(file, Position.Rank);
                    if (!board.IsOccupied(horizontalPos))  // Can only move horizontally to empty squares
                    {
                        moves.Add(horizontalPos);
                    }
                }
            }

            return moves;
        }

        public override void MoveTo(Position newPosition)
        {
            base.MoveTo(newPosition);
            _hasMoved = true;
        }

        public override Piece Clone()
        {
            var clone = new Pawn(Color, Position);
            if (_hasMoved)
                clone.MoveTo(Position);  // This will set _hasMoved to true
            return clone;
        }
    }
}