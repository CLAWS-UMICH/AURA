using UnityEngine;
using MixedReality.Toolkit.UX.Experimental;

public class ScrollableWithEyeGaze : Scrollable
{
    [SerializeField]
    [Tooltip("The eye gaze interactor to track gaze position.")]
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor eyeGazeInteractor;

    [SerializeField]
    [Tooltip("The distance from the viewport edge to trigger scrolling (normalized, 0 to 1).")]
    private float edgeThreshold = 0.1f;

    [SerializeField]
    [Tooltip("The speed at which the scroll rect moves when gaze is near the edge.")]
    private float scrollSpeed = 0.5f;

    private RectTransform viewport;

    protected void Start()
    {

        if (ScrollRect != null)
        {
            viewport = ScrollRect.viewport;
        }
    }

    private void Update()
    {
        if (eyeGazeInteractor == null || viewport == null || ScrollRect == null)
        {
            Debug.LogWarning("Eye Gaze Interactor or Scroll Rect is not assigned.");
            return;
        }

        // Get the gaze position in world space
        if (eyeGazeInteractor.TryGetHitInfo(out Vector3 gazeHitPosition, out _, out _, out bool isValidTarget) && isValidTarget)
        {
            // Convert the gaze position to viewport space
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, Camera.main.WorldToScreenPoint(gazeHitPosition), Camera.main, out localPoint);

            // Normalize the local point to a 0-1 range
            Vector2 normalizedPoint = new Vector2(
                Mathf.InverseLerp(viewport.rect.xMin, viewport.rect.xMax, localPoint.x),
                Mathf.InverseLerp(viewport.rect.yMin, viewport.rect.yMax, localPoint.y)
            );

            // Check if the gaze is near the edges and adjust scroll position
            if (normalizedPoint.x < edgeThreshold)
            {
                ScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(ScrollRect.horizontalNormalizedPosition - scrollSpeed * Time.deltaTime);
            }
            else if (normalizedPoint.x > 1 - edgeThreshold)
            {
                ScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(ScrollRect.horizontalNormalizedPosition + scrollSpeed * Time.deltaTime);
            }

            if (normalizedPoint.y < edgeThreshold)
            {
                ScrollRect.verticalNormalizedPosition = Mathf.Clamp01(ScrollRect.verticalNormalizedPosition - scrollSpeed * Time.deltaTime);
            }
            else if (normalizedPoint.y > 1 - edgeThreshold)
            {
                ScrollRect.verticalNormalizedPosition = Mathf.Clamp01(ScrollRect.verticalNormalizedPosition + scrollSpeed * Time.deltaTime);
            }
        }
    }
}