using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.XR.Content.Utils;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityLabs.Slices.Passthrough
{
    public class PassthroughVolume : MonoBehaviour
    {
        const float k_NormalTransitionSpeed = 1.5f;

        Transform m_Transform;
        Transform m_RendererTransform;
        Vector3 m_VisibleMRScale;
        Vector3 m_HiddenScale;
        Vector3 m_TargetScale;
        Vector3 m_NonCalibratingTargetScale;
        float m_TransitionSpeed;
        bool m_Visible;
        bool m_InflateVolume;
        bool m_Calibrating;
#pragma warning disable CS0618 // Type or member is obsolete

        Vector3TweenableVariable m_TableBaseLocalScaleAttribute = new Vector3TweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete

        float m_BaseLocalScaleAnimationSpeedScalar;
        Vector3 m_RendererVisibleLocalScale;
        float m_RendererLocalScaleAnimationSpeedScalar;

        [SerializeField]
        float m_SpeedMultiplier = 1f;

        [SerializeField]
        Renderer m_Renderer = null;

        [SerializeField]
        Vector3 m_InflatedScale = new Vector3(20f, 20f, 20f);

        readonly BindingsGroup m_BindingsGroup = new BindingsGroup();

        public bool visible
        {
            set
            {
                m_Visible = value;

                m_TargetScale = m_Visible ? m_VisibleMRScale : m_HiddenScale;
                m_NonCalibratingTargetScale = m_TargetScale;
                // Slower fade in, vs faster fade out
                m_TransitionSpeed = m_Visible ? k_NormalTransitionSpeed : 9f;

                // Automatically deflate if hiding volume
                if (!m_Visible)
                    inflateVolume = false;

                m_TableBaseLocalScaleAttribute.target = m_Visible ? m_RendererVisibleLocalScale : Vector3.zero;
                m_RendererLocalScaleAnimationSpeedScalar = m_Visible ? 10f : 6f;
            }
        }

        public bool inflateVolume
        {
            set
            {
                if (m_InflateVolume == value)
                    return;

                m_InflateVolume = value;

                if (m_Visible)
                {
                    m_TransitionSpeed = m_InflateVolume ? 0.125f : 6f;
                    m_TargetScale = m_InflateVolume ? m_InflatedScale : m_VisibleMRScale;
                    m_NonCalibratingTargetScale = m_TargetScale;
                }
                else
                {
                    m_TargetScale = m_HiddenScale;
                    m_NonCalibratingTargetScale = m_TargetScale;
                    m_Transform.localScale = m_HiddenScale;
                }
            }
        }

        public bool calibrating
        {
            set
            {
                m_Calibrating = value;
                m_TargetScale = m_Calibrating ? m_HiddenScale : m_NonCalibratingTargetScale;
                m_TransitionSpeed = m_Calibrating ? 15f : k_NormalTransitionSpeed;
            }
        }

        void Awake()
        {
            m_Transform = transform;
            m_VisibleMRScale = m_Transform.localScale;
            m_TargetScale = m_VisibleMRScale;
            m_HiddenScale = new Vector3(m_VisibleMRScale.x, 0f, m_VisibleMRScale.z);
            m_Transform.localScale = m_HiddenScale;
            visible = true;

            m_RendererTransform = m_Renderer.transform;
            m_RendererVisibleLocalScale = m_RendererTransform.localScale;
            m_TableBaseLocalScaleAttribute.Initialize(Vector3.zero);
            m_BindingsGroup.AddBinding(m_TableBaseLocalScaleAttribute.SubscribeAndUpdate(newScale => m_RendererTransform.localScale = newScale));
        }

        void Update()
        {
            m_Transform.localScale = Vector3.Lerp(m_Transform.localScale, m_TargetScale, Time.deltaTime * m_TransitionSpeed * m_SpeedMultiplier);

            if (m_InflateVolume && m_Transform.localScale.y < 1)
            {
                // Force the height of the volume to reach its target height if going from AR mode to VR mode, preventing the volume from looking too short in height when revealing
                var increasedHeightSeekingScale = Mathf.Lerp(m_Transform.localScale.y, m_TargetScale.y, Time.deltaTime * 8f);
                m_Transform.localScale = new Vector3(m_Transform.localScale.x, increasedHeightSeekingScale, m_Transform.localScale.z);
            }

            m_TableBaseLocalScaleAttribute.HandleTween(Time.deltaTime * m_RendererLocalScaleAnimationSpeedScalar);
        }
    }
}
