using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit;
using UnityEngine.InputSystem;

public class MouseButtonManager : MonoBehaviour
{
    private PressableButton pressableButton;

    void Start()
    {
        pressableButton = GetComponent<PressableButton>();
        if (pressableButton == null)
        {
            Debug.LogError("PressableButton component not found on this GameObject.");
            return;
        }
    }

     void Update()
    {
        if (pressableButton.IsGazeHovered && Mouse.current.leftButton.wasPressedThisFrame)
        {

            if (pressableButton.ToggleMode == StatefulInteractable.ToggleType.Toggle)
            {
                // Toggle logic
                bool newState = !pressableButton.IsToggled.Active;
                pressableButton.ForceSetToggled(newState);
                Debug.Log($"{gameObject.name} toggled to: {newState}");
            }
            else
            {
                // Click logic
                pressableButton.OnClicked.Invoke();
                Debug.Log($"{gameObject.name} clicked.");
            }
        }
    }
}
