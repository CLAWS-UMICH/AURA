using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using MixedReality.Toolkit.UX;

public class CriticalVitalsScript : MonoBehaviour
{
    private Subscription<UpdatedVitalsEvent> vitalsUpdateEvent;
    private Subscription<FellowAstronautVitalsDataChangeEvent> fellowVitalsUpdateEvent;
    [SerializeField] private GameObject heartRate, oxygenCons, carbonProd, temp, oxyTime; // priFan, secFan, scrubA, scrubB;
    [SerializeField] private GameObject heartRate2, oxygenCons2, carbonProd2, temp2, oxyTime2; //priFan2, secFan2, scrubA2, scrubB2;
    // Start is called before the first frame update
    [SerializeField] private Slider oxygenSlider, oxygenSlider2;
    void Start()
    {
        vitalsUpdateEvent = EventBus.Subscribe<UpdatedVitalsEvent>(onVitalsUpdate);
        fellowVitalsUpdateEvent = EventBus.Subscribe<FellowAstronautVitalsDataChangeEvent>(onFellowVitalsUpdate);
    }

    private void onVitalsUpdate(UpdatedVitalsEvent e) {
        // astr1 vitals update
        oxygenCons.transform.Find("O2Cnum").GetComponent<TextMeshPro>().text = e.vitals.oxy_consumption.ToString("F1");
        heartRate.transform.Find("HRnum").GetComponent<TextMeshPro>().text = e.vitals.heart_rate.ToString("F0");
        carbonProd.transform.Find("CO2Pnum").GetComponent<TextMeshPro>().text = e.vitals.co2_production.ToString("F1");
        temp.transform.Find("Tempnum").GetComponent<TextMeshPro>().text = e.vitals.temperature.ToString("F0");
        // Helmet fan
        // priFan.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.fan_pri_rpm.ToString().Substring(0, Mathf.Min(2, e.vitals.fan_pri_rpm.ToString().Length)) + "k";
        // secFan.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.fan_sec_rpm.ToString().Substring(0, Mathf.Min(2, e.vitals.fan_sec_rpm.ToString().Length)) + "k";
        // // Scrubbers
        // scrubA.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.scrubber_a_co2_storage.ToString("F0");
        // scrubB.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.scrubber_b_co2_storage.ToString("F0");
        int time_left = e.vitals.oxy_time_left;
        int hours = time_left / 3600;
        int minutes = (time_left % 3600) / 60;
        string timeLeftString = $"{hours}h {minutes}m";
        
        oxyTime.transform.Find("BodyText").GetComponent<TextMeshPro>().text = timeLeftString;
        if (oxygenSlider != null)
        {
            Debug.Log("Oxygen vital instantiated");
            oxygenSlider.Value = e.vitals.oxy_time_left; // Normalize value to 0-1
        }
    }

    private void onFellowVitalsUpdate(FellowAstronautVitalsDataChangeEvent e) {
        // astr2 vitals update
        oxygenCons2.transform.Find("O2Cnum").GetComponent<TextMeshPro>().text = e.vitals.oxy_consumption.ToString("F1");
        heartRate2.transform.Find("HRnum").GetComponent<TextMeshPro>().text = e.vitals.heart_rate.ToString("F0");
        carbonProd2.transform.Find("CO2Pnum").GetComponent<TextMeshPro>().text = e.vitals.co2_production.ToString("F1");
        temp2.transform.Find("Tempnum").GetComponent<TextMeshPro>().text = e.vitals.temperature.ToString("F0");
        // Helmet fan
        // priFan.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.fan_pri_rpm.ToString().Substring(0, Mathf.Min(2, e.vitals.fan_pri_rpm.ToString().Length)) + "k";
        // secFan.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.fan_sec_rpm.ToString().Substring(0, Mathf.Min(2, e.vitals.fan_sec_rpm.ToString().Length)) + "k";
        // // Scrubbers
        // scrubA.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.scrubber_a_co2_storage.ToString("F0");
        // scrubB.transform.Find("BodyText").GetComponent<TextMeshPro>().text = e.vitals.scrubber_b_co2_storage.ToString("F0");
        int time_left = e.vitals.oxy_time_left;
        int hours = time_left / 3600;
        int minutes = (time_left % 3600) / 60;
        string timeLeftString = $"{hours}h {minutes}m";

        oxyTime2.transform.Find("BodyText").GetComponent<TextMeshPro>().text = timeLeftString;
        if (oxygenSlider2 != null)
        {
            oxygenSlider2.Value = e.vitals.oxy_time_left; // Normalize value to 0-1
        }
    }

    private void OnDestroy() {
        EventBus.Unsubscribe(vitalsUpdateEvent);
        EventBus.Unsubscribe(fellowVitalsUpdateEvent);
    }
}
