using HalfChess.Core.Domain.Pieces;
using HalfChess.Core.Interfaces;

namespace HalfChess.Core.Domain
{
    public class Board : IBoard
    {
        private readonly Piece?[,] _squares;
        private readonly List<Move> _moveHistory;

        public Board()
        {
            _squares = new Piece?[8, 4];  // 8 ranks, 4 files (half chess board)
            _moveHistory = new List<Move>();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            // Initialize white pieces (bottom)
            _squares[0, 0] = new Rook(PieceColor.White, new Position(0, 0));
            _squares[0, 1] = new Knight(PieceColor.White, new Position(1, 0));
            _squares[0, 2] = new Bishop(PieceColor.White, new Position(2, 0));
            _squares[0, 3] = new King(PieceColor.White, new Position(3, 0));

            // White pawns
            for (int file = 0; file < 4; file++)
            {
                _squares[1, file] = new Pawn(PieceColor.White, new Position(file, 1));
            }

            // Initialize black pieces (top)
            _squares[7, 0] = new Rook(PieceColor.Black, new Position(0, 7));
            _squares[7, 1] = new Knight(PieceColor.Black, new Position(1, 7));
            _squares[7, 2] = new Bishop(PieceColor.Black, new Position(2, 7));
            _squares[7, 3] = new King(PieceColor.Black, new Position(3, 7));

            // Black pawns
            for (int file = 0; file < 4; file++)
            {
                _squares[6, file] = new Pawn(PieceColor.Black, new Position(file, 6));
            }
        }

        public Piece? GetPiece(Position position) =>
            _squares[position.Rank, position.File];

        public bool IsOccupied(Position position) =>
            GetPiece(position) != null;

        public bool MovePiece(Position from, Position to)
        {
            var piece = GetPiece(from);
            if (piece == null) return false;

            var targetPiece = GetPiece(to);
            bool isCapture = targetPiece != null;

            // Record the move
            _moveHistory.Add(new Move(from, to, piece, isCapture));

            // Update the board
            _squares[to.Rank, to.File] = piece;
            _squares[from.Rank, from.File] = null;
            piece.MoveTo(to);

            return true;
        }

        public IEnumerable<Position> GetAllPiecePositions(PieceColor color)
        {
            var positions = new List<Position>();
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 4; file++)
                {
                    var piece = _squares[rank, file];
                    if (piece?.Color == color)
                    {
                        positions.Add(new Position(file, rank));
                    }
                }
            }
            return positions;
        }

        public bool IsCheck(PieceColor color)
        {
            // Find the king
            Position? kingPosition = null;
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 4; file++)
                {
                    var piece = _squares[rank, file];
                    if (piece?.Type == PieceType.King && piece.Color == color)
                    {
                        kingPosition = new Position(file, rank);
                        break;
                    }
                }
                if (kingPosition.HasValue) break;
            }

            if (!kingPosition.HasValue)
                throw new InvalidOperationException("King not found on board");

            // Check if any opponent's piece can capture the king
            var oppositeColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            foreach (var position in GetAllPiecePositions(oppositeColor))
            {
                var piece = GetPiece(position);
                if (piece!.GetLegalMoves(this).Contains(kingPosition.Value))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCheckmate(PieceColor color)
        {
            if (!IsCheck(color)) return false;

            // Try all possible moves for all pieces
            foreach (var position in GetAllPiecePositions(color))
            {
                var piece = GetPiece(position);
                foreach (var move in piece!.GetLegalMoves(this))
                {
                    // Try the move
                    var tempBoard = Clone();
                    tempBoard.MovePiece(position, move);

                    // If this move gets us out of check, it's not checkmate
                    if (!tempBoard.IsCheck(color))
                    {
                        return false;
                    }
                }
            }

            // If we get here, no move can get us out of check
            return true;
        }

        public bool IsStalemate(PieceColor color)
        {
            if (IsCheck(color)) return false;

            // Check if the player has any legal moves
            foreach (var position in GetAllPiecePositions(color))
            {
                var piece = GetPiece(position);
                if (piece!.GetLegalMoves(this).Any())
                {
                    return false;
                }
            }

            // If we get here, the player has no legal moves but is not in check
            return true;
        }

        public IBoard Clone()
        {
            var clone = new Board();
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 4; file++)
                {
                    if (_squares[rank, file] != null)
                    {
                        var piece = _squares[rank, file]!;
                        clone._squares[rank, file] = piece.Clone();
                    }
                }
            }

            // Clone move history
            foreach (var move in _moveHistory)
            {
                clone._moveHistory.Add(new Move(
                    move.From,
                    move.To,
                    move.Piece.Clone(),
                    move.IsCapture
                ));
            }

            return clone;
        }
    }
}
