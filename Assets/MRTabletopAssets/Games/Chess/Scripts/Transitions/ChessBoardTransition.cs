using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;
using UnityLabs.Slices.Transitions;
using UnityLabs.SmartUX.Interaction.Widgets;
using Unity.Mathematics;
using UnityEngine.XR.Content.Utils;

namespace UnityLabs.Slices.Games.Chess
{
    /// <summary>
    /// The second transition which goes from the setup menu to the game board.
    /// </summary>
    public class ChessBoardTransition : Transition
    {
        [SerializeField]
        Canvas m_BoardCanvas = null;

        [SerializeField]
        GameObject m_TimerObject;

        [SerializeField]
        Transform m_BoardRoot = null;

        [SerializeField]
        Transform m_SetupMenuRoot = null;

        [SerializeField]
        Transform m_PiecesRoot = null;

        [SerializeField]
        ChessBoardUI m_ChessBoardUI;

        [SerializeField]
        ChessBoard m_ChessBoard = null;

        [SerializeField]
        XRToggle m_ToggleOptions = null;

        [SerializeField]
        ChessBoardStartAnimator m_BoardStartAnimator = null;

#pragma warning disable CS0618 // Type or member is obsolete
        Vector3TweenableVariable m_PiecesRootScaleAttribute = new Vector3TweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete


        static readonly Vector3 m_PiecesCloseScale = new Vector3(1f, 0.05f, 1f);
        readonly BindingsGroup m_BindingGroup = new BindingsGroup();

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_TimerObject != null)
                m_TimerObject.SetActive(false);
            m_BindingGroup.AddBinding(m_PiecesRootScaleAttribute.Subscribe(scale => m_PiecesRoot.localScale = (Vector3)scale));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_BindingGroup.Clear();
        }

        public override void OnBeginTransitionIn()
        {
            base.OnBeginTransitionIn();

            if (m_ToggleOptions != null)
                m_ToggleOptions.enabled = false;
            m_ChessBoardUI.SetStartButtonsActive(false);

            m_BoardRoot.gameObject.SetActive(true);
            m_BoardCanvas.gameObject.SetActive(false);

            m_PiecesRootScaleAttribute.Initialize(m_PiecesCloseScale);
            m_PiecesRootScaleAttribute.animationCurve = transitionCurve;

            // todo some scheme so that you don't have to do this in chained coroutines, but can rather sequence tween from the time
            // value passed in through OnTransitionIn so you can then make this interruptable
            StartCoroutine(m_BoardStartAnimator.StartGame(duration: InDuration * .75f, () =>
            {
                m_ChessBoard.SetChessPiecesVisibility(true);

                m_BoardCanvas.gameObject.SetActive(true);
                m_PiecesRoot.gameObject.SetActive(true);
                m_SetupMenuRoot.gameObject.SetActive(false);
                if (m_TimerObject != null)
                    m_TimerObject.SetActive(true);
                StartCoroutine(m_PiecesRootScaleAttribute.PlaySequence(m_PiecesCloseScale, Vector3.one, InDuration * .25f));
            }));
        }

        public override void OnTransitionIn(float time)
        {
            base.OnTransitionIn(time);
        }

        public override void OnEndTransitionIn()
        {
            base.OnEndTransitionIn();
            if (m_ToggleOptions != null)
                m_ToggleOptions.enabled = true;
            m_BoardStartAnimator.CloseInsetInstant();
        }

        public override void OnBeginTransitionOut()
        {
            base.OnBeginTransitionOut();

            if (m_ToggleOptions != null)
                m_ToggleOptions.enabled = false;

            m_BoardRoot.gameObject.SetActive(true);
            m_PiecesRoot.gameObject.SetActive(true);
            m_SetupMenuRoot.gameObject.SetActive(true);

            m_BoardStartAnimator.OpenInsetInstant();
            m_BoardStartAnimator.EndGameEnableFountainVisualizer(); // start fountain immediately so by the time it tweens the tiles to it, there are already particles
            m_PiecesRootScaleAttribute.animationCurve = transitionCurve;
            StartCoroutine(m_PiecesRootScaleAttribute.PlaySequence((float3)Vector3.one, m_PiecesCloseScale, OutDuration * .25f, () =>
             {
                 m_BoardCanvas.gameObject.SetActive(false);
                 m_PiecesRoot.gameObject.SetActive(false);
                 // m_SetupMenuRoot.gameObject.SetActive(true);

                 StartCoroutine(m_BoardStartAnimator.EndGame(duration: OutDuration * .7f)); //todo scale duration on endgame
             }));
        }

        public override void OnTransitionOut(float time)
        {
            base.OnTransitionOut(time);
        }

        public override void OnEndTransitionOut()
        {
            base.OnEndTransitionOut();
            StopAllCoroutines();
            m_PiecesRootScaleAttribute.Initialize(m_PiecesCloseScale);
            m_BoardStartAnimator.EndInstant();

            m_BoardRoot.gameObject.SetActive(false);
            m_PiecesRoot.gameObject.SetActive(false);
            m_SetupMenuRoot.gameObject.SetActive(true);

            if (m_ToggleOptions != null)
                m_ToggleOptions.enabled = true;
            m_ChessBoardUI.SetStartButtonsActive(true);

            // Reset any board rotation of finishing transition.
            // Should happen instantly and board is symmetrical so you shouldn't be able to see.
            m_ChessBoard.boardRotation.Value = 0;
        }
    }
}
