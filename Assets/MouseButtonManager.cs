using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MixedReality.Toolkit.UX;
using MixedReality.Toolkit;

public class MouseButtonManager : MonoBehaviour
{
    [SerializeField] private PressableButton pressableButton;

    void Start()
    {
        pressableButton = GetComponent<PressableButton>();
    }

     void Update()
    {
        if (pressableButton.IsGazeHovered && Input.GetMouseButtonDown(0))
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
