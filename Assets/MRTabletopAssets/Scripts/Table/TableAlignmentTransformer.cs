using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Samples.Hands;

namespace XRMultiplayer
{
    public enum CalibrationState
    {
        Inactive,
        TwoHandedFree,
        OneHandedLocked,
        OneHandedUnlocked // New state for unlocked movement
    }

    public class TableAlignmentTransformer : XRBaseGrabTransformer, IXRDropTransformer
    {
        bool m_IsLeftGrabbed;
        bool m_IsRightGrabbed;

        [SerializeField]
        [Tooltip("Scale factor for one-handed displacement adjustments.")]
        float m_OneHandedDisplacementScale = 0.5f;

        BindableEnum<CalibrationState> m_CalibrationState = new(CalibrationState.Inactive);
        public IReadOnlyBindableVariable<CalibrationState> calibrationState => m_CalibrationState;

        protected override RegistrationMode registrationMode => RegistrationMode.SingleAndMultiple;

        public bool canProcessOnDrop => true;

        AttachPointVelocityTracker m_LeftVelocityTracker = new AttachPointVelocityTracker();
        ImprovedOneEuroFilterVector3 m_LeftFilter = new ImprovedOneEuroFilterVector3(Vector3.zero);
        Pose m_LeftPose = Pose.identity;

        AttachPointVelocityTracker m_RightVelocityTracker = new AttachPointVelocityTracker();
        ImprovedOneEuroFilterVector3 m_RightFilter = new ImprovedOneEuroFilterVector3(Vector3.zero);
        Pose m_RightPose = Pose.identity;

        // Variable to store the dominant axis for one-handed movement
        enum MovementAxis
        {
            None,
            Horizontal,
            Vertical
        }

        MovementAxis m_CurrentMovementAxis = MovementAxis.None;

        float m_PanningLowerThreshold = 0.1f;
        float m_PanningThreshold = 0.5f; // Adjust this value as needed
        float m_MaxNoPanningDuration = 0.5f; // 0.5 seconds
        float m_NoPanningTime = 0f;
        float m_LastStateChangeTime = 0f;
        float m_StateTransitionFactor = 1f;

        Pose m_PreviousHandlePose;

        readonly BindingsGroup m_BindingsGroup = new BindingsGroup();
        XRGrabInteractable m_GrabInteractable;

        // Helper instance
        OneHandedTransformerHelper m_OneHandedHelper = new OneHandedTransformerHelper();

        void OnEnable()
        {
            m_BindingsGroup.AddBinding(m_CalibrationState.Subscribe(OnCalibrationStateChanged));
        }

        void OnDisable()
        {
            m_BindingsGroup.Clear();
        }

        void OnCalibrationStateChanged(CalibrationState state)
        {
            m_LastStateChangeTime = Time.unscaledTime;

            if (state == CalibrationState.Inactive)
                return;

            if (state == CalibrationState.TwoHandedFree)
                return;

            // Shared setup for OneHandedLocked and OneHandedUnlocked
            m_CurrentMovementAxis = MovementAxis.None;

            if (m_GrabInteractable.interactorsSelecting.Count > 0)
            {
                UpdateHandlePositions(m_GrabInteractable, ref m_LeftPose, ref m_RightPose, true);
                m_PreviousHandlePose = m_IsLeftGrabbed ? m_LeftPose : m_RightPose;

                // Initialize the helper with the current interaction
                m_OneHandedHelper.Setup(m_GrabInteractable, m_GrabInteractable.interactorsSelecting[0]);
            }
        }

        public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
            if (updatePhase is XRInteractionUpdateOrder.UpdatePhase.Dynamic or XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
            {
                UpdateTarget(grabInteractable, ref targetPose, ref localScale);
            }
        }

