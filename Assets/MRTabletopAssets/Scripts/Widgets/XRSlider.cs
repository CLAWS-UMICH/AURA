using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityLabs.SmartUX.Interaction.Widgets
{
    public class XRSlider : XRBaseInteractable
    {
        [Header("Slider Anchors")]
        [SerializeField] private Transform m_SliderHandleRoot;
        [SerializeField] private Transform m_SliderStart;
        [SerializeField] private Transform m_SliderEnd;

        [Header("Snapping Config")]
        [SerializeField] [Range(1, 10)] private int m_NbSnappingPoints = 3;
        [SerializeField] private bool m_BehaveAsToggle = false;
        [SerializeField] private float m_ToggleHoldTimeThreshold = 0.5f;

        [Header("Events")]
        [SerializeField] private UnityEvent<float> m_OnFillAmountChanged;
        [SerializeField] private UnityEvent<int> m_OnSnapIndexChanged;

        public BindableVariable<float> floatFillAmt { get; } = new BindableVariable<float>();
        public BindableVariable<int> snapIndex { get; } = new BindableVariable<int>();

        private Vector3 m_LocalLineStart;
        private Vector3 m_LocalLineEnd;
        private Vector3 m_SliderHandleTarget;
        private float m_InteractionStartTime;
        private int m_StartSnapIndex;
        private bool m_IsInteracting;
        private Vector3 m_InitialAttachLocalPosition;

        protected override void Awake()
        {
            base.Awake();
            CaptureLineEnds();
            m_SliderHandleTarget = transform.InverseTransformPoint(m_SliderHandleRoot.position);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(OnSelectEntered);
            selectExited.AddListener(OnSelectExited);
            snapIndex.SubscribeAndUpdate(OnSnapIndexChanged);
            floatFillAmt.SubscribeAndUpdate(OnFillAmountChanged);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(OnSelectEntered);
            selectExited.RemoveListener(OnSelectExited);
            base.OnDisable();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (m_IsInteracting && isSelected)
                {
                    UpdateTarget(GetLocalTargetPosition());
                }

                m_SliderHandleRoot.localPosition = Vector3.Lerp(m_SliderHandleRoot.localPosition, m_SliderHandleTarget, Time.deltaTime * 8f);
            }
        }

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        private void OnSelectEntered(SelectEnterEventArgs args)

        {
            m_IsInteracting = true;
            m_StartSnapIndex = ComputeHandleIndex();
            m_InteractionStartTime = Time.time;

            // Store the initial attach point in local space
            m_InitialAttachLocalPosition = transform.InverseTransformPoint(args.interactorObject.GetAttachTransform(this).position);
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            m_IsInteracting = false;

            if (snapIndex.Value > -1)
            {
                if (m_BehaveAsToggle && m_StartSnapIndex == snapIndex.Value && m_NbSnappingPoints == 2)
                {
                    float interactionDuration = Time.time - m_InteractionStartTime;
                    if (interactionDuration < m_ToggleHoldTimeThreshold)
                    {
                        snapIndex.Value = (snapIndex.Value == 0) ? 1 : 0;
                    }
                }

                m_SliderHandleTarget = ComputeSnappingPoint(m_LocalLineStart, m_LocalLineEnd, snapIndex.Value);
            }
        }
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

        private Vector3 GetLocalTargetPosition()
        {
            if (firstInteractorSelecting == null)
                return m_SliderHandleTarget;

            var attachTransform = firstInteractorSelecting.GetAttachTransform(this);
            Vector3 worldAttachPoint = attachTransform.position;
            Vector3 localAttachPoint = transform.InverseTransformPoint(worldAttachPoint);

            // Calculate the delta movement from the initial attach point
            Vector3 localDelta = localAttachPoint - m_InitialAttachLocalPosition;

            return m_SliderHandleTarget + localDelta;
        }

        private void UpdateTarget(Vector3 targetLocalPos, bool snapToTarget = false)
        {
            m_SliderHandleTarget = ClampTarget(targetLocalPos, out var fillAmt);

            if (snapToTarget)
            {
                m_SliderHandleRoot.localPosition = m_SliderHandleTarget;
            }

            snapIndex.Value = ComputeHandleIndex();
            floatFillAmt.Value = fillAmt;
        }

        private void OnSnapIndexChanged(int newIndex)
        {
            m_OnSnapIndexChanged?.Invoke(newIndex);
        }

        private void OnFillAmountChanged(float newFillAmt)
        {
            m_OnFillAmountChanged?.Invoke(newFillAmt);
        }

        private void CaptureLineEnds()
        {
            m_LocalLineStart = transform.InverseTransformPoint(m_SliderStart.position);
            m_LocalLineEnd = transform.InverseTransformPoint(m_SliderEnd.position);
        }

        private int ComputeHandleIndex()
        {
            return ComputeSnappingIndex(m_SliderHandleTarget);
        }

        private int ComputeSnappingIndex(Vector3 targetLocalSpace)
        {
            if (m_NbSnappingPoints < 2)
                return -1;

            float smallestSqrDist = float.MaxValue;
            int targetIndex = -1;

            for (int i = 0; i < m_NbSnappingPoints; i++)
            {
                var snappingPoint = ComputeSnappingPoint(m_LocalLineStart, m_LocalLineEnd, i);
                float sqrDist = Vector3.SqrMagnitude(snappingPoint - targetLocalSpace);
                if (sqrDist < smallestSqrDist)
                {
                    targetIndex = i;
                    smallestSqrDist = sqrDist;
                }
            }

            return targetIndex;
        }

        private Vector3 ComputeSnappingPoint(Vector3 start, Vector3 end, int index)
        {
            return Vector3.Lerp(start, end, (float)index / (m_NbSnappingPoints - 1f));
        }

        private Vector3 ClampTarget(Vector3 targetLocalPose, out float fillAmt)
        {
            Vector3 sliderDir = (m_LocalLineEnd - m_LocalLineStart).normalized;
            Vector3 localPoseOnLine = Vector3.Project(targetLocalPose - m_LocalLineStart, sliderDir) + m_LocalLineStart;

            float t = Vector3.Dot(localPoseOnLine - m_LocalLineStart, sliderDir) / Vector3.Dot(m_LocalLineEnd - m_LocalLineStart, sliderDir);
            fillAmt = Mathf.Clamp01(t);

            return Vector3.Lerp(m_LocalLineStart, m_LocalLineEnd, fillAmt);
        }

        public void SetSnapIndex(int index)
        {
            var snapLocalPos = ComputeSnappingPoint(m_LocalLineStart, m_LocalLineEnd, index);
            UpdateTarget(snapLocalPos, true);
        }

        private void OnDrawGizmos()
        {
            if (m_SliderStart != null && m_SliderEnd != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(m_SliderStart.position, m_SliderEnd.position);

                for (int i = 0; i < m_NbSnappingPoints; i++)
                {
                    Gizmos.DrawWireSphere(transform.TransformPoint(ComputeSnappingPoint(m_LocalLineStart, m_LocalLineEnd, i)), 0.01f);
                }
            }
        }

        private void OnValidate()
        {
            if (m_SliderStart != null && m_SliderEnd != null)
            {
                CaptureLineEnds();
            }
        }
    }
}
