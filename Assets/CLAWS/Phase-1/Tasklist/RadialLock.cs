/* 
Haotian Li
UI Lock Feature 
*/ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialLock : MonoBehaviour
{
    public float distanceFromUser = 0.5f; 
    public float verticalOffset = -0.2f;  

    private Transform cameraTransform;
    private float initialYPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        initialYPosition = transform.position.y; // Store the initial Y position
    }

    void Update()
    {
        Vector3 targetPosition = cameraTransform.position 
                                 + cameraTransform.forward * distanceFromUser;

        // Lock the y position to the initial Y position plus verticalOffset
        targetPosition.y = initialYPosition + verticalOffset;

        transform.position = targetPosition;
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}

