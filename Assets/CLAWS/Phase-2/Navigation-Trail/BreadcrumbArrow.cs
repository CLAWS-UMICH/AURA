using UnityEngine;
using UnityEngine.UI;

public class BreadcrumbArrow : MonoBehaviour
{
    public Transform player; // Main Camera / Player 
    public LineRenderer breadcrumbTrail; // Breadcrumb trail (ray)
    public Camera mainCamera; // Reference to main camera
    public Image arrowUI; // UI Image of the arrow

    public float screenEdgeThreshold = 0.1f; // Adjust how close to the screen edge the breadcrumb should be considered "visible"

    private Vector3 lastBreadcrumbPosition; // Last breadcrumb point

    void Update()
    {
        bool breadcrumbVisible = IsBreadcrumbInView();

        if (!breadcrumbVisible)
        {
            arrowUI.enabled = true; // Show arrow when breadcrumb is off-screen
            PositionArrow();
        }
        else
        {
            arrowUI.enabled = false; // Hide arrow when breadcrumb is visible
        }
    }

    bool IsBreadcrumbInView()
    {
        for (int i = 0; i < breadcrumbTrail.positionCount; i++)
        {
            Vector3 worldPos = breadcrumbTrail.GetPosition(i);
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(worldPos);

            // If any point in the breadcrumb trail is visible, return true
            if (viewportPoint.x > screenEdgeThreshold && viewportPoint.x < (1 - screenEdgeThreshold) &&
                viewportPoint.y > screenEdgeThreshold && viewportPoint.y < (1 - screenEdgeThreshold) &&
                viewportPoint.z > 0)
            {
                lastBreadcrumbPosition = worldPos; // Save the last visible position
                return true;
            }
        }
        return false; // If no points are visible, return false
    }

    void PositionArrow()
    {
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(lastBreadcrumbPosition);
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 direction = (screenPoint - screenCenter).normalized;

        float arrowEdgeOffset = 50f; // Offset from the screen edge

        // Position the arrow at the screen edge, pointing towards the breadcrumb
        Vector3 arrowPosition = screenCenter + direction * ((Screen.width / 2) - arrowEdgeOffset);
        arrowUI.rectTransform.position = arrowPosition;

        // Rotate the arrow to point towards the breadcrumb trail
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowUI.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}



