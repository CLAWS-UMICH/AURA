using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CriticalVitalsScript : MonoBehaviour
{
    private Subscription<VitalsUpdatedEvent> vitalsUpdateEvent;
    private Subscription<FellowAstronautVitalsDataChangeEvent> fellowVitalsUpdateEvent;
    [SerializeField] private GameObject heartRate, oxygenCons, carbonProd, temp;
    [SerializeField] private GameObject heartRate2, oxygenCons2, carbonProd2, temp2;
    // Start is called before the first frame update
    void Start()
    {
        vitalsUpdateEvent = EventBus.Subscribe<VitalsUpdatedEvent>(onVitalsUpdate);
        fellowVitalsUpdateEvent = EventBus.Subscribe<FellowAstronautVitalsDataChangeEvent>(onFellowVitalsUpdate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
