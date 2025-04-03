using System;
using System.Collections.Generic;
using Chess;
using Chess.Game;
using UnityEngine;
using ChessBoard = Chess.Board;

namespace UnityLabs.Slices.Games.Chess
{
    public abstract class AbstractChessEngine : MonoBehaviour
    {
        public abstract event Action<ChessSquare, ChessPieceType, ChessColor> onPieceRemoved;
        public abstract event Action<ChessSquare, ChessSquare, ChessPieceType, ChessColor> onPieceMoved;
        public abstract event Action<ChessSquare, ChessPieceType, ChessColor> onPieceAdded;
        public abstract event Action<Move> onAIMoveCompleted;

        public abstract Result gameResult { get; }

        public abstract bool lastMoveAppliedLocally { get; }

        public abstract bool isStaleMated { get; }

        public abstract ChessColor currentTurnSide { get; }

        public abstract void NewGame(PlayerType whitePlayerType, PlayerType blackPlayerType, bool fromLocal);

        public abstract void StopGame();

        public abstract void GetLegalTargetSquares(ChessSquare fromSquare, List<ChessSquare> validSquares);

        public abstract Move TryGetLegalMove(ChessSquare fromSquare, ChessSquare toSquare);

        public abstract void ApplyMove(Move move, bool fromLocal);

        public abstract void EndTurn(bool fromLocal);

        public abstract bool IsPlayMated(ChessColor color);

        public abstract void UndoMove(Move move);
    }
}
