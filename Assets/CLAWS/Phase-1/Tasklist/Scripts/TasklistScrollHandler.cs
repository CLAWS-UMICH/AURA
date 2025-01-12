using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskListScrollHandler : MonoBehaviour
{
    [SerializeField] private float spacing = 0.1f; // Distance between gameobjects
    [SerializeField] private float lerp = 0.1f;

    [SerializeField] private BoxCollider Bounds;
    [SerializeField] private Transform Content;

    private List<Transform> Objects = new List<Transform>(); // List to store all buttons

    private Vector3 startBounds;
    private Vector3 endBounds;
    private float colliderOffset;
    private Vector3 scrollTarget;
    private bool isScrolling;

    private Subscription<InitPopFinishedEvent> initEvent;

    /// <summary>
    /// Returns the index of the first visible element (visible by more than 1/2 showing).
    /// </summary>
    int GetScrollIndex()
    {
        float min_distance = float.MaxValue;
        int min_index = 0;

        int index = 0;
        foreach (Transform t in Objects)
        {
            float distance = Mathf.Abs(Content.position.y - spacing + t.localPosition.y);
            if (distance < min_distance)
            {
                min_distance = distance;
                min_index = index;
            }
            index++;
        }

        return min_index;
    }

    [ContextMenu("Func ScrollUp")]
    public void ScrollUp()
    {
        Debug.Log("Scroll up");
        int index = GetScrollIndex() - 1;

        if (index < 0)
        {
            index = 0;
        }

        ScrollTo(index);
    }

    [ContextMenu("Func ScrollDown")]
    public void ScrollDown()
    {
        Debug.Log("Scroll down");
        int index = GetScrollIndex() + 1;

        if (index > Objects.Count - 1)
        {
            index = Objects.Count - 1;
        }

        ScrollTo(index);
    }

    void ScrollTo(int index_f)
    {
        isScrolling = true;
        scrollTarget = new Vector3(startBounds.x,
                -Objects[index_f].transform.localPosition.y,
                startBounds.z);
    }

    private void Awake()
    {
        initEvent = EventBus.Subscribe<InitPopFinishedEvent>(OnSub);
        Debug.LogWarning("subscribed");
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(initEvent);
    }

    private void OnSub(InitPopFinishedEvent e)
    {;
        colliderOffset = Bounds.center.y;

        startBounds = transform.localPosition;
        endBounds = startBounds;
        scrollTarget = startBounds;
        isScrolling = false;

        FixLocations();
    }

    private void Update()
    {
        // update the box collider to be scrollable
        Bounds.center = new Vector3(Bounds.center.x, colliderOffset - Bounds.transform.localPosition.y, Bounds.center.z);


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

    /// <summary>
    /// Adds all buttons to a list. Used in case of new buttons added in the middle of the run.
    /// </summary>
    void CollectAllButtons()
    {
        Transform parentTransform = transform;
        Objects.Clear();

        foreach (Transform child in parentTransform)
        {
            if (child.name != "Grid Layout")
            {
                Objects.Add(child);
            }
        }

    }

    /// <summary>
    /// Fixes the layout of all objects in the scroll handler.
    /// </summary>
    [ContextMenu("Func FixLayout")]
    public void FixLocations()
    {
        CollectAllButtons();

        for (int i = 0; i < Objects.Count; i++)
        {
            float yOffset;
            bool taskToSub = false;

            if (i == 0)
            {
                yOffset = 0;
            }
            else
            {
                string prev_n = Objects[i - 1].name;
                string curr_n = Objects[i].name;
                taskToSub = (prev_n[0] == 'T' && curr_n[0] == 'S');
                if (taskToSub)
                {
                    yOffset = Objects[i - 1].transform.localPosition.y
                    - (Objects[i - 1].GetComponent<BoxCollider>().size.y / 2
                        * Objects[i - 1].transform.localScale.y)
                    - (Objects[i-1].GetComponent<BoxCollider>().size.y
                        * Objects[i-1].transform.localScale.y)
                    - spacing;
                }
                else
                {
                    yOffset = Objects[i - 1].transform.localPosition.y
                    - (Objects[i - 1].GetComponent<BoxCollider>().size.y / 2
                        * Objects[i - 1].transform.localScale.y)
                    - (Objects[i].GetComponent<BoxCollider>().size.y / 2
                        * Objects[i].transform.localScale.y)
                    - spacing;
                }
        
            }
            // Vector3 newPosition = parentTransform.position + new Vector3(xOffset, yOffset, 0f);
            Vector3 newPosition = new Vector3(Objects[i].transform.localPosition.x, yOffset, Objects[i].transform.localPosition.z);
            Objects[i].transform.localPosition = newPosition; // Move each button to the new position
        }

        if (Objects.Count > 0)
        {
            endBounds = new Vector3(endBounds.x,
                -Objects[Objects.Count - 1].transform.localPosition.y
                + (Objects[Objects.Count - 1].GetComponent<BoxCollider>().size.y / 2
                            * Objects[Objects.Count - 1].transform.localScale.y),
                endBounds.z);
        }
    }

}
