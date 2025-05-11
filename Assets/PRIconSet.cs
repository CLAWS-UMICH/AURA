using System.Collections;
using System.Collections.Generic;
using GLTFast.Schema;
using UnityEngine;
using UnityEngine.UI;

public class PRIconSet : MonoBehaviour
{
    [SerializeField] private GameObject fullMapIcon;


    private Subscription<PR_LocationUpdatedEvent> PRLocationUpdatedEvent;

    

    private void OnPRLocationUpdated(PR_LocationUpdatedEvent e)
    {
        Debug.Log($"PR location updated: X = {e.data.posX}, Y = {e.data.posY}, Z = {e.data.posZ}");
        fullMapIcon.transform.position = new Vector3((float)e.data.posX - (float)AstronautInstance.User.origin.posX, 
                                                        (float)e.data.posY - (float)AstronautInstance.User.origin.posZ, 
                                                        (float)e.data.posZ - (float)AstronautInstance.User.origin.posY);
    }

    private void OnDestroy()
    { 
        EventBus.Unsubscribe(PRLocationUpdatedEvent);
    }
}