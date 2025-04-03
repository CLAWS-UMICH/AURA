using System;
using System.Collections;
using System.Collections.Generic;
using Chess;
using Unity.XR.CoreUtils.Bindings;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Content.Utils;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;
using Result = Chess.Game.Result;
using PlayerType = Chess.Game.PlayerType;

namespace UnityLabs.Slices.Games.Chess
{
    public class ChessBoard : MonoBehaviour
    {
        [SerializeField]
        NetworkChessBoard m_NetworkChessBoard;

        [SerializeField]
        AbstractChessEngine m_ChessAIEngine;

        [SerializeField]
        ChessBoardGenerator m_BoardGenerator = null;

        [SerializeField]
        ChessTimer m_ChessTimer = null;

        [SerializeField]
        List<ChessPiece> m_ChessPieceControllers = new List<ChessPiece>();

        [Header("Board Offset")]
        [SerializeField]
        Transform m_BoardRootTransform = null;

        [SerializeField]
        Transform m_GameRootTransform = null;

        [SerializeField]
        float m_TurnZOffset;

        [Header("Pieces")]
        [SerializeField]
        Transform m_WhiteParent = null;

        [SerializeField]
        Transform m_BlackParent = null;

        public event Action<ChessColor> onUndoAvailable;
        public event Action<ChessColor, string> onPlayerLost;

        ChessColor m_PlayerLost = ChessColor.None;

        ChessColor m_CurrentTurn = ChessColor.White;

        ChessPiece m_PendingMovePiece = null;

        public ChessColor currentTurn => m_CurrentTurn;

        /// <summary>
        /// Changes behavior of how pieces attach to squares
        /// </summary>
        public bool attachPiecesToSquares
        {
            get => m_AttachPiecesToSquares;
            set
            {
                m_AttachPiecesToSquares = value;
                SyncPiecesToTiles(); // Necessary to update transform targets.
            }
        }

        public bool shouldShowUndo => !isMoveConfirmed && Moves.Count > 0;

        public bool isMoveConfirmed => (m_CurrentTurn == ChessColor.Black && m_ChessAIEngine.currentTurnSide == ChessColor.Black) ||
                                       (m_CurrentTurn == ChessColor.White && m_ChessAIEngine.currentTurnSide == ChessColor.White);

        /// <summary>
        /// All chess pieces on the board.
        /// </summary>
        public IReadOnlyList<ChessPiece> chessPieceControllers => m_ChessPieceControllers;

        /// <summary>
        /// All chess pieces spawned during gameplay. Will get flushed and pieces destroyed on game reset.
        /// This only holds the pieces that this client spawned. This is because each client will call the
        /// networked Destroy function to destroy the pieces it spawned and the network will replicate that.
        /// </summary>
        readonly List<ChessPiece> m_SpawnedChessPieceControllers = new List<ChessPiece>();

        readonly List<Move> m_Moves = new List<Move>();
        public List<Move> Moves => m_Moves;

        Vector2 m_MoveTimer = new Vector2(OptionState.defaultTimeAmount, OptionState.defaultTimeAmount);
        public Vector2 moveTimer => m_MoveTimer;

        OptionState m_OptionState = OptionState.DefaultOptionsState();
        public OptionState optionState => m_OptionState;

        BindableVariable<bool> m_ShowingOptions = new BindableVariable<bool>();
        public IReadOnlyBindableVariable<bool> showingOptions => m_ShowingOptions;

        BindableEnum<ChessGameMode> m_GameMode = new();
        public IReadOnlyBindableVariable<ChessGameMode> GameMode => m_GameMode;

        BindableVariable<float> m_BoardRotation = new BindableVariable<float>();
        public BindableVariable<float> boardRotation => m_BoardRotation;

        BindableVariable<bool> m_GameConnected = new BindableVariable<bool>();
        public IReadOnlyBindableVariable<bool> gameConnected => m_GameConnected;

        public ChessBoardGenerator boardGenerator => m_BoardGenerator;

        public ChessColor playerLost => m_PlayerLost;


        static readonly Vector3 k_PieceInvalidLocalPostion = new Vector3(0, 10, 0);

        readonly BindingsGroup m_BindingGroup = new BindingsGroup();

#pragma warning disable CS0618 // Type or member is obsolete
        readonly Vector3TweenableVariable m_BoardPositionAttribute = new Vector3TweenableVariable();

