using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
//using UnityLabs.Slices.Table;
using UnityLabs.Slices.Transitions;

namespace UnityLabs.Slices.Games.Chess
{
    /// <summary>
    /// Transition to the chess win state.
    /// </summary>
    public class ChessWinTransition : Transition
    {
        [FormerlySerializedAs("m_BoardController")]
        [SerializeField]
        ChessBoard m_Board = null;

        [SerializeField]
        ChessBoardStartAnimator m_BoardStartAnimator = null;

        [SerializeField]
        Transform m_OptionsButton = null;

        [SerializeField]
        XRBaseInteractable m_RestartButtonW = null;

        [SerializeField]
        XRBaseInteractable m_RestartButtonB = null;

        [SerializeField]
        WinBoardJobVisualizer m_WinVisualizer = null;

        [SerializeField]
        SkinnedMeshRenderer m_BoardMesh = null;

        [SerializeField]
        float m_BoardInsetDepth = 10f;

        void Awake()
        {
            //todo dependency management
            //m_BoardMesh = FindObjectOfType<TableBaseVisualController>().GetComponent<SkinnedMeshRenderer>();
            if (m_Board == null)
                Debug.LogWarning("Board is null in ChessWinTransition");
        }

        public override void OnBeginTransitionIn()
        {
            base.OnBeginTransitionIn();

            gameObject.SetActive(true);

            m_OptionsButton.gameObject.SetActive(false);

            m_Board.attachPiecesToSquares = true;

            m_BoardStartAnimator.OpenInsetInstant();

            for (int i = 0; i < m_Board.chessPieceControllers.Count; ++i)
            {
                if (m_Board.chessPieceControllers[i].PieceType == ChessPieceType.King &&
                    m_Board.chessPieceControllers[i].color == m_Board.playerLost.ComplimentaryColor())
                {
                    m_WinVisualizer.SetWinningPiece(m_Board.chessPieceControllers[i].transform);
                    break;
                }
            }

            m_WinVisualizer.Initialize();
        }

        public override void OnTransitionIn(float time)
        {
            base.OnTransitionIn(time);
            float easedTime = CurveEasedTime(time);
            m_WinVisualizer.SetWeight(easedTime);
            if (m_BoardMesh != null)
                m_BoardMesh.SetBlendShapeWeight(1, Mathf.Lerp(0, m_BoardInsetDepth, easedTime));
            m_WinVisualizer.rippleHeight = Mathf.Clamp(easedTime, 0, .01f);
            m_WinVisualizer.rippleOffset = Mathf.Lerp(0, .2f, easedTime);
        }

        public override void OnEndTransitionIn()
        {
            base.OnEndTransitionIn();
            m_WinVisualizer.SetWeight(1);
            if (m_BoardMesh != null)
                m_BoardMesh.SetBlendShapeWeight(1, m_BoardInsetDepth);
            m_RestartButtonW.enabled = true;
            m_RestartButtonB.enabled = true;
            m_RestartButtonW.gameObject.SetActive(true);
            m_RestartButtonB.gameObject.SetActive(true);
        }

        public override void OnBeginTransitionOut()
        {
            base.OnBeginTransitionOut();
            m_RestartButtonW.enabled = false;
            m_RestartButtonB.enabled = false;
            m_RestartButtonW.gameObject.SetActive(false);
            m_RestartButtonB.gameObject.SetActive(false);
            m_WinVisualizer.rippleHeight = 0;
            m_WinVisualizer.rippleOffset = 0;
        }

        public override void OnTransitionOut(float time)
        {
            base.OnTransitionOut(time);
            float easedTime = CurveEasedTime(time);
            m_WinVisualizer.SetWeight(easedTime);
            if (m_BoardMesh != null)
                m_BoardMesh.SetBlendShapeWeight(1, Mathf.Lerp(0, m_BoardInsetDepth, easedTime));
        }

        public override void OnEndTransitionOut()
        {
            base.OnEndTransitionOut();
            m_WinVisualizer.rippleHeight = 0;
            m_WinVisualizer.rippleOffset = 0;
            m_WinVisualizer.SetWeight(0);
            m_WinVisualizer.ShutDown();
            if (m_BoardMesh != null)
                m_BoardMesh.SetBlendShapeWeight(1, 0);
            m_RestartButtonW.enabled = false;
            m_RestartButtonB.enabled = false;
            m_RestartButtonW.gameObject.SetActive(false);
            m_RestartButtonB.gameObject.SetActive(false);
            m_OptionsButton.gameObject.SetActive(true);
            m_Board.attachPiecesToSquares = false;

            gameObject.SetActive(false);
        }
    }
}
