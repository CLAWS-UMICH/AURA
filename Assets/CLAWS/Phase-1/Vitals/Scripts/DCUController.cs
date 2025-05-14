using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DCUController : MonoBehaviour
{
    private Subscription<DCUChangedEvent> dcuChangedEvent;
    private Subscription<DCUErrorEvent> dcuErrorEvent;




    // Start is called before the first frame update
    void Start()
    {
        dcuChangedEvent = EventBus.Subscribe<DCUChangedEvent>(onDcuChange);
        dcuErrorEvent = EventBus.Subscribe<DCUErrorEvent>(onDcuError);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onDcuChange(DCUChangedEvent e)
    {

    }

    private void onDcuError(DCUErrorEvent e)
    {

    }
}
