using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace XRMultiplayer
{
    /// <summary>
    /// Handles preventing unintended interactions by disabling interactables when an interactor overlaps a specified bounding region.
    /// </summary>
    [DisallowMultipleComponent]
    public class AccidentalPressHandler : MonoBehaviour
    {
        private List<XRBaseInteractor> m_OriginInteractors = new List<XRBaseInteractor>();

        [SerializeField]
        private List<XRBaseInteractable> m_InteractablesToManage = new List<XRBaseInteractable>();

        [SerializeField]
        private Vector3 m_TableBoundsSize = new Vector3(0.8f, 1.0f, 0.8f);

        private Bounds m_TableBoundingBox;

        void Start()
        {
            var origin = FindAnyObjectByType<XROrigin>();
            if (origin != null)
                m_OriginInteractors.AddRange(origin.GetComponentsInChildren<XRBaseInteractor>(true));

            // Update the bounding box every frame in case the table or this object moves.
            m_TableBoundingBox = new Bounds(transform.position + Vector3.up * m_TableBoundsSize.y / 2f, m_TableBoundsSize);
        }

        void Update()
        {
            bool anyOverlap = false;
            foreach (var interactor in m_OriginInteractors)
            {
                if (interactor == null || !interactor.isActiveAndEnabled)
                    continue;

                var attachTransform = interactor.GetAttachTransform(null);
                if (attachTransform == null)
                    continue;

                var interactorPosition = attachTransform.position;
                if (m_TableBoundingBox.Contains(interactorPosition))
                {
                    anyOverlap = true;
                    break;
                }
            }

            foreach (var interactable in m_InteractablesToManage)
            {
                if (interactable.isSelected)
                    continue;
                interactable.enabled = !anyOverlap;
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + Vector3.up * m_TableBoundsSize.y / 2f, m_TableBoundsSize);
        }
    }

}
