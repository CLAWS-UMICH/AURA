using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityLabs.Slices.Transitions
{
    /// <summary>
    /// Push/Pop Transitions to this TransitionStack in order to transition them in/out.
    /// Transitions will be sequentially transitioned in or out, one after another, to reach the current
    /// transition that should be displayed. So you can push/pop multiple frames simultaneously in one cycle
    /// and this will manage displaying their transitions one after the other.
    ///
    /// If you need transitions to occur in a Queue, for a pop-up notification like scheme, use the
    /// TransitionQueue component, which does not currently exist, but will at some point probably.
    /// </summary>
    public class TransitionStack : MonoBehaviour
    {
        [SerializeField]
        Transition m_PushTransitionOnStart;

        List<Transition> m_Transitions = new List<Transition>();

        float m_CurrentTransitionTime;
        Transition m_CurrentTransitionOut;
        Transition m_CurrentTransitionIn;
        int m_CurrentTransitionIndex = -1;

        int nextTransitionIndex => m_CurrentTransitionIndex + 1;

        /// <summary>
        /// Push a transition to occur. If multiple pushes occur, transitions will be queued and occur sequentially.
        ///
        /// This will have a bug in the logic where if you are popping a long chain of Transitions, then push one while that is happening something bad may happen, need solution still.
        /// </summary>
        public Task<Transition.TransitionState> PushTransition(Transition transition)
        {
            if (!m_Transitions.Contains(transition))
            {
                m_Transitions.Add(transition);
                return transition.inTask;
            }

            // If the currently pushed transition is already transitioning in, update index
            // to that transition. This may happen in rapid clicking of a toggle.
            if (m_CurrentTransitionIn == transition)
            {
                m_CurrentTransitionIndex = m_Transitions.IndexOf(transition);
            }

            return Task.FromResult(Transition.TransitionState.TransitionedOut);
        }

        /// <summary>
        /// Pops the passed in transition, as well as all transitions on top of the passed in transition.
        /// </summary>
        public Task<Transition.TransitionState> PopTransition(Transition transition)
        {
            int popIndex = m_Transitions.IndexOf(transition);
            if (popIndex == -1)
            {
                Debug.LogWarning($"Stack does not contain transition: {transition} " + " count " + m_Transitions.Count);
                return Task.FromResult(Transition.TransitionState.TransitionedOut);
            }

            for (int i = m_Transitions.Count - 1; i >= popIndex; i--)
            {
                // Remove all frames higher in the stack that are already transitioned out since
                // they dont need to show their transition out animation. This state could occur if you push
                // multiple frames quickly and before they can all transition in, you pop to a lower one.
                if (m_Transitions[i].state == Transition.TransitionState.TransitionedOut)
                {
                    m_Transitions[i].CancelTasks();
                    m_Transitions.RemoveAt(i);
                }
                // If it so happens a frame above or equal is already transitioning out, then probably due to rapid
                // clicking of the user, they pushed, popped, pushed and popped again before transition finished
                // so remove from transitions as it shouldn't be there
                else if (m_Transitions[i].state == Transition.TransitionState.TransitioningOut)
                {
                    m_Transitions.RemoveAt(i);
                }
            }

            if (popIndex > m_CurrentTransitionIndex)
            {
                // SmartLogger.Log($"Skipping Pop {transition}. Already popping to lower transition index!");
                return Task.FromResult(Transition.TransitionState.TransitionedOut);
            }

            m_CurrentTransitionIndex = popIndex - 1;
            return transition.outTask;
        }

        /// <summary>
        /// Pop to given transition immediately. Immediately interrupts any resets any transition higher in stack.
        /// </summary>
        public bool PopTransitionImmediate(Transition transition)
        {
            int popIndex = m_Transitions.IndexOf(transition);
            if (popIndex == -1)
            {
                Debug.LogWarning($"Stack does not contain transition: {transition}");
                return false;
            }

            m_CurrentTransitionIn = null;
            m_CurrentTransitionOut = null;

            for (int i = m_Transitions.Count - 1; i >= popIndex; i--)
            {
                m_Transitions[i].OnEndTransitionOut();
                m_Transitions[i].CancelTasks();
                m_Transitions.RemoveAt(i);
            }

            m_CurrentTransitionIndex = popIndex - 1;
            return true;
        }

        public bool ContainsTransition(Transition transition)
        {
            return m_Transitions.Contains(transition);
        }

        void Start()
        {
            if (m_PushTransitionOnStart != null)
            {
                PushTransition(m_PushTransitionOnStart);
            }
        }

        void Update()
        {
            if (m_Transitions.Count == 0) return;

            if (m_CurrentTransitionOut != null) TransitionOut();
            else if (m_CurrentTransitionIn != null) TransitionIn();
            // check for transition in
            else if (nextTransitionIndex < m_Transitions.Count && m_Transitions[nextTransitionIndex].state == Transition.TransitionState.TransitionedOut)
            {
                if (m_CurrentTransitionIn == null)
                {
                    m_CurrentTransitionIn = m_Transitions[nextTransitionIndex];
                    if (nextTransitionIndex < m_Transitions.Count) m_CurrentTransitionIndex = nextTransitionIndex;
                }
            }
            // check for transition out, if m_CurrentTransitionIndex == m_Transition.Count that means it is at the
            // last transition already, so  m_CurrentTransitionIndex is less than that, then transition those higher in stack
            else if (m_CurrentTransitionIndex < m_Transitions.Count - 1)
            {
                if (m_CurrentTransitionOut == null)
                {
                    m_CurrentTransitionOut = m_Transitions[m_Transitions.Count - 1];
                    m_Transitions.Remove(m_CurrentTransitionOut);
                }
            }
        }

        void TransitionIn()
        {
            if (m_CurrentTransitionIn.state != Transition.TransitionState.TransitioningIn)
            {
                m_CurrentTransitionTime = 0;
                m_CurrentTransitionIn.OnBeginTransitionIn();
            }
            else if (m_CurrentTransitionTime < 1f)
            {
                m_CurrentTransitionTime += Time.deltaTime / m_CurrentTransitionIn.InDuration;
                m_CurrentTransitionIn.OnTransitionIn(m_CurrentTransitionTime);
            }
            else if (m_CurrentTransitionIn.state != Transition.TransitionState.TransitionedIn)
            {
                m_CurrentTransitionTime = 1;
                m_CurrentTransitionIn.OnEndTransitionIn();
                m_CurrentTransitionIn = null;
            }
        }

        void TransitionOut()
        {
            if (m_CurrentTransitionOut.state != Transition.TransitionState.TransitioningOut)
            {
                m_CurrentTransitionTime = 1;
                m_CurrentTransitionOut.OnBeginTransitionOut();
            }
            else if (m_CurrentTransitionTime > 0f)
            {
                m_CurrentTransitionTime -= Time.deltaTime / m_CurrentTransitionOut.OutDuration;
                m_CurrentTransitionOut.OnTransitionOut(m_CurrentTransitionTime);
            }
            else if (m_CurrentTransitionOut.state != Transition.TransitionState.TransitionedOut)
            {
                m_CurrentTransitionTime = 0;
                m_CurrentTransitionOut.OnEndTransitionOut();
                m_CurrentTransitionOut = null;
            }
        }
    }
}