        void UpdateTarget(XRGrabInteractable grabInteractable, ref Pose targetPose, ref Vector3 localScale)
        {
            if (m_CalibrationState.Value == CalibrationState.Inactive)
                return;

            // Update handle positions
            UpdateHandlePositions(grabInteractable, ref m_LeftPose, ref m_RightPose);

            if (!m_IsLeftGrabbed && !m_IsRightGrabbed)
            {
                m_CalibrationState.Value = CalibrationState.Inactive;
                return;
            }

            Pose handlePose = GetCurrentHandlePose();
            var interactor = grabInteractable.interactorsSelecting[0];

            m_StateTransitionFactor = Mathf.Clamp01((Time.unscaledTime - m_LastStateChangeTime) / 0.2f);

            switch (m_CalibrationState.Value)
            {
                case CalibrationState.TwoHandedFree:
                    targetPose = ComputeTwoHandedFreeTargetPose(interactor, m_LeftPose, m_RightPose);
                    break;
                case CalibrationState.OneHandedLocked:
                    // Handle state switching logic after pose computation
                    HandleOneHandedLockedStateSwitching(handlePose);

                    if (m_StateTransitionFactor < 1)
                    {
                        var unlockedPose = ComputeOneHandedUnlockedTargetPose(targetPose, handlePose, interactor);
                        var lockedPose = ComputeOneHandedLockedTargetPose(targetPose, handlePose, interactor);

                        // Handle blend
                        targetPose = PoseExtensions.Lerp(unlockedPose, lockedPose, m_StateTransitionFactor);
                    }
                    else
                    {
                        targetPose = ComputeOneHandedLockedTargetPose(targetPose, handlePose, interactor);
                    }
                    break;

                case CalibrationState.OneHandedUnlocked:
                    // Handle state switching logic after pose computation
                    HandleOneHandedUnlockedStateSwitching(handlePose);

                    if (m_StateTransitionFactor < 1)
                    {
                        var unlockedTargetPose = ComputeOneHandedUnlockedTargetPose(targetPose, handlePose, interactor);
                        var lockedTargetPose = ComputeOneHandedLockedTargetPose(targetPose, handlePose, interactor);
                        targetPose = PoseExtensions.Lerp(lockedTargetPose, unlockedTargetPose, m_StateTransitionFactor);
                    }
                    else
                    {
                        targetPose = ComputeOneHandedUnlockedTargetPose(targetPose, handlePose, interactor);
                    }
                    break;
            }

            m_PreviousHandlePose = handlePose;
        }

        #region Target Pose Computation Methods

        Pose ComputeTwoHandedFreeTargetPose(IXRSelectInteractor interactor0, Pose leftHandlePose, Pose rightHandlePose)
        {
            bool isInteractorLeft = interactor0.handedness == InteractorHandedness.Left;

            Vector3 handleCenter = Vector3.Lerp(leftHandlePose.position, rightHandlePose.position, 0.5f);
            Vector3 rightVector = Vector3.ProjectOnPlane((rightHandlePose.position - leftHandlePose.position).normalized, Vector3.up);
            Vector3 forwardVector = -Vector3.Cross(-rightVector, Vector3.up);

            // Determine initial grab point offset
            Pose zeroPose = isInteractorLeft ? m_LeftPose : m_RightPose;
            m_OneHandedHelper.UpdateTarget(m_GrabInteractable, interactor0, ref zeroPose);
            var centerOffset = Vector3.Project(zeroPose.position - handleCenter, rightVector);

            Quaternion targetRotation = Quaternion.LookRotation(forwardVector, Vector3.up);

            return new Pose(handleCenter + centerOffset, targetRotation);
        }

        Pose ComputeOneHandedLockedTargetPose(Pose currentTargetPose, Pose handlePose, IXRSelectInteractor interactor)
        {
            // Determine the dominant axis of motion
            DetermineDominantAxis();

            // Process one-handed adjustment based on dominant axis
            ProcessOneHandedAdjustment(ref currentTargetPose, handlePose);

            return currentTargetPose;
        }

