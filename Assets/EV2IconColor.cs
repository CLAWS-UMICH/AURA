using System.Collections;
using System.Collections.Generic;
using GLTFast.Schema;
using UnityEngine;
using UnityEngine.UI;

public class EV2IconColor : MonoBehaviour
{
    [SerializeField] private GameObject fullMapIcon;
    [SerializeField] private GameObject EVmapIcon;
    [SerializeField] private GameObject buttonIcon;

    private Dictionary<string, int> colorMap = new Dictionary<string, int>
    {
        { "red", 0xB02F2F },
        { "blue", 0x38A7AD },
        { "green", 0x29842E },
        { "yellow", 0xE09A12 },
        { "pink", 0xAD0E8E },
        { "orange", 0xD45409 }
    };

    // TODO: match the scene view color to the button color for ev2
     private Dictionary<string, int> colorMapButton = new Dictionary<string, int>
    {
        { "red", 0x890000 },
        { "blue", 0x0 },
        { "green", 0x0 },
        { "yellow", 0x0 },
        { "pink", 0x0 },
        { "orange", 0x0 }
    };

    private Subscription<EV2_LocationUpdatedEvent> EV2LocationUpdatedEvent;

    void Start()
    {
        EV2LocationUpdatedEvent = EventBus.Subscribe<EV2_LocationUpdatedEvent>(OnEV2LocationUpdated);
        string userColor = AstronautInstance.User.fellowAstronaut.color;
        if (colorMap.ContainsKey(userColor))
        {
            int hexValue = colorMap[userColor];
            Color color = HexToColor(hexValue);

            Renderer fullMapRenderer = fullMapIcon.GetComponent<Renderer>();
            if (fullMapRenderer != null)
            {
                fullMapRenderer.material.color = color;
                Debug.Log($"Set fullMapIcon color to {userColor} with hex value {hexValue:X}");
            }
            else
            {
                Debug.LogError("Renderer component not found on fullMapIcon.");
            }

            Renderer evMapRenderer = EVmapIcon.GetComponent<Renderer>();
            if (evMapRenderer != null)
            {
                evMapRenderer.material.color = color;
                Debug.Log($"Set EVmapIcon color to {userColor} with hex value {hexValue:X}");
            }
            else
            {
                Debug.LogError("Renderer component not found on EVmapIcon.");
            }

            // TODO: Set button color
        }
    }

    private void OnEV2LocationUpdated(EV2_LocationUpdatedEvent e)
    {
        fullMapIcon.transform.position = new Vector3((float)e.data.posX, (float)e.data.posY, (float)e.data.posZ);
        EVmapIcon.transform.position = new Vector3((float)e.data.posX, (float)e.data.posY, (float)e.data.posZ);
    }

    private Color HexToColor(int hex)
    {
        float r = ((hex >> 16) & 0xFF) / 255f;
        float g = ((hex >> 8) & 0xFF) / 255f;
        float b = (hex & 0xFF) / 255f;
        return new Color(r, g, b);
    }

    private void OnDestroy()
    { 
        EventBus.Unsubscribe(EV2LocationUpdatedEvent);
    }
}