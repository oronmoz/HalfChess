using HalfChess.Core.Interfaces;

namespace HalfChess.Core.Domain.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(PieceColor color, Position position)
            : base(color, PieceType.Bishop, position)
        {
        }

        public override IEnumerable<Position> GetLegalMoves(IBoard board)
        {
            var moves = new List<Position>();

            // All diagonal directions
            var directions = new[] { (1, 1), (1, -1), (-1, 1), (-1, -1) };

            foreach (var (fileOffset, rankOffset) in directions)
            {
                var currentFile = Position.File;
                var currentRank = Position.Rank;

                while (true)
                {
                    currentFile += fileOffset;
                    currentRank += rankOffset;

                    if (currentFile < 0 || currentFile >= 4 || currentRank < 0 || currentRank >= 8)
                        break;

                    var newPos = new Position(currentFile, currentRank);
                    var piece = board.GetPiece(newPos);

                    if (piece == null)
                    {
                        moves.Add(newPos);
                    }
                    else
                    {
                        if (piece.Color != Color)
                            moves.Add(newPos);
                        break;
                    }
                }
            }

            return moves;
        }

        public override Piece Clone() =>
    new Bishop(Color, Position);
    }
}