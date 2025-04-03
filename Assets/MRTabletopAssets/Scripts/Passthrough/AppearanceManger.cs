using System;
using Unity.XR.CoreUtils.Bindings;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;
using UnityLabs.Slices.Passthrough;

namespace XRMultiplayer
{
    /// <summary>
    /// Manages the overall appearance including AR/MR/VR passthrough state and hand visuals.
    /// </summary>
    public class AppearanceManger : MonoBehaviour
    {
        [SerializeField]
        PassthroughClipSource m_HandMaskClipSourceVolume = null;

        [SerializeField]
        PassthroughVolume m_PassthroughVolume = null;

        [SerializeField]
        HandVisuals m_HandVisuals = null;

        readonly BindingsGroup m_BindingsGroup = new BindingsGroup();

        BindableEnum<PassthroughState> m_PassthroughState = new BindableEnum<PassthroughState>(PassthroughState.AR);
        public IReadOnlyBindableVariable<PassthroughState> passThroughState => m_PassthroughState;

        /// <summary>
        /// PassthroughState determines if we are in AR, MR, or VR mode.
        /// </summary>
        public enum PassthroughState
        {
            AR,
            MR,
            VR
        }

        /// <summary>
        /// Gets or sets the current passthrough state, and updates the visuals accordingly.
        /// </summary>
        public PassthroughState passthroughState
        {
            get => m_PassthroughState.Value;
            set
            {
                Debug.Log($"[AppearanceManager] Changing passthrough state to: {value}");
                m_PassthroughState.Value = value;
                switch (value)
                {
                    case PassthroughState.AR:
                        SetARState();
                        break;
                    case PassthroughState.MR:
                        SetMRState();
                        break;
                    case PassthroughState.VR:
                        SetVRState();
                        break;
                }
            }
        }

        void Awake()
        {
            switch (m_PassthroughState.Value)
            {
                case PassthroughState.AR:
                    SetARState();
                    break;
                case PassthroughState.MR:
                    SetMRState();
                    break;
                case PassthroughState.VR:
                    SetVRState();
                    break;
            }
        }

        /// <summary>
        /// Helper function to set passthrough state via int (0=AR,1=MR,2=VR).
        /// </summary>
        public void SetPassthroughState(int state)
        {
            Debug.Log($"[AppearanceManager] SetPassthroughState called with int: {state}");
            passthroughState = (PassthroughState)state;
        }

        void OnDestroy()
        {
            m_BindingsGroup.Clear();
        }

        void Update()
        {
            UpdateTweenables();
        }

        void UpdateTweenables()
        {
            // Debug to see if any tweening updates run.
            // If needed, add more debug outputs here.
        }

        void SetARState()
        {
            Debug.Log("[AppearanceManager] Entering AR state");
            if (m_HandMaskClipSourceVolume != null)
                m_HandMaskClipSourceVolume.clipSourceEnabled = false;
            if (m_HandVisuals != null)
            {
                m_HandVisuals.visible = true;
                m_HandVisuals.displayARModePassthrougHands = true;
            }
            if (m_PassthroughVolume != null)
                m_PassthroughVolume.visible = false;
        }

        void SetMRState()
        {
            Debug.Log("[AppearanceManager] Entering MR state");
            if (m_HandMaskClipSourceVolume != null)
            {
                m_HandMaskClipSourceVolume.clipSourceEnabled = true;
                m_HandMaskClipSourceVolume.inflateClipSource = false;
            }
            if (m_HandVisuals != null)
            {
                m_HandVisuals.visible = true;
                m_HandVisuals.displayARModePassthrougHands = false;
            }
            if (m_PassthroughVolume != null)
            {
                m_PassthroughVolume.visible = true;
                m_PassthroughVolume.inflateVolume = false;
            }
        }

        void SetVRState()
        {
            Debug.Log("[AppearanceManager] Entering VR state");
            if (m_HandMaskClipSourceVolume != null)
            {
                m_HandMaskClipSourceVolume.clipSourceEnabled = true;
                m_HandMaskClipSourceVolume.inflateClipSource = true;
            }
            if (m_HandVisuals != null)
            {
                m_HandVisuals.visible = false;
                m_HandVisuals.displayARModePassthrougHands = false;
            }
            if (m_PassthroughVolume != null)
            {
                m_PassthroughVolume.visible = true;
                m_PassthroughVolume.inflateVolume = true;
            }
        }
    }
}
