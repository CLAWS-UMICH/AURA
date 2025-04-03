using UnityEngine;
using UnityLabs.Slices.Systems;
using XRMultiplayer;

namespace UnityLabs.Slices.Passthrough
{
    /// <summary>
    /// Manages the visuals for hand rendering, applying passthrough and related effects to all shared materials.
    /// </summary>
    public class HandVisuals : MonoBehaviour
    {
        [SerializeField] Renderer m_LeftHandRenderer;
        [SerializeField] Renderer m_RightHandRenderer;
        [SerializeField] AppearanceManger m_AppearanceManger;

        Material[] m_LeftHandMaterials;
        Material[] m_RightHandMaterials;

        float m_CurrentPassthroughOpacity;
        float m_TargetPassthroughOpacity;

        float m_CurrentNonPassthroughOpacity;
        float m_TargetNonPassthroughOpacity;

        bool m_Visible;
        bool m_DisplayARModePassthrougHands;
        float m_PassthroughHandSpeedScalar = 8f;

        readonly int m_NonPassthroughOpacityPropertyID = Shader.PropertyToID("_ARPassthroughAlpha");
        readonly int m_PassthroughOpacityPropertyID = Shader.PropertyToID("_PassthroughAlpha");
        readonly int m_PassthroughBoundaryCrossAlphaOpacityPropertyID = Shader.PropertyToID("_PassthroughBoundaryCrossAlpha");
        readonly int m_ModePropertyID = Shader.PropertyToID("_Mode");

        public bool visible
        {
            set
            {
                m_Visible = value;
                m_TargetPassthroughOpacity = value ? 1f : 0f;
            }
        }

        public bool displayARModePassthrougHands
        {
            set
            {
                m_DisplayARModePassthrougHands = value;
                m_TargetNonPassthroughOpacity = m_DisplayARModePassthrougHands ? 1f : 0f;
            }
        }

        void Start()
        {
            // Instead of grabbing a single slot, we take all shared materials.
            m_LeftHandMaterials = m_LeftHandRenderer.sharedMaterials;
            m_RightHandMaterials = m_RightHandRenderer.sharedMaterials;
        }

        void Update()
        {
            if (m_AppearanceManger == null)
            {
                return;
            }

            // Opacity lerp
            m_CurrentPassthroughOpacity = Mathf.Lerp(m_CurrentPassthroughOpacity, m_TargetPassthroughOpacity, Time.deltaTime * m_PassthroughHandSpeedScalar);
            m_CurrentNonPassthroughOpacity = Mathf.Lerp(m_CurrentNonPassthroughOpacity, m_TargetNonPassthroughOpacity, Time.deltaTime * m_PassthroughHandSpeedScalar);

            var currentState = m_AppearanceManger.passThroughState.Value;

            // Map state to shader mode
            int mode;
            switch (currentState)
            {
                case AppearanceManger.PassthroughState.AR:
                    mode = 1; // AR
                    break;
                case AppearanceManger.PassthroughState.MR:
                    mode = 2; // MR
                    break;
                default:
                    mode = 0; // VR
                    break;
            }

            // Apply effects to all left hand materials
            for (int i = 0; i < m_LeftHandMaterials.Length; i++)
            {
                m_LeftHandMaterials[i].SetFloat(m_PassthroughOpacityPropertyID, m_CurrentPassthroughOpacity);
                m_LeftHandMaterials[i].SetFloat(m_NonPassthroughOpacityPropertyID, m_CurrentNonPassthroughOpacity);
                m_LeftHandMaterials[i].SetInt(m_ModePropertyID, mode);
            }

            // Apply effects to all right hand materials
            for (int i = 0; i < m_RightHandMaterials.Length; i++)
            {
                m_RightHandMaterials[i].SetFloat(m_PassthroughOpacityPropertyID, m_CurrentPassthroughOpacity);
                m_RightHandMaterials[i].SetFloat(m_NonPassthroughOpacityPropertyID, m_CurrentNonPassthroughOpacity);
                m_RightHandMaterials[i].SetInt(m_ModePropertyID, mode);
            }
        }
    }
}
