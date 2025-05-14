using UnityEngine;
using MixedReality.Toolkit.UX;
using Microsoft.MixedReality.GraphicsTools;
using MixedReality.Toolkit.Input;
using System.Collections;

public class EyeGazeButtonHighlight : MonoBehaviour
{
    private FrontPlatePulse frontPlatePulse; // Reference to the FrontPlatePulse component
    private PressableButton button;

    private bool isGazeActive = false; // Tracks whether the gaze is active
    private Coroutine pulseCoroutine; // Reference to the pulse coroutine

    void Start()
    {
        // Ensure the button component is assigned
        button = transform.parent.parent.GetComponent<PressableButton>();
        if (button == null)
        {
            Debug.LogError("PressableButton component not found on this GameObject.");
            return;
        }

        frontPlatePulse = transform.Find("UX.Button.FrontplateHighlight").GetComponent<FrontPlatePulse>();
        // Ensure the FrontPlatePulse component is assigned
        if (frontPlatePulse == null)
        {
            Debug.LogError("FrontPlatePulse component not found on this GameObject.");
            return;
        }

        // Add listeners for gaze hover events
        button.IsGazeHovered.OnEntered.AddListener((data) => OnButtonGazeEnter());
        button.IsGazeHovered.OnExited.AddListener((data) => OnButtonGazeExit());
    }

    private void OnButtonGazeEnter()
    {
        isGazeActive = true;
        // Start the pulse coroutine if not already running
        if (pulseCoroutine == null)
        {
            pulseCoroutine = StartCoroutine(PulseWhileGazed());
        }
    }

    private void OnButtonGazeExit()
    {
        isGazeActive = false;
        // Stop the pulse coroutine
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
    }

    private IEnumerator PulseWhileGazed()
    {
        float pulseInterval = 0.1f;
        while (isGazeActive)
        {
            // Trigger the pulse
            frontPlatePulse.PulseAt(transform.position, 0); // Use index 0 for left pulse
            float elapsedTime = 0f;

            while (elapsedTime < pulseInterval && isGazeActive)
            {
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for next frame to continue
            }
        }

    }
}