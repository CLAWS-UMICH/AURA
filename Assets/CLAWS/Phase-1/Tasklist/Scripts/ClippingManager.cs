using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.GraphicsTools;

public class ClippingManager : MonoBehaviour
{
    [SerializeField] private ClippingBox clipping;
    [SerializeField] public List<GameObject> objectsToClip;

    private Subscription<InitPopFinishedEvent> initEventClip;

    void Start()
    {
        initEventClip = EventBus.Subscribe<InitPopFinishedEvent>(CallRenderers);
    }

    private void CallRenderers(InitPopFinishedEvent e)
    {
        SetRenderers();
    }

    public void SetRenderers()
    {
        clipping.ClearRenderers();

        foreach (GameObject g in objectsToClip)
        {
            Renderer[] renderers = g.GetComponentsInChildren<Renderer>();

            foreach (Renderer r in renderers)
            {
                clipping.AddRenderer(r);
            }
        }
    }


}

