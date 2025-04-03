using System;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.XR.Content.Utils;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace Transmutable.UI.Content
{
    public class VisualizerGravityWell : MonoBehaviour
    {
        [SerializeField]
        ParticleSystemForceField m_ForceField = null;

        public float effectRadius => m_ForceField.endRange;

        public float sqEffectRadius { get; private set; }

        [SerializeField]
        float m_ReleaseForce = -0.3f;

        [SerializeField]
        float m_GrabForce = 0.3f;

#pragma warning disable CS0618 // Type or member is obsolete
        Vector4TweenableVariable m_PoseDataAttribute = new Vector4TweenableVariable();
#pragma warning restore CS0618 // Type or member is obsolete


        readonly BindingsGroup m_BindingGroup = new BindingsGroup();

        public bool isLocallyControlled { get; set; } = false;

        void OnEnable()
        {
            sqEffectRadius = Mathf.Pow(effectRadius, 2);
        }

        void OnDisable()
        {
            m_BindingGroup.Clear();
        }

        void LateUpdate()
        {
            if (!isLocallyControlled)
            {
                m_PoseDataAttribute.HandleTween(Time.deltaTime * 15f);
            }
            OnPoseDataUpdated(m_PoseDataAttribute.Value);
        }

        public void UpdatePoseData(Vector4 handInputState)
        {
            // Directly update locally controlled state to improve interaction feel
            if (isLocallyControlled)
            {
                m_PoseDataAttribute.Initialize(handInputState);
            }
            else
            {
                m_PoseDataAttribute.target = handInputState;
            }
        }

        void OnPoseDataUpdated(Vector4 handInputState)
        {
            var gravity = m_ForceField.gravity;
            transform.position = new Vector3(handInputState.x, handInputState.y, handInputState.z);
            gravity.constant = Mathf.Lerp(m_ReleaseForce, m_GrabForce, handInputState.w);
            m_ForceField.gravity = gravity;
        }
    }
}