        Pose ComputeOneHandedUnlockedTargetPose(Pose currentTargetPose, Pose handlePose, IXRSelectInteractor interactor)
        {
            // Use the helper to compute the new target pose
            Pose newTargetPose = currentTargetPose;

            m_OneHandedHelper.UpdateTarget(m_GrabInteractable, interactor, ref newTargetPose);

            // Constrain rotation
            ConstrainRotation(ref newTargetPose);

            return newTargetPose;
        }

        #endregion

        #region Helper Methods

        void UpdateHandlePositions(XRGrabInteractable grabInteractable, ref Pose leftHandlePose, ref Pose rightHandlePose, bool reset = false)
        {
            m_IsLeftGrabbed = m_IsRightGrabbed = false;

            foreach (var interactor in grabInteractable.interactorsSelecting)
            {
                var attachTransform = interactor.GetAttachTransform(grabInteractable);
                var handlePose = attachTransform.GetWorldPose();

                if (interactor.handedness == InteractorHandedness.Left)
                {
                    leftHandlePose = handlePose;
                    m_IsLeftGrabbed = true;

                    m_LeftVelocityTracker.UpdateAttachPointVelocityData(attachTransform);
                    if (reset)
                        m_LeftFilter.Initialize(handlePose.position);

                    var filteredPos = m_LeftFilter.Filter(handlePose.position, m_LeftVelocityTracker.GetAttachPointVelocity());
                    m_LeftPose = new Pose(filteredPos, handlePose.rotation);
                }
                else if (interactor.handedness == InteractorHandedness.Right)
                {
                    rightHandlePose = handlePose;
                    m_IsRightGrabbed = true;

                    m_RightVelocityTracker.UpdateAttachPointVelocityData(attachTransform);
                    if (reset)
                        m_RightFilter.Initialize(handlePose.position);

                    var filteredPos = m_RightFilter.Filter(handlePose.position, m_RightVelocityTracker.GetAttachPointVelocity());
                    m_RightPose = new Pose(filteredPos, handlePose.rotation);
                }
            }
        }

        Vector3 GetVelocityOfSelectingHand()
        {
            if (m_IsRightGrabbed)
                return m_RightVelocityTracker.GetAttachPointVelocity();
            if (m_IsLeftGrabbed)
                return m_LeftVelocityTracker.GetAttachPointVelocity();
            return Vector3.zero;
        }

        Vector3 GetAngularVelocityOfSelectingHand()
        {
            if (m_IsRightGrabbed)
                return m_RightVelocityTracker.GetAttachPointAngularVelocity();
            if (m_IsLeftGrabbed)
                return m_LeftVelocityTracker.GetAttachPointAngularVelocity();
            return Vector3.zero;
        }

        void DetermineDominantAxis()
        {
            Vector3 velocity = GetVelocityOfSelectingHand();

            // Use absolute values to compare magnitudes
            float absHorizontalVelocity = Mathf.Abs(velocity.x) + Mathf.Abs(velocity.z);
            float absVerticalVelocity = Mathf.Abs(velocity.y);

            // Compare velocities to determine dominant axis
            if (absHorizontalVelocity > absVerticalVelocity)
            {
                m_CurrentMovementAxis = MovementAxis.Horizontal;
            }
            else if (absVerticalVelocity > absHorizontalVelocity)
            {
                m_CurrentMovementAxis = MovementAxis.Vertical;
            }
            else
            {
                m_CurrentMovementAxis = MovementAxis.None;
            }
        }

        void ProcessOneHandedAdjustment(ref Pose targetPose, Pose handlePose)
        {
            Vector3 displacement = handlePose.position - m_PreviousHandlePose.position;
            // Scale displacement for smoother movement
            displacement *= m_OneHandedDisplacementScale;

            if (m_CurrentMovementAxis == MovementAxis.Vertical)
            {
                // Adjust vertically
                targetPose.position += Vector3.up * displacement.y;
            }
            else if (m_CurrentMovementAxis == MovementAxis.Horizontal)
            {
                // Adjust horizontally in the XZ plane
                Vector3 horizontalDisplacement = new Vector3(displacement.x, 0f, displacement.z);
                targetPose.position += horizontalDisplacement;
            }
        }

