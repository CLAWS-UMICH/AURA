using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointScript : MonoBehaviour
{
    [SerializeField] private Transform playerIcon;  // Reference to the player icon's transform

    private float currentRotationY;

    private void Start()
    {
        // Initialize the waypoint's rotation based on the player's initial rotation
        currentRotationY = NormalizeAngle(playerIcon.eulerAngles.y);
    }

    // Update is called once per frame
    void Update()
    {
        // Get the player's Y rotation (horizontal rotation only)
        float targetRotationY = NormalizeAngle(playerIcon.eulerAngles.y);

        // Ensure the rotation stays in the upright range
        float rotationDifference = Mathf.DeltaAngle(currentRotationY, targetRotationY);

        // Prevent any flipping behavior
        if (Mathf.Abs(rotationDifference) > 90f)
        {
            targetRotationY = currentRotationY; // Ignore the rotation if it would cause a flip
        }

        // Smoothly interpolate the rotation to avoid jitter
        currentRotationY = Mathf.LerpAngle(currentRotationY, targetRotationY, Time.deltaTime * 10f);

        // Apply the corrected rotation, locking X to 90 to keep it upright
        transform.rotation = Quaternion.Euler(90, currentRotationY, 0);
    }

    // Utility function to normalize angles to the 0-360 range
    private float NormalizeAngle(float angle)
    {
        while (angle < 0) angle += 360;
        while (angle >= 360) angle -= 360;
        return angle;
    }
}