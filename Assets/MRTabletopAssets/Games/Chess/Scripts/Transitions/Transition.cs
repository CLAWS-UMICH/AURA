using System;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Datums;
using UnityEngine;
using UnityEngine.Events;

namespace UnityLabs.Slices.Transitions
{
    /// <summary>
    /// Implement to define how a set of components would transition in and out.
    /// Submit to TransitionStack to initiate.
    /// </summary>
    public class Transition : MonoBehaviour
    {
        [SerializeField]
        FloatDatumProperty m_InDurationRef = new FloatDatumProperty(1);

        [SerializeField]
        FloatDatumProperty m_OutDurationRef = new FloatDatumProperty(1);

        AnimationCurve m_TransitionCurveRef = AnimationCurve.EaseInOut(0, 0, 1f, 1f);

        [SerializeField]
        UnityEvent m_BeginTransitionIn;

        [SerializeField]
        FloatUnityEvent m_TransitionIn;

        [SerializeField]
        UnityEvent m_EndTransitionIn;

        [SerializeField]
        UnityEvent m_BeginTransitionOut;

        [SerializeField]
        FloatUnityEvent m_TransitionOut;

        [SerializeField]
        UnityEvent m_EndTransitionOut;

        public enum TransitionState
        {
            TransitionedOut,
            TransitioningOut,
            TransitioningIn,
            TransitionedIn,
        }

        public event Action BeginTransitionIn;
        public event Action<float> TransitionIn;
        public event Action EndTransitionIn;

        public event Action BeginTransitionOut;
        public event Action<float> TransitionOut;
        public event Action EndTransitionOut;

        TaskCompletionSource<TransitionState> m_InTCS;
        TaskCompletionSource<TransitionState> m_OutTCS;

        public TransitionState state { get; private set; } = TransitionState.TransitionedOut;

        public Task<TransitionState> inTask => (m_InTCS ??= new TaskCompletionSource<TransitionState>()).Task;
        public Task<TransitionState> outTask => (m_OutTCS ??= new TaskCompletionSource<TransitionState>()).Task;

        public float InDuration => m_InDurationRef;
        public float OutDuration => m_OutDurationRef;

        public AnimationCurve transitionCurve => m_TransitionCurveRef;

        /// <summary>
        /// Denotes whether this transition needs to go fully from a beginning state to
        /// an ending state in order to not break anything, or if it can be interrupted
        /// midway through a transition and reversed the other direction.
        ///
        /// Currently this does not actually do anything, but putting here because
        /// this will probably become important concept. Some transitions may just be too
        /// complex to implement in an interruptible/reversible way. Or maybe not...
        /// </summary>
        public virtual bool interruptable => false;

        protected virtual void OnEnable()
        {
            BeginTransitionIn += m_BeginTransitionIn.Invoke;
            TransitionIn += m_TransitionIn.Invoke;
            EndTransitionIn += m_EndTransitionIn.Invoke;
            BeginTransitionOut += m_BeginTransitionOut.Invoke;
            TransitionOut += m_TransitionOut.Invoke;
            EndTransitionOut += m_EndTransitionOut.Invoke;
        }

        protected virtual void OnDisable()
        {
            BeginTransitionIn -= m_BeginTransitionIn.Invoke;
            TransitionIn -= m_TransitionIn.Invoke;
            EndTransitionIn -= m_EndTransitionIn.Invoke;
            BeginTransitionOut -= m_BeginTransitionOut.Invoke;
            TransitionOut -= m_TransitionOut.Invoke;
            EndTransitionOut -= m_EndTransitionOut.Invoke;
        }

        public void CancelTasks()
        {
            m_OutTCS?.SetCanceled();
            m_OutTCS = null;
            m_InTCS?.SetCanceled();
            m_InTCS = null;
        }

        public virtual void OnBeginTransitionIn()
        {
            state = TransitionState.TransitioningIn;
            BeginTransitionIn?.Invoke();
        }

        /// <summary>
        /// Called every update from of the transition receiving a time scale
        /// which will go from 0 to 1 within the defined TransitionDuration.
        /// </summary>
        public virtual void OnTransitionIn(float time)
        {
            m_TransitionIn?.Invoke(time);
            TransitionIn?.Invoke(time);
        }

        public virtual void OnEndTransitionIn()
        {
            state = TransitionState.TransitionedIn;
            EndTransitionIn?.Invoke();
            m_InTCS?.SetResult(state);
            m_InTCS = null;
        }

        public virtual void OnBeginTransitionOut()
        {
            state = TransitionState.TransitioningOut;
            BeginTransitionOut?.Invoke();
        }

        /// <summary>
        /// Called every update from of the transition receiving a time scale
        /// which will go from 0 to 1 within the defined TransitionDuration.
        /// </summary>
        public virtual void OnTransitionOut(float time)
        {
            TransitionOut?.Invoke(time);
        }

        /// <summary>
        /// Called at the end of transitioning out. This single call should ensure all
        /// state changed is cleaned up because if a user called PopTransitionImmediate in
        /// TransitionStack then only this method will be called to clean up the transition.
        ///
        /// If you started Coroutines then call StopAllCorountines!
        ///
        /// Should there maybe also be a 'cleanup' method? Redundant?
        /// </summary>
        public virtual void OnEndTransitionOut()
        {
            state = TransitionState.TransitionedOut;
            EndTransitionOut?.Invoke();
            m_OutTCS?.SetResult(state);
            m_OutTCS = null;
        }

        protected float CurveEasedTime(float time)
        {
            return m_TransitionCurveRef.Evaluate(time);
        }
    }
}