        readonly QuaternionTweenableVariable m_BoardRotationAttribute = new QuaternionTweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete

        readonly List<ChessPiece> m_DeadBlackPieces = new List<ChessPiece>();
        readonly List<ChessPiece> m_DeadWhitePieces = new List<ChessPiece>();
        readonly List<ChessPiece> m_HiddenPieces = new List<ChessPiece>();
        readonly Dictionary<ChessSquare, ChessPiece> m_ChessPieceMatrix = new Dictionary<ChessSquare, ChessPiece>();
        readonly HashSet<ChessSquare> m_HighlightedSquares = new HashSet<ChessSquare>();
        readonly HashSet<ChessPiece> m_SelectedPieces = new HashSet<ChessPiece>();
        Vector2 m_PriorMoveTimer = Vector2.zero;
        bool m_AttachPiecesToSquares = false;

        void OnEnable()
        {
            m_BoardPositionAttribute.Value = m_BoardRootTransform.localPosition;
            m_BindingGroup.AddBinding(m_BoardPositionAttribute.SubscribeAndUpdate(newPos => m_BoardRootTransform.localPosition = newPos));
            m_BindingGroup.AddBinding(m_BoardRotation.SubscribeAndUpdate(newRot =>
            {
                m_GameRootTransform.localEulerAngles = m_GameRootTransform.localEulerAngles.SetAxis(newRot, Axis.Y);
                m_BoardRotationAttribute.target = m_GameRootTransform.localRotation;
            }));
            m_BoardRotationAttribute.Value = m_GameRootTransform.localRotation;
            m_BindingGroup.AddBinding(m_BoardRotationAttribute.SubscribeAndUpdate(newRot =>
            {
                m_BoardGenerator.transform.localRotation = newRot;
            }));

            m_ChessAIEngine.onPieceAdded += PieceAdded;
            m_ChessAIEngine.onPieceMoved += PieceMoved;
            m_ChessAIEngine.onPieceRemoved += PieceRemoved;
            m_ChessAIEngine.onAIMoveCompleted += AIMoveCompleted;
        }

        void OnDisable()
        {
            m_BindingGroup.Clear();
            m_ChessAIEngine.onPieceAdded -= PieceAdded;
            m_ChessAIEngine.onPieceMoved -= PieceMoved;
            m_ChessAIEngine.onPieceRemoved -= PieceRemoved;
            m_ChessAIEngine.onAIMoveCompleted -= AIMoveCompleted;
            OnGameEnded("App Closed");
        }

        void Update()
        {
            m_BoardPositionAttribute.HandleTween(Time.deltaTime * 4f);
            m_BoardRotationAttribute.HandleTween(Time.deltaTime * 3f);

            EvaluateSelectedPieceOverlaps();
            CountDownTimer(Time.deltaTime);

            if (attachPiecesToSquares)
            {
                for (int i = 0; i < m_ChessPieceControllers.Count; ++i)
                {
                    if (m_ChessPieceControllers[i].currentSquare.IsValid)
                    {
                        var tile = m_BoardGenerator.positionMap[m_ChessPieceControllers[i].currentSquare];
                        m_ChessPieceControllers[i].transform.localPosition = tile.transform.localPosition;
                    }
                }
            }
        }

        /// <summary>
        /// Enable/Disable renderers of ChessPieces
        /// </summary>
        public void SetChessPiecesVisibility(bool state)
        {
            for (int i = 0; i < m_ChessPieceControllers.Count; ++i)
            {
                m_ChessPieceControllers[i].SetPieceVisibility(state);
            }
        }

        public void OnNetworkTick()
        {
            for (int i = 0; i < m_ChessPieceControllers.Count; ++i)
            {
                m_ChessPieceControllers[i].OnNetworkTick();
            }
        }

        void AIMoveCompleted(Move move)
        {
            StartCoroutine(AIMoveCompletedCoroutine(move));
        }

        IEnumerator AIMoveCompletedCoroutine(Move move)
        {
            ClaimBoardStateOwnership();
            m_Moves.Add(move);
            // Immediately ending turn after adding to m_Moves list can make network race condition
            // because m_Moves needs to be synced first, so wait a quarter second.
            yield return new WaitForSeconds(0.5f);
            ClaimBoardStateOwnership();
            EndTurnLocal();
        }

