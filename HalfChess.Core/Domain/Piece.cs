using HalfChess.Core.Interfaces;

namespace HalfChess.Core.Domain
{
    public abstract class Piece
    {
        public PieceColor Color { get; }
        public PieceType Type { get; }
        public Position Position { get; protected set; }

        protected Piece(PieceColor color, PieceType type, Position position)
        {
            Color = color;
            Type = type;
            Position = position;
        }

        public abstract IEnumerable<Position> GetLegalMoves(IBoard board);

        protected bool IsValidMove(Position newPosition, IBoard board)
        {
            // Basic validation common to all pieces
            if (!newPosition.Equals(Position))  // Can't move to same position
            {
                var targetPiece = board.GetPiece(newPosition);
                return targetPiece == null || targetPiece.Color != Color;  // Empty square or enemy piece
            }
            return false;
        }

        public virtual bool CanMoveTo(Position target, IBoard board)
        {
            return GetLegalMoves(board).Contains(target);
        }

        public virtual void MoveTo(Position newPosition)
        {
            Position = newPosition;
        }

        public override string ToString()
        {
            return $"{Color} {Type} at {Position}";
        }

        public abstract Piece Clone();
    }
}