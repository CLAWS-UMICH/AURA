namespace UnityLabs.Slices.Games.Chess
{
    // Enums set as byte because they are intended to be synced over Normcore as bytes.
    public enum ChessPieceType : byte
    {
        Pawn = 0,
        Bishop = 1,
        Knight = 2,
        Rook = 3,
        Queen = 4,
        King = 5,
        None = 6
    }

    public enum ChessColor : byte
    {
        Black = 0,
        White = 1,
        None = 2
    }

    public enum ChessMoveType : byte
    {
        StandardMove = 0,
        CastlingMove = 1,
        EnPassantMove = 2,
        PromotionMove = 3
    }

    public enum ChessGameMode : byte
    {
        NotStarted = 0,
        HumanVsHuman = 1,
        HumanVsAI = 2,
        AIvsHuman = 3,
        AIvsAI = 4
    }

    public static class ChessTypeExtensions
    {
        public static ChessColor ComplimentaryColor(this ChessColor chessColor)
        {
            switch (chessColor)
            {
                default:
                case ChessColor.None:
                    return ChessColor.None;
                case ChessColor.White:
                    return ChessColor.Black;
                case ChessColor.Black:
                    return ChessColor.White;
            }
        }
    }
}
