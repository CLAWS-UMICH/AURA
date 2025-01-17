using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CriticalVitalsScript : MonoBehaviour
{
    private Subscription<UpdatedVitalsEvent> vitalsUpdateEvent;
    private Subscription<FellowAstronautVitalsDataChangeEvent> fellowVitalsUpdateEvent;
    [SerializeField] private GameObject heartRate, oxygenCons, carbonProd, temp; // priFan, secFan, scrubA, scrubB;
    [SerializeField] private GameObject heartRate2, oxygenCons2, carbonProd2, temp2; //priFan2, secFan2, scrubA2, scrubB2;
    // Start is called before the first frame update
    void Start()
    {
        vitalsUpdateEvent = EventBus.Subscribe<UpdatedVitalsEvent>(onVitalsUpdate);
        fellowVitalsUpdateEvent = EventBus.Subscribe<FellowAstronautVitalsDataChangeEvent>(onFellowVitalsUpdate);
    }

    private void onVitalsUpdate(UpdatedVitalsEvent e) {
        // astr1 vitals update
        //checkAlerts(e.vitals);
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
        // VitalsData vitalsData = new VitalsData
        //     {
        //         type = "VITALS",
        //         use = "POST",
        //         data = AstronautInstance.User.VitalsData
        //     };
        // string json = JsonUtility.ToJson(vitalsData);
        // webSocketClient.SendJsonData(json, "VITALS");
        // Debug.Log(json);

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

    }

    // private void checkAlerts(Vitals vitals)
    // {
    //     if (vitals.heart_rate > 150)
    //     {
    //         CreateAlert alert = new Alert {

    //         }
    //         EventBus.Publish<
    //         Debug.Log("Warning: Heart rate is too high!");
    //     }
    // }

    private void OnDestroy() {
        EventBus.Unsubscribe(vitalsUpdateEvent);
        EventBus.Unsubscribe(fellowVitalsUpdateEvent);
    }
}
