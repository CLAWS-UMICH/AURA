using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityLabs.Slices.Transitions;
using UnityLabs.SmartUX.Interaction.Widgets;

namespace UnityLabs.Slices.Games.Chess
{
    public class ChessOptionsTransition : Transition
    {
        [SerializeField]
        Transform m_PiecesRoot = null;

        [SerializeField]
        Transform m_OptionsRoot = null;

        [SerializeField]
        XRBaseInteractable m_ResignButtonA = null;

        [SerializeField]
        XRBaseInteractable m_ResignButtonB = null;

        [SerializeField]
        XRToggle m_ToggleOptions = null;

        [SerializeField]
        BoardJobVisualizer m_OptionsVisualzer = null;

        [SerializeField]
        float m_PiecesScaleDuration = .2f;

        const float PiecesClosedScale = .05f;
        readonly BindingsGroup m_BindingGroup = new();
        readonly AnimationCurve m_PiecesScaleYCurve = new();
        readonly AnimationCurve m_TileParticlesTweenCurve = new();

        public override bool interruptable => true;

        void Awake()
        {
            m_PiecesScaleYCurve.AddKey(0, 1);
            m_PiecesScaleYCurve.AddKey(m_PiecesScaleDuration, PiecesClosedScale);
            m_TileParticlesTweenCurve.AddKey(m_PiecesScaleDuration, 0f);
            m_TileParticlesTweenCurve.AddKey(1f, 1f);

            m_ResignButtonA.enabled = false;
            m_ResignButtonB.enabled = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_BindingGroup.Clear();
        }

        void Transition(float time)
        {
            float easedTime = CurveEasedTime(time);

            // Needs to be a better way to do this kind of logic.
            m_PiecesRoot.localScale = new Vector3(1, m_PiecesScaleYCurve.Evaluate(easedTime), 1);
            m_PiecesRoot.gameObject.SetActive(m_PiecesRoot.localScale.y > PiecesClosedScale);

            m_OptionsVisualzer.SetWeight(easedTime);
        }

        public override void OnBeginTransitionIn()
        {
            base.OnBeginTransitionIn();

            m_ToggleOptions.enabled = false;
            m_ResignButtonA.enabled = false;
            m_ResignButtonB.enabled = false;

            m_OptionsRoot.gameObject.SetActive(true);

            m_OptionsVisualzer.Initialize();
            Transition(0);
        }

        public override void OnTransitionIn(float time)
        {
            base.OnTransitionIn(time);
            Transition(time);
        }

        public override void OnEndTransitionIn()
        {
            base.OnEndTransitionIn();

            m_ToggleOptions.enabled = true;
            m_ResignButtonA.enabled = true;
            m_ResignButtonB.enabled = true;
            Transition(1);
        }

        public override void OnBeginTransitionOut()
        {
            base.OnBeginTransitionOut();
            m_OptionsVisualzer.BeginShutdown();
            m_ToggleOptions.enabled = false;
            m_ResignButtonA.enabled = false;
            m_ResignButtonB.enabled = false;
        }

        public override void OnTransitionOut(float time)
        {
            base.OnTransitionOut(time);
            Transition(time);
        }

        public override void OnEndTransitionOut()
        {
            base.OnEndTransitionOut();
            Transition(0);

            m_ToggleOptions.enabled = true;
            m_ResignButtonA.enabled = false;
            m_ResignButtonB.enabled = false;

            m_OptionsVisualzer.ShutDown();

            m_OptionsRoot.gameObject.SetActive(false);
        }
    }
}