        void PieceRemoved(ChessSquare fromSquare, ChessPieceType pieceType, ChessColor pieceColor)
        {
            Debug.Log($"Piece Removed {fromSquare} {pieceType} {pieceColor}");
            AddToGraveyard(m_ChessPieceMatrix[fromSquare]);
            m_ChessPieceMatrix.Remove(fromSquare);
            SyncPiecesToTiles();
        }

        void PieceMoved(ChessSquare fromSquare, ChessSquare toSquare, ChessPieceType type, ChessColor pieceColor)
        {
            Debug.Log($"Piece Moved {fromSquare} {toSquare} {type} {pieceColor}");
            m_ChessPieceMatrix.Add(toSquare, m_ChessPieceMatrix[fromSquare]);
            m_ChessPieceMatrix[toSquare].currentSquare = toSquare;
            m_ChessPieceMatrix.Remove(fromSquare);
            SyncPiecesToTiles();
        }

        void PieceAdded(ChessSquare toSquare, ChessPieceType pieceType, ChessColor pieceColor)
        {
            Debug.Log($"Piece Added {toSquare} {pieceType} {pieceColor}");

            ChessPiece addedPiece = RetrieveFromGraveyard(pieceColor, pieceType); ;
            if (addedPiece != null)
            {
                addedPiece.currentSquare = toSquare;
                m_ChessPieceMatrix.Add(toSquare, addedPiece);
            }

            SyncPiecesToTiles();
        }

        public void OnGameConnectChanged(bool connected)
        {
            m_GameConnected.Value = connected;
            Debug.Log($"Game Connected: {connected}");

            // this may now be unnnecessary since game disconnect would just unload whole board?
            // if (!connected)
            // {
            //     m_ChessAIEngine.StopGame();
            //     m_GameMode.Value = ChessGameMode.NotStarted;
            //     m_ShowingOptions.Value = false;
            //     ResetBoardState(false);
            //
            //     m_OptionState = OptionState.DefaultOptionsState();
            //
            //     // When a player leaves a room, normcore will automatically delete all pieces for that client
            //     // so flush the references to spawned pieces in the chess piece lists.
            //     for (int i = 0; i < m_SpawnedChessPieceControllers.Count; i++)
            //     {
            //         m_ChessPieceControllers.Remove(m_SpawnedChessPieceControllers[i]);
            //     }
            //     m_SpawnedChessPieceControllers.Clear();
            // }
        }

        public void MoveListModelRemoved(int index, Move undoMove)
        {
            if (m_Moves.Count - 1 != index)
            {
                // Thus far this has never happened. Normcore seems to ensure proper order of adding/removing
                Debug.LogError("Trying to remove move non-sequentially!.");
                return;
            }

            Move undoListMove = m_Moves[^1];

            // This should never happen if its working properly, just doing this check
            // to make sure Normcore behavior is as expected.
            if (undoListMove.StartCoord.fileIndex != undoMove.StartCoord.fileIndex ||
                undoListMove.StartCoord.rankIndex != undoMove.StartCoord.rankIndex ||
                undoListMove.TargetCoord.fileIndex != undoMove.TargetCoord.fileIndex ||
                undoListMove.TargetCoord.fileIndex != undoMove.TargetCoord.fileIndex)
            {
                Debug.LogError("Tried to remove a MovementModel that didn't exist!");
            }
            else
            {
                // Called on game reset or undoing a move.
                m_Moves.RemoveAt(m_Moves.Count - 1);

                // This would occur if undo is called on a move. This just assumes the only reason
                // you'd want to do this is if you are undoing, so it just pops the last on the list
                // rather than the exact index. As popping to an index may end up popping more than one.
                // I am not entirely sure if the index value passed into this will be in correct order.
                m_ChessAIEngine.UndoMove(undoListMove);

                ClearPendingMove();

                // need to do this again here after undo move, because the chess model
                // is not rolled back till undo move to compute new legal positions
                UpdateHighlightedPieces();

                onUndoAvailable?.Invoke(ChessColor.None);
            }
        }

