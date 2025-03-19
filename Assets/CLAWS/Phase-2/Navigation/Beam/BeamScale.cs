using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadScaleAndMove : MonoBehaviour
{
    public Transform player;
    public float minDistance = 2.0f;
    public float maxDistance = 20.0f;
    public float minScale = 0.5f;
    public float maxScale = 2.0f;
    public float minHeight = 0.0f;
    public float maxHeight = 5.0f;
    
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        if (player == null)
            player = Camera.main.transform;
    }
    
    void Update()
    {
        // Handle scaling and vertical movement
        float distance = Vector3.Distance(transform.position, player.position);
        
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        float scaleFactor = Mathf.Lerp(minScale, maxScale, t);
        
        transform.localScale = initialScale * scaleFactor;
        
        float heightOffset = Mathf.Lerp(minHeight, maxHeight, t);
        
        transform.position = new Vector3(
            initialPosition.x, 
            initialPosition.y + heightOffset, 
            initialPosition.z
        );
        
        // Make the beam look at the camera, but only use Y rotation
        Vector3 directionToCamera = player.position - transform.position;
        directionToCamera.y = 0; // Zero out the Y component to only rotate on Y axis
        
        if (directionToCamera != Vector3.zero)
        {
            // Create rotation that looks at player
            Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
            
            // Extract just the Y rotation
            Vector3 eulerAngles = lookRotation.eulerAngles;
            
            // Apply only the Y rotation while preserving original X and Z rotations
            transform.rotation = Quaternion.Euler(
                initialRotation.eulerAngles.x,
                eulerAngles.y + 180f,
                initialRotation.eulerAngles.z
            );
        }
    }
}
