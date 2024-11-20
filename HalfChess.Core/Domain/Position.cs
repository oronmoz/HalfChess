namespace HalfChess.Core.Domain
{
    public readonly struct Position : IEquatable<Position>
    {
        public int File { get; }  // 0-3 for half board (e-h)
        public int Rank { get; }  // 0-7 (1-8)

        public Position(int file, int rank)
        {
            if (file < 0 || file > 3)
                throw new ArgumentOutOfRangeException(nameof(file), "File must be between 0 and 3");
            if (rank < 0 || rank > 7)
                throw new ArgumentOutOfRangeException(nameof(rank), "Rank must be between 0 and 7");

            File = file;
            Rank = rank;
        }

        // Convert chess notation (e.g., "e4") to Position
        public static Position FromAlgebraic(string algebraic)
        {
            if (algebraic.Length != 2)
                throw new ArgumentException("Invalid algebraic notation", nameof(algebraic));

            char file = char.ToLowerInvariant(algebraic[0]);
            if (file < 'e' || file > 'h')
                throw new ArgumentException("File must be between e and h", nameof(algebraic));

            if (!int.TryParse(algebraic[1].ToString(), out int rank) || rank < 1 || rank > 8)
                throw new ArgumentException("Rank must be between 1 and 8", nameof(algebraic));

            return new Position(file - 'e', rank - 1);
        }

        // Convert Position to chess notation (e.g., "e4")
        public string ToAlgebraic()
        {
            char file = (char)('e' + File);
            int rank = Rank + 1;
            return $"{file}{rank}";
        }

        public bool Equals(Position other) => File == other.File && Rank == other.Rank;
        public override bool Equals(object? obj) => obj is Position other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(File, Rank);
        public static bool operator ==(Position left, Position right) => left.Equals(right);
        public static bool operator !=(Position left, Position right) => !left.Equals(right);
        public override string ToString() => ToAlgebraic();
    }
}