using UnityEngine;

public class BreadcrumbTrail : MonoBehaviour
{
    public Transform targetBlock; // End point
    public Transform dangerZone; // Object representing danger zone
    public float dangerRadius = 1f; // Radius of the danger zone
    public int curveResolution = 5; // Number of points for curved path

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
            Vector3 playerPos = playerTransform.position + new Vector3(0, -0.1f, 0);
            Vector3 targetPos = targetBlock.position;

            if (IsRayIntersectingDangerZone(playerPos, targetPos))
            {
                GenerateCurvedPath(playerPos, targetPos);
            }
            else
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, playerPos);
                lineRenderer.SetPosition(1, targetPos);
            }
        }
    }

    bool IsRayIntersectingDangerZone(Vector3 start, Vector3 end)
    {
        Vector3 dangerCenter = dangerZone.position;
        Vector3 closestPoint = ClosestPointOnLine(start, end, dangerCenter);
        return Vector3.Distance(closestPoint, dangerCenter) < dangerRadius;
    }

    Vector3 ClosestPointOnLine(Vector3 start, Vector3 end, Vector3 point)
    {
        Vector3 lineDir = (end - start).normalized;
        float t = Vector3.Dot(point - start, lineDir);
        t = Mathf.Clamp(t, 0, Vector3.Distance(start, end));
        return start + lineDir * t;
    }

    void GenerateCurvedPath(Vector3 start, Vector3 end)
{
    Vector3 dangerCenter = dangerZone.position;
    Vector3 closestPoint = ClosestPointOnLine(start, end, dangerCenter);
    
    // Calculate avoidance direction, ignoring Y-axis 
    // (we want ray to stay flat or parallel to the ground)
    Vector3 avoidDir = (closestPoint - dangerCenter);
    avoidDir.y = 0; // Flatten to stay parallel to the ground
    avoidDir = avoidDir.normalized;

    // Set the midpoint away from the danger zone 
    Vector3 midPoint = closestPoint + avoidDir * dangerRadius * 1.2f;
    midPoint.y = start.y; // Ensure it remains at the same Y level as the player!

    // Create smooth curve using Bezier interpolation (idk what this is, chatgpt recommended this method)
    // https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    lineRenderer.positionCount = curveResolution;
    for (int i = 0; i < curveResolution; i++)
    {
        float t = i / (float)(curveResolution - 1);
        Vector3 curvePoint = QuadraticBezier(start, midPoint, end, t);
        curvePoint.y = start.y; // Keep all points flat!
        lineRenderer.SetPosition(i, curvePoint);
    }
}

    // Bezier interpolation 
    Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }
}



