namespace HalfChess.Core.Domain
{
    public class Move
    {
        public Position From { get; }
        public Position To { get; }
        public Piece Piece { get; }
        public bool IsCapture { get; }
        public DateTime Timestamp { get; }

        public Move(Position from, Position to, Piece piece, bool isCapture)
        {
            From = from;
            To = to;
            Piece = piece;
            IsCapture = isCapture;
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString() =>
            $"{Piece.Color} {Piece.Type} {From}-{To}" + (IsCapture ? " captures" : "");
    }
}