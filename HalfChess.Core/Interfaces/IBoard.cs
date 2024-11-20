using HalfChess.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfChess.Core.Interfaces
{
    public interface IBoard
    {
        Piece? GetPiece(Position position);
        bool IsOccupied(Position position);
        bool MovePiece(Position from, Position to);
        IEnumerable<Position> GetAllPiecePositions(PieceColor color);
        bool IsCheck(PieceColor color);
        bool IsCheckmate(PieceColor color);
        bool IsStalemate(PieceColor color);
        IBoard Clone();
    }
}