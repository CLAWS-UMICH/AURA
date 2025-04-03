using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace XRMultiplayer
{
    public class TableRotationTransformer : XRBaseGrabTransformer
    {
        protected override RegistrationMode registrationMode => RegistrationMode.Single;

        // Store the initial angle between the controller and the table when the grab starts
        private float initialAngle;

        // Store the initial rotation of the table when the grab starts
        private Quaternion initialRotation;

        public override void OnGrab(XRGrabInteractable grabInteractable)
        {
            base.OnGrab(grabInteractable);

            // Get the transform of the controller's attach point
            var handle = grabInteractable.interactorsSelecting[0].GetAttachTransform(grabInteractable);

            // Get the transform of the table (interactable)
            var interactableOrigin = grabInteractable.transform;

            // Compute the vector from the table's origin to the controller's position
            Vector3 fromOriginToHandle = handle.position - interactableOrigin.position;

            // Project the vector onto the horizontal plane (XZ plane) to ignore vertical differences
            fromOriginToHandle.y = 0;

            // Calculate the initial angle in degrees using Atan2
            initialAngle = Mathf.Atan2(fromOriginToHandle.z, fromOriginToHandle.x) * Mathf.Rad2Deg;

            // Store the initial rotation of the table
            initialRotation = interactableOrigin.rotation;
        }

        public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
            if (updatePhase is XRInteractionUpdateOrder.UpdatePhase.Dynamic
                or XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
            {
                UpdateTarget(grabInteractable, ref targetPose, ref localScale);
            }
        }

        void UpdateTarget(XRGrabInteractable grabInteractable,
            ref Pose targetPose, ref Vector3 localScale)
        {
            // Get the transform of the controller's attach point
            var handle = grabInteractable.interactorsSelecting[0].GetAttachTransform(grabInteractable);

            // Get the transform of the table (interactable)
            var interactableOrigin = grabInteractable.transform;

            // Compute the vector from the table's origin to the controller's current position
            Vector3 fromOriginToHandle = handle.position - interactableOrigin.position;

            // Project the vector onto the horizontal plane (XZ plane)
            fromOriginToHandle.y = 0;

            // Calculate the current angle in degrees
            float currentAngle = Mathf.Atan2(fromOriginToHandle.z, fromOriginToHandle.x) * Mathf.Rad2Deg;

            // Calculate the difference between the current angle and the initial angle
            float angleDelta = -Mathf.DeltaAngle(initialAngle, currentAngle);

            // Create a rotation delta around the Y-axis based on the angle difference
            Quaternion rotationDelta = Quaternion.Euler(0, angleDelta, 0);

            // Apply the rotation delta to the initial rotation of the table
            targetPose.rotation = rotationDelta * initialRotation;
        }
    }
}
