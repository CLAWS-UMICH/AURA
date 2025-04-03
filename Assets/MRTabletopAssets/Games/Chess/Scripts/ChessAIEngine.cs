using System;
using System.Collections.Generic;
using System.Linq;
using Chess;
using Chess.Game;
using UnityEngine;

namespace UnityLabs.Slices.Games.Chess
{
    public class ChessAIEngine : AbstractChessEngine
    {
        [SerializeField]
        AISettings m_AISettings;

        public override event Action<ChessSquare, ChessPieceType, ChessColor> onPieceRemoved;
        public override event Action<ChessSquare, ChessSquare, ChessPieceType, ChessColor> onPieceMoved;
        public override event Action<ChessSquare, ChessPieceType, ChessColor> onPieceAdded;

        public override event Action<Move> onAIMoveCompleted;

        public override bool lastMoveAppliedLocally => m_LastMoveAppliedLocally;

        public override bool isStaleMated => m_GameResult == Result.Stalemate;

        public override Result gameResult => m_GameResult;

        MoveGenerator m_MoveGenerator;
        Result m_GameResult = Result.NotStarted;
        Player m_WhitePlayer;
        Player m_BlackPlayer;
        Player m_PlayerToMove;
        ulong m_ZobristDebug;
        bool m_LastMoveAppliedLocally;
        Board m_Board;
        //The reason you have a seperate chess board for search is the search alghorithms adds and removes moves during
        //its AI calculaiton and you don't want this firing all the events off the main board.
        Board m_SearchBoard;

        public override ChessColor currentTurnSide
        {
            get
            {
                if (m_Board == null) return ChessColor.None;
                return m_Board.WhiteToMove ? ChessColor.White : ChessColor.Black;
            }
        }

        public override void NewGame(PlayerType whitePlayerType, PlayerType blackPlayerType, bool fromLocal)
        {
            if (m_Board != null)
            {
                m_Board.onPieceAdded -= PieceAdded;
                m_Board.onPieceMoved -= PieceMoved;
                m_Board.onPieceRemoved -= PieceRemoved;
            }
            m_Board = new Board();
            m_Board.onPieceAdded += PieceAdded;
            m_Board.onPieceMoved += PieceMoved;
            m_Board.onPieceRemoved += PieceRemoved;

            m_SearchBoard = new Board();
            m_MoveGenerator = new MoveGenerator();
            m_AISettings.diagnostics = new Search.SearchDiagnostics();

            m_Board.LoadStartPosition();
            m_SearchBoard.LoadStartPosition();

            CreatePlayer(ref m_WhitePlayer, whitePlayerType);
            CreatePlayer(ref m_BlackPlayer, blackPlayerType);

            m_LastMoveAppliedLocally = fromLocal;

            m_GameResult = Result.Playing;
            //Debug.Log(m_GameResult.ToString());

            NotifyPlayerToMove();
        }

        public override void StopGame()
        {
            // Should everything get cleared here?
            m_GameResult = Result.NotStarted;
        }

        public override void GetLegalTargetSquares(ChessSquare fromSquare, List<ChessSquare> validSquares)
        {
            Coord fromCoord = fromSquare.ToCoord();
            List<Move> moves = m_MoveGenerator.GenerateMoves(m_Board);
            for (int i = 0; i < moves.Count; i++)
            {
                if (moves[i].StartSquare == BoardRepresentation.IndexFromCoord(fromCoord))
                {
                    Coord endCoord = BoardRepresentation.CoordFromIndex(moves[i].TargetSquare);
                    validSquares.Add(endCoord.ToSquare());
                }
            }
        }

        public override Move TryGetLegalMove(ChessSquare fromSquare, ChessSquare toSquare)
        {
            Coord fromCoord = fromSquare.ToCoord();
            Coord toCoord = toSquare.ToCoord();
            List<Move> moves = m_MoveGenerator.GenerateMoves(m_Board);
            for (int i = 0; i < moves.Count; i++)
            {
                if (moves[i].StartSquare == BoardRepresentation.IndexFromCoord(fromCoord) &&
                    moves[i].TargetSquare == BoardRepresentation.IndexFromCoord(toCoord))
                {
                    return moves[i];
                }
            }

            return Move.InvalidMove;
        }

        public override void ApplyMove(Move move, bool fromLocal)
        {
            m_LastMoveAppliedLocally = fromLocal;
            OnMoveChosen(move);
        }

        public override void EndTurn(bool fromLocal)
        {
            m_LastMoveAppliedLocally = fromLocal;
            NotifyPlayerToMove();
        }

        public override bool IsPlayMated(ChessColor color)
        {
            if (color == ChessColor.Black)
            {
                return m_GameResult == Result.BlackIsMated;
            }

            if (color == ChessColor.White)
            {
                return m_GameResult == Result.WhiteIsMated;
            }

            return false;
        }

        public override void UndoMove(Move move)
        {
            m_Board.UnmakeMove(move);
            m_SearchBoard.UnmakeMove(move);
        }

        void PieceAdded(int square, int pieceType, int pieceColor)
        {
            Coord coord = BoardRepresentation.CoordFromIndex(square);
            onPieceAdded?.Invoke(coord.ToSquare(), IndexToChessPieceType(pieceType), IndexToColor(pieceColor));
        }

