using HalfChess.Core.Interfaces;

namespace HalfChess.Core.Domain.Pieces
{
	public class Knight : Piece
	{
		public Knight(PieceColor color, Position position)
			: base(color, PieceType.Knight, position)
		{
		}

		public override IEnumerable<Position> GetLegalMoves(IBoard board)
		{
			var moves = new List<Position>();

			// All possible knight moves
			var jumps = new[]
			{
				(2, 1), (2, -1), (-2, 1), (-2, -1),
				(1, 2), (1, -2), (-1, 2), (-1, -2)
			};

			foreach (var (fileOffset, rankOffset) in jumps)
			{
				var newFile = Position.File + fileOffset;
				var newRank = Position.Rank + rankOffset;

				if (newFile >= 0 && newFile < 4 && newRank >= 0 && newRank < 8)
				{
					var newPos = new Position(newFile, newRank);
					if (IsValidMove(newPos, board))
					{
						moves.Add(newPos);
					}
				}
			}

			return moves;
		}

		public override Piece Clone() =>
			new Knight(Color, Position);
	}
}
