using UnityEngine;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit;
using UnityEngine.InputSystem;

public class CanvasMouseButtonManager : MonoBehaviour
{
    private PressableButton pressableButton;
    private UGUIInputAdapter inputAdapter;

    void Start()
    {
        pressableButton = GetComponent<PressableButton>();
        inputAdapter = GetComponent<UGUIInputAdapter>();
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
                inputAdapter.Click();
                Debug.Log($"{gameObject.name} clicked.");
            }
        }
    }
}
