using UnityEngine;
using MixedReality.Toolkit.UX;
using Microsoft.MixedReality.GraphicsTools;
using MixedReality.Toolkit.Input;
using System.Collections;

public class EyeGazeCanvasHighlight: MonoBehaviour
{
    [SerializeField] private FrontPlatePulse frontPlatePulse;
    [SerializeField] private PressableButton button;

    private bool isGazeActive = false;
    private Coroutine pulseCoroutine;
    void Start()
    {
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
            frontPlatePulse.PulseAt(transform.position, 0);
            float elapsedTime = 0f;

            while (elapsedTime < pulseInterval && isGazeActive)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

    }
}