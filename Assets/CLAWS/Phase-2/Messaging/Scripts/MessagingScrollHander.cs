using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagingScrollHandler : MonoBehaviour
{
    [SerializeField] private float spacing = 0.1f;
    [SerializeField] private float lerp = 0.1f;

    [SerializeField] private Renderer BoundsRenderer; // Use Renderer instead of BoxCollider
    [SerializeField] private Transform Content;

    public DynamicMessagingPop dynamicMessagingPop; // Reference to DynamicMessagingPop

    private Vector3 startBounds;
    private Vector3 endBounds;
    private float colliderOffset;
    private Vector3 scrollTarget;
    private bool isScrolling;

    private void Start()
    {
        if (dynamicMessagingPop == null)
        {
            Debug.LogError("DynamicMessagingPop script not found in the scene!");
            return;
        }

        if (BoundsRenderer == null)
        {
            Debug.LogError("BoundsRenderer is not assigned!");
            return;
        }

        colliderOffset = BoundsRenderer.bounds.center.y;
        startBounds = transform.localPosition;
        endBounds = startBounds;
        scrollTarget = startBounds;
        isScrolling = false;

        FixLocations();
    }

    private void Update()
    {
        // Update bounds based on the Renderer
        Bounds bounds = BoundsRenderer.bounds;
        colliderOffset = bounds.center.y;

        if (isScrolling)
        {
            Content.localPosition = Vector3.Lerp(Content.localPosition, scrollTarget, lerp);
            if (Mathf.Abs(Content.localPosition.y - scrollTarget.y) < 0.001f)
            {
                isScrolling = false;
            }
        }

        if (Content.localPosition.y < 0)
        {
            Content.localPosition = Vector3.Lerp(Content.localPosition, startBounds, lerp);
        }
        else if (Content.localPosition.y > endBounds.y)
        {
            Content.localPosition = Vector3.Lerp(Content.localPosition, endBounds, lerp);
        }
    }

    public void FixLocations()
    {
        if (dynamicMessagingPop == null) return;

        // Use the clones list from DynamicMessagingPop
        List<Transform> clones = dynamicMessagingPop.lmccClones;

        for (int i = 0; i < clones.Count; i++)
        {
            float yOffset;

            if (i == 0)
            {
                yOffset = 0;
            }
            else
            {
                yOffset = clones[i - 1].localPosition.y
                    - (clones[i - 1].GetComponent<BoxCollider>().size.y / 2 * clones[i - 1].localScale.y)
                    - (clones[i].GetComponent<BoxCollider>().size.y / 2 * clones[i].localScale.y)
                    - spacing;
            }

            Vector3 newPosition = new Vector3(clones[i].localPosition.x, yOffset, clones[i].localPosition.z);
            clones[i].localPosition = newPosition;
        }

        if (clones.Count > 0)
        {
            endBounds = new Vector3(endBounds.x,
                -clones[clones.Count - 1].localPosition.y
                + (clones[clones.Count - 1].GetComponent<BoxCollider>().size.y / 2 * clones[clones.Count - 1].localScale.y),
                endBounds.z);
        }
    }
}