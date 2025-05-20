using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PRMinimap : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;        // Reference to the player's transform

    
    private Subscription<PR_LocationUpdatedEvent> locationUpdateEvent; 

    private void Start()
    {
        locationUpdateEvent = EventBus.Subscribe<PR_LocationUpdatedEvent>(OnPRLocationUpdated);        
    }

    private void OnPRLocationUpdated(PR_LocationUpdatedEvent evt)
    {


        Vector3 newPosition = new Vector3(
                (float)evt.data.posX,
                transform.position.y,                     
                (float)evt.data.posY
            );
            transform.position = newPosition;


            // Apply only the player's Z rotation to icon
            float playerZRotation = playerTransform.eulerAngles.y; // Use Y for horizontal rotation
            transform.rotation = Quaternion.Euler(90, 0, -playerZRotation); // Adjust as needed
    }




    // Update is called once per frame


    private void Update()
    {
        
    }

    private void OnDestroy() 
    {
        EventBus.Unsubscribe(locationUpdateEvent);
    }
} 