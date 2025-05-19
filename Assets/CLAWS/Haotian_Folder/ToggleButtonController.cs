using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 


public class ToggleButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("References")]
    public GameObject toggleDot; // Assign in Inspector
    public Vector2 offPosition = new Vector2(-30, 0); // Local position for "off"
    public Vector2 onPosition = new Vector2(30, 0);   // Local position for "on"

    [Header("State")]
    public bool isOn = false;

    // Optional: Add a callback for when toggled
    public System.Action<bool> OnToggle;

    void Start()
    {
        UpdateDotPosition();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isOn = !isOn;
        UpdateDotPosition();
        OnToggle?.Invoke(isOn);
    }

    private void UpdateDotPosition()
    {
        RectTransform rt = toggleDot.GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = isOn ? onPosition : offPosition;
    }
}
