using HalfChess.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HalfChess.RazorWeb.Services
{
    public class BoardManager
    {
        private readonly Dictionary<Position, Piece> _board = new();

        public BoardManager()
        {
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            // Initialize white pieces
            _board[new Position(0, 0)] = new Piece(PieceType.Rook, PieceColor.White);
            _board[new Position(1, 0)] = new Piece(PieceType.Knight, PieceColor.White);
            _board[new Position(2, 0)] = new Piece(PieceType.Bishop, PieceColor.White);
            _board[new Position(3, 0)] = new Piece(PieceType.King, PieceColor.White);

            // White pawns
            for (int file = 0; file < 4; file++)
            {
                _board[new Position(file, 1)] = new Piece(PieceType.Pawn, PieceColor.White);
            }

            // Initialize black pieces
            _board[new Position(0, 7)] = new Piece(PieceType.Rook, PieceColor.Black);
            _board[new Position(1, 7)] = new Piece(PieceType.Knight, PieceColor.Black);
            _board[new Position(2, 7)] = new Piece(PieceType.Bishop, PieceColor.Black);
            _board[new Position(3, 7)] = new Piece(PieceType.King, PieceColor.Black);

            // Black pawns
            for (int file = 0; file < 4; file++)
            {
                _board[new Position(file, 6)] = new Piece(PieceType.Pawn, PieceColor.Black);
            }
        }

        public void MakeMove(string from, string to)
        {
            var fromPos = Position.FromAlgebraic(from);
            var toPos = Position.FromAlgebraic(to);

            if (_board.TryGetValue(fromPos, out var piece))
            {
                _board.Remove(fromPos);
                _board[toPos] = piece;
            }
        }

        public string GetCurrentPosition()
        {
            var fen = new StringBuilder();

            for (int rank = 7; rank >= 0; rank--)
            {
                int emptySquares = 0;

                for (int file = 0; file < 4; file++)
                {
                    var pos = new Position(file, rank);
                    if (_board.TryGetValue(pos, out var piece))
                    {
                        if (emptySquares > 0)
                        {
                            fen.Append(emptySquares);
                            emptySquares = 0;
                        }
                        fen.Append(GetPieceFenSymbol(piece));
                    }
                    else
                    {
                        emptySquares++;
                    }
                }

                if (emptySquares > 0)
                {
                    fen.Append(emptySquares);
                }

                if (rank > 0)
                {
                    fen.Append('/');
                }
            }

            return fen.ToString();
        }

        public string GetInitialPosition() => "rnbk/pppp/4/4/4/4/PPPP/RNBK";

        private string GetPieceFenSymbol(Piece piece)
        {
            char symbol = piece.Type switch
            {
                PieceType.King => 'k',
                //PieceType.Queen => 'q',
                PieceType.Rook => 'r',
                PieceType.Bishop => 'b',
                PieceType.Knight => 'n',
                PieceType.Pawn => 'p',
                _ => throw new ArgumentException("Invalid piece type")
            };

            return piece.Color == PieceColor.White ? char.ToUpper(symbol).ToString() : symbol.ToString();
        }
    }

    public record Piece(PieceType Type, PieceColor Color);
}