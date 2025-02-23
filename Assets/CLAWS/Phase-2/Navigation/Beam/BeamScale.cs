using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamScale : MonoBehaviour
{
    public Transform player;
    public float scaleFactor = 1f;
    public float minScale = 0.1f;
    public float maxScale = 10f;
    public float minHeight = -1f;
    public float maxHeight = 10f;
    public float referenceDistance = 50f;

    private Vector3 originalScale;
    private float originalY;

    void Start()
    {
        if (player == null)
        {
            player = Camera.main.transform;
        }

        originalScale = transform.localScale;
        originalY = transform.position.y;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        float scaleFraction = Mathf.Clamp01(distance / referenceDistance);

        // Scale the beam (taller and wider when far, shorter and skinnier when close)
        float heightScale = Mathf.Lerp(minScale, maxScale, scaleFraction);
        float widthScale = Mathf.Lerp(minScale, maxScale, scaleFraction);
        Vector3 newScale = new Vector3(
            originalScale.x * widthScale * scaleFactor,
            originalScale.y * heightScale * scaleFactor,
            originalScale.z * widthScale * scaleFactor
        );
        transform.localScale = newScale;

        // Adjust height (lower when close, higher when far)
        float newY = Mathf.Lerp(minHeight, maxHeight, scaleFraction);
        Vector3 newPosition = transform.position;
        newPosition.y = originalY + newY;
        transform.position = newPosition;
    }
}
