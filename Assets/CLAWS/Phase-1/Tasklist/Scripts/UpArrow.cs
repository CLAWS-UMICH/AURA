using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UpArrow : MonoBehaviour
{
    public ScrollRect scrollRect; // Reference to the ScrollRect component
    public float scrollIncrement = 0.5f; // Amount to scroll per button click


    void Start()
    {
        // Automatically get the ScrollRect component on the same GameObject
        scrollRect = GetComponent<ScrollRect>();

        // Check if the ScrollRect component was found
        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect component not found on this GameObject!");
        }
        else
        {
            Debug.LogError("ScrollRect found!");
            Debug.LogError(scrollIncrement);
        }
    }
    public void ScrollUp()
    {
        // Increase the vertical scroll position
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + scrollIncrement);
    }

    public void ScrollDown()
    {
        // Decrease the vertical scroll position
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - scrollIncrement);
    }
}

