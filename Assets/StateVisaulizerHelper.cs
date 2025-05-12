using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public static class StateVisualizerHelper
{
    public static void TriggerSelect(XRBaseInteractable interactable, IXRSelectInteractor interactor)
    {
        // Use reflection to access the private OnSelectEntered method
        MethodInfo onSelectEnteredMethod = typeof(XRBaseInteractable).GetMethod("OnSelectEntered", BindingFlags.NonPublic | BindingFlags.Instance);
        if (onSelectEnteredMethod != null)
        {
            // Create a SelectEnterEventArgs to pass to the method
            var args = new SelectEnterEventArgs
            {
                interactableObject = interactable,
                interactorObject = interactor
            };

            // Invoke the OnSelectEntered method
            onSelectEnteredMethod.Invoke(interactable, new object[] { args });
            Debug.Log($"Triggered Select state for {interactable.gameObject.name}");
        }
        else
        {
            Debug.LogError("Unable to find the OnSelectEntered method.");
        }
    }
}