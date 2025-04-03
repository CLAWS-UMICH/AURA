using UnityEngine;

namespace UnityLabs.Slices.Passthrough
{
    [ExecuteAlways]
    public class PassthroughClipSource : MonoBehaviour
    {
        readonly string kShaderKeyword = "_WorldToVolume";
        readonly string kPlayerRegionShaderKeyword = "_WorldToVolumePlayRegion";

        [SerializeField]
        float m_ShowSpeedScalar = 1.5f;

        [SerializeField]
        float m_HideSpeedScalar = 1f;

        [SerializeField]
        Transform m_PlayerRegionMatrixSource = null;

        Transform m_MatrixSource;
        Vector3 m_VisibleScale;
        Vector3 m_HiddenScale = Vector3.one * 0.001f; // Shrink/hide the volume when unused
        Vector3 m_TargetScale;
        Vector3 m_PlayerRegionTargetScale;
        float m_TransitionSpeed;

        public bool clipSourceEnabled
        {
            set
            {
                m_TargetScale = value ? m_VisibleScale : m_HiddenScale;
                m_TransitionSpeed = value ? m_ShowSpeedScalar : m_HideSpeedScalar;
                m_PlayerRegionTargetScale = value ? m_VisibleScale : m_VisibleScale * 3;
            }
        }

        public bool inflateClipSource
        {
            set
            {
                m_TargetScale = value ? m_VisibleScale * 10f : m_VisibleScale;
                m_TransitionSpeed = value ? m_HideSpeedScalar * 0.75f : m_ShowSpeedScalar;
            }
        }

        /// <summary>
        /// Indicates if we are currently transitioning (scaling) between states.
        /// If the volume scale is not close to its target scale, we consider that a transition is in progress.
        /// </summary>
        public bool IsTransitioning
        {
            get
            {
                // You can adjust the threshold as needed
                return (Vector3.Distance(m_MatrixSource.localScale, m_TargetScale) > 0.001f);
            }
        }

        void Awake()
        {
            m_MatrixSource = transform;
            m_VisibleScale = m_MatrixSource.localScale;
            m_TargetScale = m_VisibleScale;
            clipSourceEnabled = true;
        }

        void Update()
        {
            // Grow/shrink the MR-volume clipping source scale for gradual reveal/hide
            var newLocalScale = Vector3.Lerp(m_MatrixSource.localScale, m_TargetScale, Time.deltaTime * m_TransitionSpeed);
            m_MatrixSource.localScale = newLocalScale;
            Shader.SetGlobalMatrix(kShaderKeyword, m_MatrixSource.worldToLocalMatrix);

            // The offset player region
            newLocalScale = Vector3.Lerp(m_PlayerRegionMatrixSource.localScale, m_PlayerRegionTargetScale, Time.deltaTime * m_TransitionSpeed);
            m_PlayerRegionMatrixSource.localScale = newLocalScale;
            Shader.SetGlobalMatrix(kPlayerRegionShaderKeyword, m_PlayerRegionMatrixSource.worldToLocalMatrix);
        }
    }
}
