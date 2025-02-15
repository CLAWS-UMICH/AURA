using UnityEngine;

public class BreadcrumbTrail : MonoBehaviour
{
    public Transform targetBlock; // End point
    public Transform[] dangerZones; // Array of objects representing danger zones
    public float dangerRadius = 1f; // Radius of each danger zone
    public int curveResolution = 10; // Number of points for curved path
    public LayerMask groundLayer; // Layer mask to detect the floor
    public float breadcrumbHeightOffset = 0.05f; // Small offset to prevent clipping

    private LineRenderer lineRenderer;
    private Transform playerTransform; // Player position reference

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Find the Main Camera (player's position reference)
        playerTransform = Camera.main?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Main Camera not found! Ensure the Main Camera is tagged as 'MainCamera'.");
        }
    }

    void Update()
    {
        if (playerTransform != null && targetBlock != null)
        {
            // Get ground-adjusted positions
            Vector3 playerPos = GetGroundPosition(playerTransform.position);
            Vector3 targetPos = GetGroundPosition(targetBlock.position);

            Transform closestDangerZone = null;
            Vector3 closestIntersection = Vector3.zero;
            float minDist = float.MaxValue;

            // Check if the breadcrumb line intersects with any danger zone
            foreach (Transform dangerZone in dangerZones)
            {
                Vector3 intersection;
                if (IsRayIntersectingDangerZone(playerPos, targetPos, dangerZone, out intersection))
                {
                    float dist = Vector3.Distance(playerPos, intersection);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestDangerZone = dangerZone;
                        closestIntersection = intersection;
                    }
                }
            }

            if (closestDangerZone != null)
            {
                GenerateCurvedPath(playerPos, targetPos, closestDangerZone, closestIntersection);
            }
            else
            {
                DrawBreadcrumbPath(playerPos, targetPos);
            }
        }
    }

    void DrawBreadcrumbPath(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = curveResolution;
        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            point = GetGroundPosition(point); // Ensure breadcrumb is set on the ground
            lineRenderer.SetPosition(i, point);
        }
    }

    Vector3 GetGroundPosition(Vector3 originalPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(originalPosition + Vector3.up * 2f, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            return new Vector3(originalPosition.x, hit.point.y + breadcrumbHeightOffset, originalPosition.z);
        }
        return new Vector3(originalPosition.x, 0.05f, originalPosition.z); // Default to a flat plane if no ground detected
    }

    bool IsRayIntersectingDangerZone(Vector3 start, Vector3 end, Transform dangerZone, out Vector3 intersection)
    {
        Vector3 dangerCenter = dangerZone.position;
        Vector3 closestPoint = ClosestPointOnLine(start, end, dangerCenter);
        float distance = Vector3.Distance(closestPoint, dangerCenter);

        if (distance < dangerRadius)
        {
            intersection = closestPoint;
            return true;
        }
        intersection = Vector3.zero;
        return false;
    }

    Vector3 ClosestPointOnLine(Vector3 start, Vector3 end, Vector3 point)
    {
        Vector3 lineDir = (end - start).normalized;
        float t = Vector3.Dot(point - start, lineDir);
        t = Mathf.Clamp(t, 0, Vector3.Distance(start, end));
        return start + lineDir * t;
    }

    void GenerateCurvedPath(Vector3 start, Vector3 end, Transform dangerZone, Vector3 closestPoint)
    {
        Vector3 dangerCenter = dangerZone.position;
        
        // Calculate avoidance direction, ignoring Y-axis 
        Vector3 avoidDir = (closestPoint - dangerCenter);
        avoidDir.y = 0; // Keep the curve flat
        avoidDir = avoidDir.normalized;

        // Set the midpoint away from the danger zone 
        Vector3 midPoint = closestPoint + avoidDir * dangerRadius * 1.5f;
        midPoint = GetGroundPosition(midPoint); // Ensure it stays on the ground

        // Create smooth curve using Bezier interpolation
        lineRenderer.positionCount = curveResolution;
        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);
            Vector3 curvePoint = QuadraticBezier(start, midPoint, end, t);
            curvePoint = GetGroundPosition(curvePoint); // Keep each point on the ground
            lineRenderer.SetPosition(i, curvePoint);
        }
    }

    Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }
}