        void ConstrainRotation(ref Pose targetPose)
        {
            Vector3 projectedForward = Vector3.ProjectOnPlane(targetPose.forward, Vector3.up);
            targetPose.rotation = Quaternion.LookRotation(projectedForward.normalized, Vector3.up);
        }

        Pose GetCurrentHandlePose()
        {
            if (m_GrabInteractable.interactorsSelecting.Count > 0)
            {
                var interactor = m_GrabInteractable.interactorsSelecting[0];
                var handleTransform = interactor.GetAttachTransform(m_GrabInteractable);
                return handleTransform.GetWorldPose();
            }

            return Pose.identity;
        }

        bool IsPanning()
        {
            Vector3 angularVelocity = GetAngularVelocityOfSelectingHand();
            float yAngularVelocity = angularVelocity.y;
            return Mathf.Abs(yAngularVelocity) > Mathf.Lerp(m_PanningLowerThreshold, m_PanningThreshold, m_StateTransitionFactor);
        }

        void HandleOneHandedLockedStateSwitching(Pose handlePose)
        {
            if (IsPanning())
            {
                m_CalibrationState.Value = CalibrationState.OneHandedUnlocked;
                m_NoPanningTime = 0f;
            }
        }

        void HandleOneHandedUnlockedStateSwitching(Pose handlePose)
        {
            if (!IsPanning())
            {
                m_NoPanningTime += Time.deltaTime;
                if (m_NoPanningTime >= m_MaxNoPanningDuration)
                {
                    m_CalibrationState.Value = CalibrationState.OneHandedLocked;
                    m_NoPanningTime = 0f;
                }
            }
            else
            {
                m_NoPanningTime = 0f; // Reset timer if panning is detected
            }
        }

        #endregion

        #region Event Handlers

        public override void OnUnlink(XRGrabInteractable grabInteractable)
        {
            base.OnUnlink(grabInteractable);
            ResetCalibrationState();
        }

        public override void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
        {
            base.OnGrabCountChanged(grabInteractable, targetPose, localScale);
            var newCount = grabInteractable.interactorsSelecting.Count;

            if (newCount == 2)
            {
                m_CalibrationState.Value = CalibrationState.TwoHandedFree;
            }
            else if (newCount == 1)
            {
                m_CalibrationState.Value = CalibrationState.OneHandedLocked;
            }
        }

        public override void OnLink(XRGrabInteractable grabInteractable)
        {
            base.OnLink(grabInteractable);
            m_GrabInteractable = grabInteractable;
        }

        public void OnDrop(XRGrabInteractable grabInteractable, DropEventArgs args)
        {
            ResetCalibrationState();
        }

        void ResetCalibrationState()
        {
            m_CalibrationState.Value = CalibrationState.Inactive;
        }

        #endregion
    }

    public static class PoseExtensions
    {
        /// <summary>
        /// Blends the position and rotation of two poses based on a given factor.
        /// </summary>
        /// <param name="poseA">The first pose.</param>
        /// <param name="poseB">The second pose.</param>
        /// <param name="blendFactor">The blend factor, where 0 returns poseA and 1 returns poseB.</param>
        /// <returns>A new blended pose.</returns>
        public static Pose Lerp(Pose poseA, Pose poseB, float blendFactor)
        {
            Vector3 blendedPosition = Vector3.Lerp(poseA.position, poseB.position, blendFactor);
            Quaternion blendedRotation = Quaternion.Slerp(poseA.rotation, poseB.rotation, blendFactor);
            return new Pose(blendedPosition, blendedRotation);
        }
    }
}
