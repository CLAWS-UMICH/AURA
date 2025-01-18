using UnityEngine;

public class BreadcrumbTrail : MonoBehaviour
{
    public Transform targetBlock; // Reference to the block object
    private LineRenderer lineRenderer;
    private Transform playerTransform; // Reference to the Main Camera for player position

    void Start()
    {
        // Get the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();

        // Set the number of points to 2
        lineRenderer.positionCount = 2;

        // Set the material color to blue
        //lineRenderer.material.color = Color.blue;

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
            // Add an optional height offset to the player's position
            Vector3 playerPos = playerTransform.position + new Vector3(0, -0.1f, 0); // Adjust offset as needed

            // Update the positions of the line's endpoints
            lineRenderer.SetPosition(0, playerPos);
            lineRenderer.SetPosition(1, targetBlock.position);
        }
    }
}