        void PieceMoved(int fromSquare, int toSquare, int pieceType, int pieceColor)
        {
            Coord fromCoord = BoardRepresentation.CoordFromIndex(fromSquare);
            Coord toCoord = BoardRepresentation.CoordFromIndex(toSquare);
            onPieceMoved?.Invoke(fromCoord.ToSquare(), toCoord.ToSquare(), IndexToChessPieceType(pieceType), IndexToColor(pieceColor));
        }

        void PieceRemoved(int square, int pieceType, int pieceColor)
        {
            Coord coord = BoardRepresentation.CoordFromIndex(square);
            onPieceRemoved?.Invoke(coord.ToSquare(), IndexToChessPieceType(pieceType), IndexToColor(pieceColor));
        }

        ChessColor IndexToColor(int index)
        {
            switch (index)
            {
                case Piece.Black:
                    return ChessColor.Black;
                case Piece.White:
                    return ChessColor.White;
                default:
                    return ChessColor.None;
            }
        }

        ChessPieceType IndexToChessPieceType(int index)
        {
            switch (index)
            {
                case Piece.Bishop:
                    return ChessPieceType.Bishop;
                case Piece.King:
                    return ChessPieceType.King;
                case Piece.Knight:
                    return ChessPieceType.Knight;
                case Piece.Pawn:
                    return ChessPieceType.Pawn;
                case Piece.Queen:
                    return ChessPieceType.Queen;
                case Piece.Rook:
                    return ChessPieceType.Rook;
                default:
                    return ChessPieceType.None;
            }
        }

        void Update()
        {
            if (m_GameResult == Result.Playing)
            {
                m_ZobristDebug = m_Board.ZobristKey;
                // LogAIDiagnostics();
                m_PlayerToMove.Update();
            }
        }

        void OnAIMoveChosen(Move move)
        {
            OnMoveChosen(move);
            onAIMoveCompleted?.Invoke(move);
            NotifyPlayerToMove();
        }

        void OnMoveChosen(Move move)
        {
            m_Board.MakeMove(move);
            m_SearchBoard.MakeMove(move);
        }

        void NotifyPlayerToMove()
        {
            if (m_GameResult == Result.NotStarted) return;

            m_GameResult = GetGameState();
            Debug.Log(m_GameResult.ToString());

            if (m_GameResult == Result.Playing)
            {
                m_PlayerToMove = (m_Board.WhiteToMove) ? m_WhitePlayer : m_BlackPlayer;

                // Have the player which did last local move calc the next AI move and
                // sync it back to users. Ya a bug will exist here where if multiple people are
                // watching an AI game and a player moves a piece
                // and immediately disconnects before its AI plays the next move, the game
                // may get stuck in a state it cant get past. Ways around this?
                if (m_LastMoveAppliedLocally) m_PlayerToMove.NotifyTurnToMove();
            }
            else
            {
                Debug.Log("Game Over");
            }
        }

        Result GetGameState()
        {
            MoveGenerator moveGenerator = new MoveGenerator();
            var moves = moveGenerator.GenerateMoves(m_Board);

            // Look for mate/stalemate
            if (moves.Count == 0)
            {
                if (moveGenerator.InCheck())
                {
                    return (m_Board.WhiteToMove)
                        ? Result.WhiteIsMated
                        : Result.BlackIsMated;
                }

                return Result.Stalemate;
            }

            // Fifty move rule
            if (m_Board.fiftyMoveCounter >= 100)
            {
                return Result.FiftyMoveRule;
            }

            // Threefold repetition
            int repCount = m_Board.RepetitionPositionHistory.Count((x => x == m_Board.ZobristKey));
            if (repCount == 3)
            {
                return Result.Repetition;
            }

            // Look for insufficient material (not all cases implemented yet)
            int numPawns = m_Board.pawns[Board.WhiteIndex].Count +
                           m_Board.pawns[Board.BlackIndex].Count;
            int numRooks = m_Board.rooks[Board.WhiteIndex].Count +
                           m_Board.rooks[Board.BlackIndex].Count;
            int numQueens = m_Board.queens[Board.WhiteIndex].Count +
                            m_Board.queens[Board.BlackIndex].Count;
            int numKnights = m_Board.knights[Board.WhiteIndex].Count +
                             m_Board.knights[Board.BlackIndex].Count;
            int numBishops = m_Board.bishops[Board.WhiteIndex].Count +
                             m_Board.bishops[Board.BlackIndex].Count;

            if (numPawns + numRooks + numQueens == 0)
            {
                if (numKnights == 1 || numBishops == 1)
                {
                    return Result.InsufficientMaterial;
                }
            }

            return Result.Playing;
        }

        void CreatePlayer(ref Player player, PlayerType playerType)
        {
            if (playerType == PlayerType.Human)
            {
                player = new HumanPlayer(m_Board);
            }
            else
            {
                // OnMoveChosen only gets called back from the AI because the ChessBoard puseshes
                // moves for human playes.
                if (player != null) player.onMoveChosen -= OnAIMoveChosen;
                player = new AIPlayer(m_SearchBoard, m_AISettings);
                player.onMoveChosen += OnAIMoveChosen;
            }
        }
    }

    public static class ChessAIExtensions
    {
        public static Coord ToCoord(this ChessSquare square)
        {
            return new Coord(square.File - 1, square.Rank - 1);
        }

        public static ChessSquare ToSquare(this Coord coord)
        {
            return new ChessSquare(coord.fileIndex + 1, coord.rankIndex + 1);
        }
    }
}
