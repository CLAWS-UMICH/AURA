// Class originally from UnityChessLib

using System.Collections.Generic;

namespace UnityLabs.Slices.Games.Chess
{
    public static class ChessSquareUtil
    {
        public static readonly Dictionary<string, int> FileCharToIntMap = new Dictionary<string, int> {
            {"a", 1},
            {"b", 2},
            {"c", 3},
            {"d", 4},
            {"e", 5},
            {"f", 6},
            {"g", 7},
            {"h", 8}
        };

        public static readonly Dictionary<int, string> FileIntToCharMap = new Dictionary<int, string> {
            {1, "a"},
            {2, "b"},
            {3, "c"},
            {4, "d"},
            {5, "e"},
            {6, "f"},
            {7, "g"},
            {8, "h"}
        };

        public static string FileRankToSquareString(int file, int rank) => $"{FileIntToCharMap[file]}{rank}";

        public static string SquareToString(ChessSquare square) => FileRankToSquareString(square.File, square.Rank);

        public static ChessSquare StringToSquare(string squareText)
        {
            int file = FileCharToIntMap[squareText.Substring(0, 1)];
            int rank = int.Parse(squareText.Substring(1, 1));

            return new ChessSquare(file, rank);
        }

        public static ChessSquare BytesToSquare(byte fileByte, byte rankByte)
        {
            int file = fileByte;
            int rank = rankByte;

            return new ChessSquare(file, rank);
        }
    }
}
