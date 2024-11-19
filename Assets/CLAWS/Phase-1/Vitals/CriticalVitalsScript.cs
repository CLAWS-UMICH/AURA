using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CriticalVitalsScript : MonoBehaviour
{
    private Subscription<UpdatedVitalsEvent> vitalsUpdateEvent;
    private Subscription<FellowAstronautVitalsDataChangeEvent> fellowVitalsUpdateEvent;
    [SerializeField] private GameObject heartRate, oxygenCons, carbonProd, temp;
    [SerializeField] private GameObject heartRate2, oxygenCons2, carbonProd2, temp2;
    // Start is called before the first frame update
    void Start()
    {
        vitalsUpdateEvent = EventBus.Subscribe<UpdatedVitalsEvent>(onVitalsUpdate);
        fellowVitalsUpdateEvent = EventBus.Subscribe<FellowAstronautVitalsDataChangeEvent>(onFellowVitalsUpdate);
    }

    private void onVitalsUpdate(UpdatedVitalsEvent e) {
        // astr1 vitals update
        oxygenCons.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_consumption.ToString("F1");
        heartRate.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.heart_rate.ToString("F0");
        carbonProd.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.co2_production.ToString("F1");
        temp.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.temperature.ToString("F0");
    }

    private void onFellowVitalsUpdate(FellowAstronautVitalsDataChangeEvent e) {
        // astr2 vitals update
        oxygenCons2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.oxy_consumption.ToString("F1");
        heartRate2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.heart_rate.ToString("F0");
        carbonProd2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.co2_production.ToString("F1");
        temp2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.temperature.ToString("F0");

    }

    private void OnDestroy() {
        EventBus.Unsubscribe(vitalsUpdateEvent);
        EventBus.Unsubscribe(fellowVitalsUpdateEvent);
    }
}
