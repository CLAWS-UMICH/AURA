// from molly:
// used to set mouse interactor position and rotation to match eye gaze
// used for clicking with a bluetooth mouse

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;
using static MixedReality.Toolkit.Input.XRRayInteractorExtensions;
using Unity.XR.CoreUtils;
using MixedReality.Toolkit.UX;

namespace MixedReality.Toolkit.Input.Experimental
{
    public class AURASpatialMouse : BaseReticleVisual
    {
       // assign from XR rig
        public SpatialMouseInteractor mouseInteractor;
        public float defaultDistance = 1f;
        public FuzzyGazeInteractor eyeGazeInteractor;
        public bool showReticle = false; // for use in game view/debugging -- should be off in build

        protected override void OnEnable()
        {
            base.OnEnable();
            mouseInteractor.selectEntered.AddListener(LocateTargetHitPoint);
            Application.onBeforeRender += OnBeforeRenderCursor;
        }

        protected virtual void OnDisable()
        {
            mouseInteractor.selectEntered.RemoveListener(LocateTargetHitPoint);
            Application.onBeforeRender -= OnBeforeRenderCursor;
        }

        private TargetHitDetails selectedHitDetails = new TargetHitDetails();
        Vector3[] rayPositions;
        int rayPositionsCount = -1;

        private Vector3 reticlePosition;
        private Vector3 reticleNormal;
        private int endPositionInLine;

        private void OnBeforeRenderCursor()
        {
            if (eyeGazeInteractor == null) { return; }
            if (!eyeGazeInteractor.TryGetHitInfo(out Vector3 gazeHitPosition, out Vector3 gazeHitNormal, out int _, out bool isValidTarget))
            {
                // no valid target, use default distance
                gazeHitPosition = eyeGazeInteractor.transform.position + eyeGazeInteractor.transform.forward * defaultDistance;
                gazeHitNormal = -eyeGazeInteractor.transform.forward;
            }
            mouseInteractor.rayOriginTransform.position = gazeHitPosition;
            mouseInteractor.rayOriginTransform.forward = gazeHitNormal;

            if (Reticle != null)
            {
                Reticle.transform.position = gazeHitPosition;
                Reticle.transform.forward = gazeHitNormal;
                if (showReticle)
                {
                    if (!Reticle.activeSelf)
                    {
                        Reticle.SetActive(true);
                    }
                }
                else
                {
                    if (Reticle.activeSelf)
                    {
                        Reticle.SetActive(false);
                    }
                }
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (isValidTarget && eyeGazeInteractor.PreciseHitResult.targetInteractable is StatefulInteractable interactable)
                {
                    if (interactable.ToggleMode == StatefulInteractable.ToggleType.Toggle)
                    {
                        interactable.ForceSetToggled(!interactable.IsToggled.Active, fireEvents: true);
                    }
                    else
                    {
                        //Debug.Log($"Mouse clicked on interactable: {interactable}");
                       // interactable.IsActiveHovered = new TimedFlag { Active = true };
                        mouseInteractor.interactionManager.SelectEnter(mouseInteractor, (IXRSelectInteractable)interactable);
                    }
                }
            }
        }

        private void LocateTargetHitPoint(SelectEnterEventArgs args)
        {
            if (rayPositions == null ||
                rayPositions.Length == 0 ||
                rayPositionsCount == 0 ||
                rayPositionsCount > rayPositions.Length)
            {
                return;
            }

            mouseInteractor.TryLocateTargetHitPoint(args.interactableObject, out selectedHitDetails);
        }
    }
}