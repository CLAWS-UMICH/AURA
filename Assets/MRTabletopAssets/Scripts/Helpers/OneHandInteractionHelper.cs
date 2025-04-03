using System;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using Unity.Burst;

namespace XRMultiplayer
{
    [BurstCompile]
    public class OneHandedTransformerHelper
    {
        Pose m_OriginalObjectPose;
        Pose m_OffsetPose;
        Pose m_OriginalInteractorPose;
        Vector3 m_InteractorLocalGrabPoint;
        Vector3 m_ObjectLocalGrabPoint;
        IXRInteractor m_OriginalInteractor;

        public void Setup(XRGrabInteractable grabInteractable, IXRInteractor interactor)
        {
            var grabInteractableTransform = grabInteractable.transform;
            var grabAttachTransform = grabInteractable.GetAttachTransform(interactor);

            m_OriginalObjectPose = grabInteractableTransform.GetWorldPose();
            m_OriginalInteractorPose = interactor.GetAttachTransform(grabInteractable).GetWorldPose();
            m_OriginalInteractor = interactor;

            Vector3 offsetTargetPosition = Vector3.zero;
            Quaternion offsetTargetRotation = Quaternion.identity;

            Quaternion capturedRotation = m_OriginalObjectPose.rotation;
            if (grabInteractable.trackRotation)
            {
                capturedRotation = m_OriginalInteractorPose.rotation;
                offsetTargetRotation = Quaternion.Inverse(Quaternion.Inverse(m_OriginalObjectPose.rotation) * grabAttachTransform.rotation);
            }

            Vector3 capturedPosition = m_OriginalObjectPose.position;
            if (grabInteractable.trackPosition)
            {
                capturedPosition = m_OriginalInteractorPose.position;

                // Calculate offset of the grab interactable's position relative to its attach transform
                var attachOffset = m_OriginalObjectPose.position - grabAttachTransform.position;
                offsetTargetPosition = grabInteractable.trackRotation ? grabAttachTransform.InverseTransformDirection(attachOffset) : attachOffset;
            }

            // Store adjusted transform pose
            m_OriginalObjectPose = new Pose(capturedPosition, capturedRotation);

            Vector3 localScale = grabInteractableTransform.localScale;
            TranslateSetup(m_OriginalInteractorPose, m_OriginalInteractorPose.position, m_OriginalObjectPose, localScale);

            Quaternion worldToGripRotation = offsetTargetRotation * Quaternion.Inverse(m_OriginalInteractorPose.rotation);
            Quaternion relativeCaptureRotation = worldToGripRotation * m_OriginalObjectPose.rotation;

            // Scale offset target position to match new local scale
            Vector3 scaledOffsetTargetPosition = offsetTargetPosition.Divide(localScale);

            m_OffsetPose = new Pose(scaledOffsetTargetPosition, relativeCaptureRotation);
        }

        void TranslateSetup(Pose interactorCentroidPose, Vector3 grabCentroid, Pose objectPose, Vector3 objectScale)
        {
            Quaternion worldToInteractorRotation = Quaternion.Inverse(interactorCentroidPose.rotation);
            m_InteractorLocalGrabPoint = worldToInteractorRotation * (grabCentroid - interactorCentroidPose.position);

            m_ObjectLocalGrabPoint = Quaternion.Inverse(objectPose.rotation) * (grabCentroid - objectPose.position);
            m_ObjectLocalGrabPoint = m_ObjectLocalGrabPoint.Divide(objectScale);
        }

        [BurstCompile]
        static void ComputeNewObjectPosition(
            in float3 interactorPosition,
            in quaternion interactorRotation,
            in quaternion objectRotation,
            in float3 objectScale,
            bool trackRotation,
            in float3 offsetPosition,
            in float3 objectLocalGrabPoint,
            in float3 interactorLocalGrabPoint,
            out Vector3 newPosition)
        {
            // Scale up offset pose with new object scale
            float3 scaledOffsetPose = Scale(offsetPosition, objectScale);

            // Adjust computed offset with current source rotation
            float3 rotationAdjustedOffset = math.mul(interactorRotation, scaledOffsetPose);
            float3 rotationAdjustedTargetOffset = trackRotation ? rotationAdjustedOffset : scaledOffsetPose;
            float3 newTargetPosition = interactorPosition + rotationAdjustedTargetOffset;

            float3 scaledGrabToObject = Scale(objectLocalGrabPoint, objectScale);
            float3 adjustedInteractorToGrab = interactorLocalGrabPoint;

            adjustedInteractorToGrab = math.mul(interactorRotation, adjustedInteractorToGrab);
            var rotatedScaledGrabToObject = math.mul(objectRotation, scaledGrabToObject);

            newPosition = adjustedInteractorToGrab - rotatedScaledGrabToObject + newTargetPosition;
        }

        static float3 Scale(float3 a, float3 b) => new float3(a.x * b.x, a.y * b.y, a.z * b.z);

        Quaternion ComputeNewObjectRotation(in Quaternion interactorRotation, bool trackRotation)
        {
            if (!trackRotation)
                return m_OriginalObjectPose.rotation;
            return interactorRotation * m_OffsetPose.rotation;
        }

        public void UpdateTarget(XRGrabInteractable grabInteractable, IXRInteractor interactor, ref Pose targetPose)
        {
            var attachTransform = interactor.GetAttachTransform(grabInteractable);
            var adjustedInteractorPosition = attachTransform.position;
            var adjustedInteractorRotation = attachTransform.rotation;

            targetPose.rotation = ComputeNewObjectRotation(adjustedInteractorRotation, grabInteractable.trackRotation);

            ComputeNewObjectPosition(
                adjustedInteractorPosition,
                adjustedInteractorRotation,
                targetPose.rotation,
                grabInteractable.transform.localScale,
                grabInteractable.trackRotation,
                m_OffsetPose.position,
                m_ObjectLocalGrabPoint,
                m_InteractorLocalGrabPoint,
                out Vector3 targetObjectPosition);

            targetPose.position = targetObjectPosition;
        }
    }
}
