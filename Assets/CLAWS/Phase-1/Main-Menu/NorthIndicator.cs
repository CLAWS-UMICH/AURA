using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NorthIndicator : MonoBehaviour
{
    public Transform player;                 // Reference to the player or main camera
    public RectTransform minimapCenter;      // Reference to the minimap's RectTransform (center of the minimap)
    private Vector3 initialPositionOffset;   // Stores the initial offset of the indicator from the center

    private void Start()
    {
        // Calculate the initial offset from the center of the minimap to the top position
        initialPositionOffset = transform.localPosition - minimapCenter.localPosition;
    }

    private void Update()
    {
        // Calculate the angle between the player's forward direction and world north (z-axis)
        Vector3 northDirection = Vector3.forward;  // North direction in world space
        Vector3 playerDirection = player.forward;

        // Find the angle to position the "N" symbol relative to north
        float angle = Vector3.SignedAngle(northDirection, playerDirection, Vector3.up);

        // Convert the angle to radians
        float radians = angle * Mathf.Deg2Rad;

        // Calculate the new position along the circumference based on the angle
        float x = initialPositionOffset.x * Mathf.Cos(radians) - initialPositionOffset.y * Mathf.Sin(radians);
        float y = initialPositionOffset.x * Mathf.Sin(radians) + initialPositionOffset.y * Mathf.Cos(radians);

        // Set the local position of the "N" indicator
        transform.localPosition = minimapCenter.localPosition + new Vector3(x, y, 0);
    }
}
