// Class originally from UnityChessLib. Decided to keep this and ChessSquareUtil because they are pretty nice types for chess.

namespace UnityLabs.Slices.Games.Chess
{
    /// <summary>Representation of a square on a chessboard.</summary>
    public struct ChessSquare
    {
        public static readonly ChessSquare Zero = new ChessSquare(0, 0);
        public static readonly ChessSquare Invalid = new ChessSquare(-1, -1);
        public static readonly ChessSquare BlackGraveyard = new ChessSquare(-2, -1);
        public static readonly ChessSquare WhiteGraveyard = new ChessSquare(-2, -2);

        public readonly int File;
        public readonly int Rank;
        internal bool IsValid => 1 <= File && File <= 8 && 1 <= Rank && Rank <= 8;

        /// <summary>Creates a new Square instance.</summary>
        /// <param name="file">Column of the square.</param>
        /// <param name="rank">Row of the square.</param>
        public ChessSquare(int file, int rank)
        {
            File = file;
            Rank = rank;
        }

        internal ChessSquare(ChessSquare startPosition, int fileOffset, int rankOffset)
        {
            File = startPosition.File + fileOffset;
            Rank = startPosition.Rank + rankOffset;
        }

        //public static int FileRankAsIndex(int file, int rank) => (rank + 1) * 10 + file;

        public static bool operator ==(ChessSquare lhs, ChessSquare rhs) => lhs.File == rhs.File && lhs.Rank == rhs.Rank;
        public static bool operator !=(ChessSquare lhs, ChessSquare rhs) => !(lhs == rhs);

        public bool Equals(ChessSquare other) => File == other.File && Rank == other.Rank;

        public bool Equals(int file, int rank) => File == file && Rank == rank;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            return obj is ChessSquare other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (File * 397) ^ Rank;
            }
        }

        public override string ToString()
        {
            return IsValid ? ChessSquareUtil.SquareToString(this) : $"$InvalidSquare: {File} - {Rank}";
        }
    }
}
