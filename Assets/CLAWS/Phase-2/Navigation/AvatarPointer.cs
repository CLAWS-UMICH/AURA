using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPointer : MonoBehaviour
{
    public Transform centerQuad;
    private float rotationRadius = 1.3f; // Set to 1.3
    private float smoothSpeed = 10f;

    [SerializeField] private float rotationAngle = 0f; // Only serialized field

    private Vector3 startingRotation = new Vector3(90f, 0f, -40f); // Set to (90, 0, -40)

    private void Start()
    {
        // Apply the starting rotation
        transform.rotation = Quaternion.Euler(startingRotation);
    }

    private void Update()
    {
        UpdateRotation(rotationAngle);
    }

    public void UpdateRotation(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float x = rotationRadius * Mathf.Cos(radians);
        float z = rotationRadius * Mathf.Sin(radians);
        
        // Position the quad 0.05 below the center quad on the y axis
        Vector3 targetPosition = centerQuad.position + new Vector3(x, -0.05f, z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        Vector3 awayFromCenter = transform.position - centerQuad.position;
        Quaternion targetRotation = Quaternion.LookRotation(awayFromCenter, Vector3.up);
        targetRotation *= Quaternion.Euler(startingRotation);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}
