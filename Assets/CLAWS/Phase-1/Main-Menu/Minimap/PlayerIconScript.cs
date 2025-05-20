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
            // Align the icon's position with AstronautInstance.User.current.posX and posY
            Vector3 newPosition = new Vector3(
                (float)AstronautInstance.User.current.posX,
                transform.position.y,                     
                (float)AstronautInstance.User.current.posZ
            );
    
            transform.position = newPosition;


            // Apply only the player's Z rotation to icon
            float playerZRotation = playerTransform.eulerAngles.y; // Use Y for horizontal rotation
            transform.rotation = Quaternion.Euler(90, 0, -playerZRotation); // Adjust as needed
    }      
}
