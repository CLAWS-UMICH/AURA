using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace XRMultiplayer
{
    public class TableHandleController : MonoBehaviour
    {
        [Header("Interactables")]
        [SerializeField]
        XRGrabInteractable m_RotationInteractable;

        [SerializeField]
        XRGrabInteractable m_MoveInteractable;

        [Header("Capsule")]
        [SerializeField]
        ProceduralCapsule m_ProceduralCapsule0;

        [SerializeField]
        float m_CapsuleRestingHeight = 10f;

        [SerializeField]
        float m_CapsuleExtendedHeight = 70f;

        [Header("Anchor Points")]
        [SerializeField]
        float m_LeftAnchor = -0.425f;

        [SerializeField]
        float m_RightAnchor = 0.425f;

        [Header("Handle Centers")]
        [SerializeField]
        Transform m_MoveHandleCenter = null;

        [SerializeField]
        Transform m_RotationHandleCenter = null;

        float totalLength => m_RightAnchor - m_LeftAnchor;

        // Flags to indicate hover and selection status
        bool m_RotationIsHovered = false;
        bool m_MoveIsHovered = false;

        bool m_RotationIsSelected = false;
        bool m_MoveIsSelected = false;

        float m_LastInteractionTime = 0f;

        bool m_IsRotationLastAction = false;

        // Target variables for capsule properties
        Vector3 m_TargetLocalPosition;
        float m_TargetCapsuleHeight;

        float m_TargetLerpSpeed = 6f;
        float m_TargetBendLerpSpeed = 16f;

        // Colors for each state
        Color m_IdleColor = Color.white;
        Color m_HoverColor = new Color(0.8f, 0.8f, 1f, 1f);
        Color m_SelectedColor = new Color(0f, 0.6f, 1f, 1f);
        Color m_DisabledColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        Color m_FresnelHighlightColor = new Color(0.8f, 0.8f, 1f, 1f);

        // Color blending fields
        Color m_CurrentColor;
        Color m_FromColor;
        Color m_ToColor;
        float m_ColorBlendFactor = 1f;
        float m_ColorTransitionSpeed = 5f;

        MaterialPropertyBlock m_Mpb;
        readonly int m_BaseColorPropertyID = Shader.PropertyToID("_BaseColor");
        readonly int m_FresnelHighlightColorPropertyID = Shader.PropertyToID("_FresnelHighlightColor");

        void OnEnable()
        {
            // Start with the capsule visible at rest position
            m_ProceduralCapsule0.isVisible = true;

            // Initialize target variables
            m_TargetLocalPosition = m_ProceduralCapsule0.transform.localPosition;
            m_TargetCapsuleHeight = m_CapsuleRestingHeight;


            // Initialize the material property block and set initial color
            m_Mpb = new MaterialPropertyBlock();
            m_CurrentColor = m_IdleColor;
            m_FromColor = m_IdleColor;
            m_ToColor = m_IdleColor;
            m_ColorBlendFactor = 1f;
            UpdateShaderColor(m_CurrentColor, m_FresnelHighlightColor);

            // Add event listeners for both interactables
            m_RotationInteractable.firstSelectEntered.AddListener(OnSelectEnter);
            m_RotationInteractable.lastSelectExited.AddListener(OnSelectExit);
            m_RotationInteractable.firstHoverEntered.AddListener(OnHoverEnter);
            m_RotationInteractable.lastHoverExited.AddListener(OnHoverExit);

            m_MoveInteractable.firstSelectEntered.AddListener(OnSelectEnter);
            m_MoveInteractable.lastSelectExited.AddListener(OnSelectExit);
            m_MoveInteractable.firstHoverEntered.AddListener(OnHoverEnter);
            m_MoveInteractable.lastHoverExited.AddListener(OnHoverExit);
        }

        void OnDisable()
        {
            // Reset the capsule to its initial state
            m_ProceduralCapsule0.isVisible = true;

            // Remove listeners
            m_RotationInteractable.firstSelectEntered.RemoveListener(OnSelectEnter);
            m_RotationInteractable.lastSelectExited.RemoveListener(OnSelectExit);
            m_RotationInteractable.firstHoverEntered.RemoveListener(OnHoverEnter);
            m_RotationInteractable.lastHoverExited.RemoveListener(OnHoverExit);

            m_MoveInteractable.firstSelectEntered.RemoveListener(OnSelectEnter);
            m_MoveInteractable.lastSelectExited.RemoveListener(OnSelectExit);
            m_MoveInteractable.firstHoverEntered.RemoveListener(OnHoverEnter);
            m_MoveInteractable.lastHoverExited.RemoveListener(OnHoverExit);
        }

        void OnHoverEnter(HoverEnterEventArgs args)
        {
            // Determine which interactable is hovered
            if (ReferenceEquals(args.interactableObject, m_RotationInteractable))
                m_RotationIsHovered = true;
            else if (ReferenceEquals(args.interactableObject, m_MoveInteractable))
                m_MoveIsHovered = true;
        }

        void OnHoverExit(HoverExitEventArgs args)
        {
            // Determine which interactable hover has exited
            if (ReferenceEquals(args.interactableObject, m_RotationInteractable))
                m_RotationIsHovered = false;
            else if (ReferenceEquals(args.interactableObject, m_MoveInteractable))
                m_MoveIsHovered = false;
        }

        void OnSelectEnter(SelectEnterEventArgs args)
        {
            // Determine which interactable is selected
            if (ReferenceEquals(args.interactableObject, m_RotationInteractable))
            {
                m_RotationIsSelected = true;
                // Set this behavior's transform to match the selected interactable
                CopyTransform(m_RotationHandleCenter, transform);
            }
            else if (ReferenceEquals(args.interactableObject, m_MoveInteractable))
            {
                m_MoveIsSelected = true;
                // Set this behavior's transform to match the selected interactable
                CopyTransform(m_MoveHandleCenter, transform);
            }
        }

        void OnSelectExit(SelectExitEventArgs args)
        {
            // Determine which interactable is deselected
            if (ReferenceEquals(args.interactableObject, m_RotationInteractable))
                m_RotationIsSelected = false;
            else if (ReferenceEquals(args.interactableObject, m_MoveInteractable))
                m_MoveIsSelected = false;
        }

        void UpdateLastInteractionTime()
        {
            m_LastInteractionTime = Time.time;
        }


        float m_LastHoverTime = 0f;

        void UpdateLastHoverTime()
        {
            m_LastHoverTime = Time.time;
        }

        void LateUpdate()
        {
            float currentTime = Time.time;

            // Determine the priority interactable
            XRGrabInteractable priorityInteractable = null;
            if (m_RotationIsSelected || m_RotationIsHovered)
            {
                // Rotation has priority
                priorityInteractable = m_RotationInteractable;
                m_IsRotationLastAction = true;
                UpdateLastHoverTime();
            }
            else if (m_MoveIsSelected || m_MoveIsHovered)
            {
                priorityInteractable = m_MoveInteractable;
                m_IsRotationLastAction = false;
                UpdateLastHoverTime();
            }

            if (m_RotationIsSelected || m_MoveIsSelected)
                UpdateLastInteractionTime();

            // If either interactable is selected, set transform to match
            if (m_RotationIsSelected || m_IsRotationLastAction)
                CopyTransform(m_RotationHandleCenter, transform);
            else
                CopyTransform(m_MoveHandleCenter, transform);

            // Collect all interactors interacting with the priority interactable
            List<IXRInteractor> interactors = new List<IXRInteractor>();
            CollectInteractors(priorityInteractable, interactors);

            int currentInteractorCount = interactors.Count;

            if (currentInteractorCount == 0)
            {
                float timeSinceLastHover = currentTime - m_LastHoverTime;

                // No hands interacting
                if (timeSinceLastHover >= 0.5f)
                {
                    // Smooth the capsule back to rest position
                    UpdateAtRestCapsule();
                }
            }
            else if (currentInteractorCount == 1 || m_RotationIsSelected)
            {
                float timeSinceLastInteraction = currentTime - m_LastInteractionTime;
                bool overResetThreshold = (timeSinceLastInteraction >= 0.5f);

                // Update the capsule based on the interactor
                UpdateInteractorForCapsule(interactors[0], overResetThreshold);
            }
            else if (currentInteractorCount >= 2)
            {
                // Smooth the capsule to origin (x=0) and height to extended height
                SmoothCapsuleToOriginAndHeight(m_CapsuleExtendedHeight);
            }

            // Apply the updates to the capsule
            ApplyCapsuleUpdates();

            // Determine state color
            bool bothDisabled = !m_RotationInteractable.enabled && !m_MoveInteractable.enabled;
            if (bothDisabled)
            {
                SetTargetColor(m_DisabledColor);
            }
            else
            {
                if (m_RotationIsSelected || m_MoveIsSelected)
                    SetTargetColor(m_SelectedColor);
                else if (m_RotationIsHovered || m_MoveIsHovered)
                    SetTargetColor(m_HoverColor);
                else
                    SetTargetColor(m_IdleColor);
            }

            UpdateColorBlend();
        }

        void CollectInteractors(XRBaseInteractable interactable, List<IXRInteractor> interactorsList)
        {
            if (interactable == null)
                return;

            // Prioritize select first
            foreach (var interactor in interactable.interactorsSelecting)
            {
                if (!interactorsList.Contains(interactor))
                    interactorsList.Add(interactor);
            }

            // Then hover
            foreach (var interactor in interactable.interactorsHovering)
            {
                if (!interactorsList.Contains(interactor))
                    interactorsList.Add(interactor);
            }
        }

        void CopyTransform(Transform fromTransform, Transform toTransform)
        {
            toTransform.SetPositionAndRotation(fromTransform.position, fromTransform.rotation);
        }

        void SmoothCapsuleToOriginAndHeight(float targetHeight)
        {
            // Set target local position x to 0, keep y and z as is
            m_TargetLocalPosition = m_ProceduralCapsule0.transform.localPosition;
            m_TargetLocalPosition.x = 0f;

            // Set target height
            m_TargetCapsuleHeight = targetHeight;
        }

        void UpdateInteractorForCapsule(IXRInteractor interactor, bool isOverResetThreshold)
        {
            Vector3 targetLocalPosition = m_ProceduralCapsule0.transform.localPosition;
            float targetCapsuleHeight = m_ProceduralCapsule0.height;

            if (m_RotationIsSelected || m_RotationIsHovered)
            {
                // Use the closest anchor position
                targetLocalPosition = transform.InverseTransformPoint(GetClosestAnchorPosition(interactor.GetAttachTransform(null).position));
                targetCapsuleHeight = m_CapsuleRestingHeight;
                m_TargetBendLerpSpeed = 16f;
            }
            else if (m_MoveIsSelected)
            {
                // Use Vector3.zero for move
                targetLocalPosition = Vector3.zero;
                targetCapsuleHeight = m_CapsuleExtendedHeight;
                m_TargetBendLerpSpeed = 0f;
            }
            else if (isOverResetThreshold)
            {
                // Get the interactor's position relative to this behavior's transform
                Vector3 localInteractorPosition = transform.InverseTransformPoint(interactor.GetAttachTransform(null).position);
                float interactorLocalX = Mathf.Clamp(localInteractorPosition.x, m_LeftAnchor, m_RightAnchor);
                targetLocalPosition = new Vector3(interactorLocalX, 0, 0);
                targetCapsuleHeight = m_CapsuleRestingHeight;
                m_TargetBendLerpSpeed = 0f;
            }

            m_TargetLocalPosition = targetLocalPosition;
            m_TargetCapsuleHeight = targetCapsuleHeight;
            m_TargetLerpSpeed = 6f;
        }

        void UpdateAtRestCapsule()
        {
            // Set the target local position to zero
            m_TargetLocalPosition = Vector3.zero;

            // Set the target height to resting height
            m_TargetCapsuleHeight = m_CapsuleRestingHeight;

            // Set the target lerp speed
            m_TargetLerpSpeed = 4f;
            m_TargetBendLerpSpeed = 0f;
        }

        void ApplyCapsuleUpdates()
        {
            // Getting to the bend amount is slow so we want to accelerate it the more bend we are
            float bendAccelFactor = 1f + Mathf.Abs(m_ProceduralCapsule0.bendAmount);

            // Lerp the capsule's local position towards the target position
            var currentPosition = m_ProceduralCapsule0.transform.localPosition;
            m_ProceduralCapsule0.transform.localPosition = Vector3.Lerp(currentPosition, m_TargetLocalPosition, Time.deltaTime * m_TargetLerpSpeed * bendAccelFactor);

            // Lerp the capsule's height towards the target height
            m_ProceduralCapsule0.height = Mathf.Lerp(m_ProceduralCapsule0.height, m_TargetCapsuleHeight, Time.deltaTime * m_TargetLerpSpeed);

            // Compute target bend amount based on target local position x
            var targetBendAmount = CalculateBendAmount(m_ProceduralCapsule0.transform.localPosition.x);

            // Lerp the capsule's bend amount towards the target bend amount
            m_ProceduralCapsule0.bendAmount = m_TargetBendLerpSpeed > 0 ? Mathf.Lerp(m_ProceduralCapsule0.bendAmount, targetBendAmount, Time.deltaTime * m_TargetBendLerpSpeed) : targetBendAmount;
        }

        float CalculateBendAmount(float xPosition)
        {
            // Compute the normalized x position (0 to 1)
            float normalizedX = (xPosition - m_LeftAnchor) / totalLength;

            // Calculate the bend amount based on the position
            if (normalizedX <= 0.1f) // Left 10%
            {
                return Mathf.Lerp(0f, -1f, 1f - (normalizedX / 0.1f));
            }
            else if (normalizedX >= 0.9f) // Right 10%
            {
                return Mathf.Lerp(0f, 1f, (normalizedX - 0.9f) / 0.1f);
            }
            else
            {
                return 0f;
            }
        }

        Vector3 GetClosestAnchorPosition(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            float leftDistance = Mathf.Abs(localPosition.x - m_LeftAnchor);
            float rightDistance = Mathf.Abs(localPosition.x - m_RightAnchor);

            return transform.TransformPoint(new Vector3(leftDistance < rightDistance ? m_LeftAnchor : m_RightAnchor, 0, 0));
        }

        /// <summary>
        /// Sets the target color and begins blending towards it.
        /// </summary>
        void SetTargetColor(Color newColor)
        {
            Color currentValue = Color.Lerp(m_FromColor, m_ToColor, m_ColorBlendFactor);
            m_FromColor = currentValue;
            m_ToColor = newColor;
            m_ColorBlendFactor = 0f;
        }

        /// <summary>
        /// Smoothly updates the current color towards the target color and applies it.
        /// </summary>
        void UpdateColorBlend()
        {
            if (m_ColorBlendFactor < 1f)
            {
                m_ColorBlendFactor = Mathf.MoveTowards(m_ColorBlendFactor, 1f, Time.deltaTime * m_ColorTransitionSpeed);
            }

            m_CurrentColor = Color.Lerp(m_FromColor, m_ToColor, m_ColorBlendFactor);

            // Update the shader with the current base color and fresnel highlight color
            UpdateShaderColor(m_CurrentColor, m_FresnelHighlightColor);
        }

        void UpdateShaderColor(Color baseColor, Color fresnelHighlightColor)
        {
            m_Mpb.SetColor(m_BaseColorPropertyID, baseColor);
            m_Mpb.SetColor(m_FresnelHighlightColorPropertyID, fresnelHighlightColor);
            m_ProceduralCapsule0.meshRenderer.SetPropertyBlock(m_Mpb);
        }
    }
}
