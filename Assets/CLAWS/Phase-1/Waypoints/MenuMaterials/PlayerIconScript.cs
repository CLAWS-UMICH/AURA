using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIconScript : MonoBehaviour
{
    public Transform playerTransform;

    // Update is called once per frame
    void Update()
    {
            // Align position with player, keeping Y constant
            Vector3 newPosition = playerTransform.position;
            newPosition.y = transform.position.y; // Maintain the icon's height
            transform.position = newPosition;

            // Apply only the player's Z rotation to icon
            float playerZRotation = playerTransform.eulerAngles.y; // Use Y for horizontal rotation
            transform.rotation = Quaternion.Euler(90, 0, -playerZRotation); // Adjust as needed
    }      
}