        public void MoveListModelAdded(Move move, int index)
        {
            if (m_Moves.Count != index)
            {
                // Thus far this has never happened. Normcore seems to ensure proper order of adding/removing
                Debug.LogError("Trying to add move non-sequentially!");
                return;
            }

            Debug.Log($"MoveListModelAdded {move}");

            ChessSquare start = move.StartCoord.ToSquare();
            ChessSquare end = move.TargetCoord.ToSquare();

            Move legalMove = m_ChessAIEngine.TryGetLegalMove(start, end);
            if (legalMove.IsInvalid)
            {
                Debug.LogError("Could not find legal move that is trying to be synced! Very bad, game state probably no longer synced.");
                return;
            }

            // Clear pending move before setting new just in case.
            // This will get called many times sequentially when playing joins mid game.
            ClearPendingMove();

            // todo something weird probably if player join mid move of other player?
            m_PendingMovePiece = m_ChessPieceMatrix[start];
            m_PendingMovePiece.pendingMove = legalMove;
            m_PendingMovePiece.startSquare = start;

            m_ChessAIEngine.ApplyMove(legalMove, false);
            m_Moves.Add(legalMove);
            onUndoAvailable?.Invoke(shouldShowUndo ? currentTurn : ChessColor.None);
        }

        public bool PlayerIsAI(ChessColor color)
        {
            switch (m_GameMode.Value)
            {
                case ChessGameMode.HumanVsAI when color == ChessColor.Black:
                case ChessGameMode.AIvsHuman when color == ChessColor.White:
                case ChessGameMode.AIvsAI:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Stop game and reset its state.
        /// </summary>
        public void ResetGame()
        {
            onUndoAvailable?.Invoke(ChessColor.None);
            m_ChessAIEngine.StopGame();
            m_GameMode.Value = ChessGameMode.NotStarted;
            DeleteSpawnedPieces();
            OnGameEnded("ResetGame");
        }

        /// <summary>
        /// Trigger event to start or stop the games.
        /// This is called from network synchronization every frame.
        /// </summary>
        public void SetGameStartedRemote(ChessGameMode gameMode)
        {
            if (m_GameMode.Value != gameMode)
            {
                if (gameMode != ChessGameMode.NotStarted)
                {
                    StartGame(gameMode, false);
                }
                else
                {
                    m_ChessAIEngine.StopGame();
                    m_GameMode.Value = ChessGameMode.NotStarted;
                }
            }
        }

        /// <summary>
        /// Trigger event to start or stop the games and to also reset all the state of the board
        /// This is typically called from user input triggering a game start/stop.
        /// </summary>
        public void StartGame(ChessGameMode gameMode, bool fromLocal)
        {
            switch (gameMode)
            {
                case ChessGameMode.HumanVsHuman:
                    m_ChessAIEngine.NewGame(PlayerType.Human, PlayerType.Human, fromLocal);
                    break;
                case ChessGameMode.HumanVsAI:
                    m_ChessAIEngine.NewGame(PlayerType.Human, PlayerType.AI, fromLocal);
                    break;
                case ChessGameMode.AIvsHuman:
                    m_ChessAIEngine.NewGame(PlayerType.AI, PlayerType.Human, fromLocal);
                    break;
                case ChessGameMode.AIvsAI:
                    m_ChessAIEngine.NewGame(PlayerType.AI, PlayerType.AI, fromLocal);
                    break;
            }

            ResetBoardState();

            m_GameMode.Value = gameMode;
        }

        /// <summary>
        /// Sets the internal state of board back to its starting condition.
        /// Does not have to be synced, used to clear locally on disconnect.
        ///
        /// Ensure this only ever called before syncing of moves on player join!
        /// </summary>
        void ResetBoardState()
        {
            m_Moves.Clear();
            m_DeadWhitePieces.Clear();
            m_DeadBlackPieces.Clear();
            m_MoveTimer = new Vector2(optionState.timeAmountValue, optionState.timeAmountValue);
            m_CurrentTurn = m_ChessAIEngine.currentTurnSide;

            m_PlayerLost = ChessColor.None;
            if (m_PendingMovePiece != null)
            {
                m_PendingMovePiece.pendingMove = Move.InvalidMove;
                m_PendingMovePiece.startSquare = ChessSquare.Zero;
                m_PendingMovePiece = null;
            }

            ResetChessPieceMatrix();
            RefreshVisuals();
            SyncPiecesToTiles();
            OnGameStarted();
        }

        void ResetChessPieceMatrix()
        {
            m_ChessPieceMatrix.Clear();
            m_HiddenPieces.Clear();
            foreach (var chessPiece in m_ChessPieceControllers)
            {
                if (!string.IsNullOrEmpty(chessPiece.initialTileCode))
                {
                    chessPiece.currentSquare = ChessSquareUtil.StringToSquare(chessPiece.initialTileCode);
                    m_ChessPieceMatrix.Add(chessPiece.currentSquare, chessPiece);
                }
                else
                {
                    chessPiece.currentSquare = ChessSquare.Zero;
                    m_HiddenPieces.Add(chessPiece);
                }
            }
        }

        public void ConfirmMove(bool selected)
        {
            Debug.Log($"Confirm Move: {selected}");
            if (!selected)
            {

            }

            ClaimBoardStateOwnership();
            EndTurnLocal();
        }

        public void UndoPendingMove()
        {
            if (m_PendingMovePiece != null)
            {
                UndoMove(false);

                // need to do this again here after undo move, because the chess model
                // is not rolled back till undo move to compute new legal positions
                UpdateHighlightedPieces();
            }
        }

        public bool UndoMove(bool selected)
        {
            if (!selected)
            {
                ClaimBoardStateOwnership();
                Move undoMove = m_Moves[m_Moves.Count - 1];
                m_Moves.RemoveAt(m_Moves.Count - 1);
                m_PendingMovePiece.pendingMove = Move.InvalidMove;
                m_PendingMovePiece.startSquare = ChessSquare.Invalid;
                m_PendingMovePiece = null;
                m_ChessAIEngine.UndoMove(undoMove);
                UpdateHighlightedPieces();
                onUndoAvailable?.Invoke(ChessColor.None);
                return true;
            }

            return false;
        }

        public Move TryMovement(ChessSquare fromSquare, ChessSquare toSquare, ChessPiece pendingMovePiece)
        {
            Move legalMove = m_ChessAIEngine.TryGetLegalMove(fromSquare, toSquare);
            if (legalMove.IsInvalid) return legalMove;
            ClaimBoardStateOwnership();
            m_ChessAIEngine.ApplyMove(legalMove, true);
            m_Moves.Add(legalMove);
            m_PendingMovePiece = pendingMovePiece;
            onUndoAvailable?.Invoke(currentTurn);
            return legalMove;
        }

        ChessPiece RetrieveFromGraveyard(ChessColor color, ChessPieceType pieceType)
        {
            ChessPiece piece = null;
            if (color == ChessColor.White)
            {
                piece = m_DeadWhitePieces.Find(p => p.PieceType == pieceType);
                if (piece != null)
                {
                    m_DeadWhitePieces.Remove(piece);
                    return piece;
                }
            }
            else if (color == ChessColor.Black)
            {
                piece = m_DeadBlackPieces.Find(p => p.PieceType == pieceType);
                if (piece != null)
                {
                    m_DeadBlackPieces.Remove(piece);
                    return piece;
                }
            }

            piece = m_HiddenPieces.Find(p => p.PieceType == pieceType && p.color == color);
            if (piece != null)
            {
                m_HiddenPieces.Remove(piece);
                return piece;
            }

            Debug.LogWarning($"No {color} {pieceType} to retrieve.");
            return null;
        }

        void AddToGraveyard(ChessPiece chessPiece)
        {
            if (chessPiece.color == ChessColor.White)
            {
                if (m_DeadWhitePieces.Contains(chessPiece)) Debug.LogError($"Trying to add piece to graveyard twice! {chessPiece}");
                chessPiece.currentSquare = ChessSquare.WhiteGraveyard;
                m_DeadWhitePieces.Add(chessPiece);
            }
            else if (chessPiece.color == ChessColor.Black)
            {
                if (m_DeadBlackPieces.Contains(chessPiece)) Debug.LogError($"Trying to add piece to graveyard twice! {chessPiece}");
                chessPiece.currentSquare = ChessSquare.BlackGraveyard;
                m_DeadBlackPieces.Add(chessPiece);
            }
        }

        void SyncPiecesToTiles()
        {
            int deadBlackPieces = 0;
            int deadWhitePieces = 0;
            float graveyardGap = m_BoardGenerator.tileWidth / 4f;
            for (int i = 0; i < m_ChessPieceControllers.Count; ++i)
            {
                if (m_ChessPieceControllers[i].currentSquare == ChessSquare.BlackGraveyard)
                {
                    m_ChessPieceControllers[i].UpdateGraveyardOffset(m_BoardGenerator.blackGraveyardRoot, graveyardGap, deadBlackPieces++, m_DeadBlackPieces.Count, false);
                }
                else if (m_ChessPieceControllers[i].currentSquare == ChessSquare.WhiteGraveyard)
                {
                    m_ChessPieceControllers[i].UpdateGraveyardOffset(m_BoardGenerator.whiteGraveyardRoot, graveyardGap, deadWhitePieces++, m_DeadWhitePieces.Count, true);
                }
                else if (m_ChessPieceControllers[i].currentSquare == ChessSquare.Invalid || m_ChessPieceControllers[i].currentSquare == ChessSquare.Zero)
                {
                    m_ChessPieceControllers[i].localPositionTarget = k_PieceInvalidLocalPostion;
                }
                else
                {
                    var tile = m_BoardGenerator.positionMap[m_ChessPieceControllers[i].currentSquare];
                    m_ChessPieceControllers[i].localPositionTarget = tile.initialLocalPosition;
                }
            }
        }

        void DeleteSpawnedPieces()
        {
            foreach (var chessPiece in m_SpawnedChessPieceControllers)
            {
                if (chessPiece == null)
                {
                    Debug.LogWarning("Null ChessPiece in SpawnedChessPieceControllers. This should not happen.");
                    continue;
                }
                m_ChessPieceControllers.Remove(chessPiece);
                Destroy(chessPiece.gameObject);
                // Realtime.Destroy(chessPiece.gameObject);
            }
            m_SpawnedChessPieceControllers.Clear();
        }

        public void EndTurnRemote(ChessColor newTurn)
        {
            m_CurrentTurn = newTurn;
            RefreshVisuals();
            onUndoAvailable?.Invoke(shouldShowUndo ? m_CurrentTurn : ChessColor.None);
            ClearPendingMove();
            m_ChessAIEngine.EndTurn(false);
            SyncPiecesToTiles();
            if (isMoveConfirmed) CheckPlayerLose(m_CurrentTurn);
        }

        void EndTurnLocal()
        {
            m_CurrentTurn = m_ChessAIEngine.currentTurnSide;
            RefreshVisuals();
            onUndoAvailable?.Invoke(ChessColor.None);
            ClearPendingMove();
            m_ChessAIEngine.EndTurn(true);
            SyncPiecesToTiles();
            CheckPlayerLose(m_CurrentTurn);
        }

        void ClearPendingMove()
        {
            if (m_PendingMovePiece == null) return;
            m_PendingMovePiece.startSquare = ChessSquare.Zero;
            m_PendingMovePiece.pendingMove = Move.InvalidMove;
            m_PendingMovePiece = null;
        }

        void CheckPlayerLose(ChessColor color)
        {
            if (m_ChessAIEngine.IsPlayMated(color))
            {
                m_PlayerLost = color;
                onPlayerLost?.Invoke(color, "Checkmate!");
                OnGameEnded(color == ChessColor.White ? "BlackIsMated" : "WhiteIsMated");
            }

            if (m_ChessAIEngine.isStaleMated)
            {
                PlayerLoseWithLeastPieces("Stalemate");
                OnGameEnded("Stalemate");
            }

            if (m_ChessAIEngine.gameResult != Result.Playing && m_ChessAIEngine.gameResult != Result.NotStarted)
            {
                switch (m_ChessAIEngine.gameResult)
                {
                    case Result.WhiteIsMated:
                        m_PlayerLost = ChessColor.White;
                        onPlayerLost?.Invoke(color, "White Checkmated!");
                        break;
                    case Result.BlackIsMated:
                        m_PlayerLost = ChessColor.Black;
                        onPlayerLost?.Invoke(color, "Black Checkmated!");
                        break;
                    case Result.Stalemate:
                        PlayerLoseWithLeastPieces("Stalemate!");
                        break;
                    case Result.Repetition:
                        PlayerLoseWithLeastPieces("Repeated 3 moves!");
                        break;
                    case Result.FiftyMoveRule:
                        PlayerLoseWithLeastPieces("50 Moves Reached!");
                        break;
                    case Result.InsufficientMaterial:
                        // I don't actually know what triggers this
                        PlayerLoseWithLeastPieces("Too few pieces!");
                        break;
                }
                OnGameEnded(m_ChessAIEngine.gameResult.ToString());
            }
        }

        /// <summary>
        /// Get a square based on a world position Vector3. This will look up the square via its initial
        /// starting location, not where it may currently be floating from an animation.
        /// </summary>
        public ChessSquare GetSquareFromPos(Vector3 pos)
        {
            return m_BoardGenerator.GetSquareClosestToPos(pos);
        }

        /// <summary>
        /// Get the world position of a square, this will return the world position based on the squares initial starting
        /// location, not where it may currently be floating due to an animation.
        /// </summary>
        public Vector3 GetPosFromSquare(ChessSquare pos)
        {
            return m_BoardGenerator.transform.TransformPoint(m_BoardGenerator.positionMap[pos].initialLocalPosition);
        }

        public Vector3 GetLocalPosFromSquare(ChessSquare pos)
        {
            return m_BoardGenerator.positionMap[pos].initialLocalPosition;
        }

        readonly HashSet<ChessSquare> m_PieceLocations = new HashSet<ChessSquare>();

        public void SetPieceLocationHighlighted(ChessSquare pieceLocation, bool isHighlighted)
        {
            if (pieceLocation.IsValid)
            {
                if (isHighlighted)
                {
                    m_PieceLocations.Add(pieceLocation);
                }
                else
                {
                    m_PieceLocations.Remove(pieceLocation);
                }
            }
        }

        void RefreshVisuals()
        {
            Vector3 pos = m_BoardPositionAttribute.target;
            m_BoardPositionAttribute.target = pos.SetAxis(GetBoardOffset(currentTurn), Axis.Z);
            m_ChessTimer.RefreshVisuals(currentTurn);
            m_PieceLocations.Clear();
            m_SelectedPieces.Clear();
        }

        float GetBoardOffset(ChessColor newTurn)
        {
            if (!m_OptionState.slideBoard)
                return 0;
            return newTurn == ChessColor.White ? -m_TurnZOffset : m_TurnZOffset;
        }

        public void UpdateHighlightedPieces()
        {
            if (m_GameMode.Value == ChessGameMode.NotStarted)
                return;

            m_HighlightedSquares.Clear();
            if (m_OptionState.showLegalMoves)
            {
                foreach (var pieceLocation in m_PieceLocations)
                {
                    m_LegalSquares.Clear();
                    m_ChessAIEngine.GetLegalTargetSquares(pieceLocation, m_LegalSquares);

                    foreach (var square in m_LegalSquares)
                    {
                        m_HighlightedSquares.Add(square);
                    }

                    m_HighlightedSquares.Add(pieceLocation);
                }
            }

            foreach ((ChessSquare key, ChessBoardTile value) in m_BoardGenerator.positionMap) // todo switch to position list
            {
                value.isHighlighted = m_HighlightedSquares.Contains(key);
            }
        }

        readonly List<ChessSquare> m_LegalSquares = new List<ChessSquare>();

        public void SetPieceSelected(ChessPiece selectedPiece, bool isSelected)
        {
            if (isSelected)
            {
                m_SelectedPieces.Add(selectedPiece);
            }
            else
            {
                m_SelectedPieces.Remove(selectedPiece);
                if (selectedPiece.currentOverlap != null)
                {
                    selectedPiece.currentOverlap.SetOverlapped(false);
                    selectedPiece.currentOverlap = null;
                }
            }
        }

        void EvaluateSelectedPieceOverlaps()
        {
            foreach (var selectedPiece in m_SelectedPieces)
            {
                var selectedSquare = selectedPiece.squareFromPosition;
                m_ChessPieceMatrix.TryGetValue(selectedSquare, out var overlappedPiece);
                if (overlappedPiece != selectedPiece.currentOverlap)
                {
                    if (selectedPiece.currentOverlap != null)
                    {
                        selectedPiece.currentOverlap.SetOverlapped(false);
                        selectedPiece.currentOverlap = null;
                    }
                    if (overlappedPiece != selectedPiece && m_BoardGenerator.positionMap[selectedSquare].isHighlighted)
                    {
                        overlappedPiece.SetOverlapped(true);
                        selectedPiece.currentOverlap = overlappedPiece;
                    }
                }
            }
        }

        /// <summary>
        /// Syncs timer over the network every move.
        /// </summary>
        public void UpdateMoveTimer(Vector2 newTime)
        {
            m_MoveTimer = newTime;
        }

        void UpdateTimeControl(bool enabled)
        {
            if (enabled)
            {
                m_ChessTimer.UpdateTimerText(ChessColor.White, m_MoveTimer.x);
                m_ChessTimer.UpdateTimerText(ChessColor.Black, m_MoveTimer.y);
            }
            else
            {
                m_ChessTimer.SetInfinityText();
            }
        }

        void CountDownTimer(float deltatTime)
        {
            if (!m_OptionState.timeControl || m_PlayerLost != ChessColor.None) return;

            if (currentTurn == ChessColor.White)
            {
                m_MoveTimer.x -= deltatTime;
                if (m_MoveTimer.x <= 0)
                {
                    m_PlayerLost = ChessColor.White;
                    onPlayerLost?.Invoke(m_PlayerLost, "White Timeout!");
                }
            }
            else
            {
                m_MoveTimer.y -= deltatTime;
                if (m_MoveTimer.y <= 0)
                {
                    m_PlayerLost = ChessColor.Black;
                    onPlayerLost?.Invoke(m_PlayerLost, "Black Timeout!");
                }
            }

            // Check to see to see if the second has actually changed so we don't create garbage from a new string 60+ times a second!
            if ((int)m_PriorMoveTimer.x != (int)m_MoveTimer.x)
            {
                m_PriorMoveTimer.x = m_MoveTimer.x;
                m_ChessTimer.UpdateTimerText(ChessColor.White, m_MoveTimer.x);
            }
            if ((int)m_PriorMoveTimer.y != (int)m_MoveTimer.y)
            {
                m_PriorMoveTimer.y = m_MoveTimer.y;
                m_ChessTimer.UpdateTimerText(ChessColor.Black, m_MoveTimer.y);
            }
        }

        void PlayerLoseWithLeastPieces(string message)
        {
            int deadBlackPieces = 0;
            int deadWhitePieces = 0;
            foreach (var chessPieceController in m_ChessPieceControllers)
            {
                if (chessPieceController.currentSquare == ChessSquare.BlackGraveyard) deadBlackPieces++;
                else if (chessPieceController.currentSquare == ChessSquare.WhiteGraveyard) deadWhitePieces++;
            }

            m_PlayerLost = deadBlackPieces > deadWhitePieces ? ChessColor.White : ChessColor.Black;
            onPlayerLost?.Invoke(m_PlayerLost, message);
        }

        public void SetShowingOptions(bool isShowingOptions)
        {
            m_ShowingOptions.Value = isShowingOptions;
        }

        public void ClaimBoardStateOwnership()
        {
            m_NetworkChessBoard.ClaimOwnership();
            // m_NetworkTransform.NetworkObject.RemoveOwnership();
        }

        public void UpdateOptionsState(OptionState newOptionState)
        {
            if (Equals(newOptionState, m_OptionState) || GameMode.Value == ChessGameMode.NotStarted)
                return;

            if (m_OptionState.timeAmountIndex != newOptionState.timeAmountIndex)
            {
                m_MoveTimer = new Vector2(newOptionState.timeAmountValue, newOptionState.timeAmountValue);
            }

            if (m_OptionState.timeControl != newOptionState.timeControl)
            {
                UpdateTimeControl(newOptionState.timeControl);
            }

            m_OptionState = newOptionState;
            RefreshVisuals();
        }

        [ContextMenu("GatherChessPieces")]
        void GatherChessPieces()
        {
            m_ChessPieceControllers.AddRange(m_WhiteParent.GetComponentsInChildren<ChessPiece>());
            m_ChessPieceControllers.AddRange(m_BlackParent.GetComponentsInChildren<ChessPiece>());
        }

        [ContextMenu("TimeExpire")]
        void TimeExpire()
        {
            ClaimBoardStateOwnership();
            m_MoveTimer = Vector2.zero;
        }

        void OnGameStarted()
        {
        }

        void OnGameEnded(string gameResult)
        {
        }
    }
}
