using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;
using UnityLabs.Slices.Transitions;

namespace UnityLabs.Slices.Games.Chess
{
    /// <summary>
    /// The first transition which opens the chess board, landing
    /// on the setup menu with the start button. Also configures a bunch of
    /// initial visibility state;
    /// </summary>
    public class ChessSetupTransition : Transition
    {
        [SerializeField]
        ChessBoardUI m_ChessBoardUI = null;

        [SerializeField]
        Transform m_SetupMenuRoot = null;

        [SerializeField]
        CanvasGroup m_CanvasGroup = null;

        [SerializeField]
        ChessBoardGenerator m_BoardGenerator = null;

        [SerializeField]
        ChessBoardStartAnimator m_BoardStartAnimator = null;

#pragma warning disable CS0618 // Type or member is obsolete
        FloatTweenableVariable m_CanvasGroupFadeAttribute = new FloatTweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete


        readonly BindingsGroup m_BindingGroup = new BindingsGroup();

        protected override void OnEnable()
        {
            base.OnEnable();
            m_BindingGroup.AddBinding(m_CanvasGroupFadeAttribute.Subscribe(alpha => m_CanvasGroup.alpha = alpha));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_BindingGroup.Clear();
        }

        public override void OnBeginTransitionIn()
        {
            base.OnBeginTransitionIn();

            m_BoardStartAnimator.EndInstant();
            m_SetupMenuRoot.gameObject.SetActive(true);
            m_ChessBoardUI.SetStartButtonsActive(false);
            m_CanvasGroupFadeAttribute.animationCurve = transitionCurve;

            StartCoroutine(m_CanvasGroupFadeAttribute.PlaySequence(0, 1, InDuration * .5f, () =>
             {
                 StartCoroutine(m_BoardStartAnimator.OpenInset(duration: InDuration * .5f));
             }));
        }

        public override void OnTransitionIn(float time)
        {
            base.OnTransitionIn(time);
        }

        public override void OnEndTransitionIn()
        {
            base.OnEndTransitionIn();
            m_ChessBoardUI.SetStartButtonsActive(true);
            m_BoardStartAnimator.EnableFountainVisualizer(true);
            m_BoardGenerator.gameObject.SetActive(true);
        }

        public override void OnBeginTransitionOut()
        {
            base.OnBeginTransitionOut();
            m_ChessBoardUI.SetStartButtonsActive(false);
            StartCoroutine(m_CanvasGroupFadeAttribute.PlaySequence(1, 0, InDuration));
        }

        public override void OnTransitionOut(float time)
        {
            base.OnTransitionOut(time);
        }

        public override void OnEndTransitionOut()
        {
            base.OnEndTransitionOut();
            StopAllCoroutines();

            m_ChessBoardUI.SetStartButtonsActive(false);

            m_BoardStartAnimator.EndInstant();
            m_BoardStartAnimator.CloseInsetInstant();
            m_BoardStartAnimator.EnableFountainVisualizer(false);

            m_BoardGenerator.gameObject.SetActive(false);
            m_SetupMenuRoot.gameObject.SetActive(false);
        }
    }
}
