using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointScript : MonoBehaviour
{
    [SerializeField] private Transform playerIcon;  // Reference to the player icon's transform

    // Update is called once per frame
    void Update()
    {
        // Get the player's Y rotation (horizontal rotation only)
        float playerYRotation = playerIcon.eulerAngles.y;

        if (playerYRotation < 0)
        {
            float makeUpwards = Mathf.Abs(-180 - playerYRotation);
            playerYRotation = makeUpwards;
        }

        // Set the waypoint's rotation to match only the Y rotation of the player icon
        // Lock the X and Z rotation to prevent flipping
        transform.rotation = Quaternion.Euler(90, playerYRotation, 0);
    }
}
