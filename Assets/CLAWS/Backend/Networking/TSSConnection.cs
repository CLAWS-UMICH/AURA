using System.Collections;
using UnityEngine;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using TMPro;

public class TSSConnection : MonoBehaviour
{
    private string IPaddr;
    int team_number;
    bool connected;
    float time_since_last_update;
    public Action<bool> OnTSSConnectionResult;


    // Database Jsons
    string UIAJsonString;
    string DCUJsonString;
    string ROVERJsonString;
    string SPECJsonString;
    string TELEMETRYJsonString;
    string COMMJsonString;
    string IMUJsonString;


    // Connect to TSS
    public void ConnectToHost(string IP_host, int _team_number)
    {
        DisconnectFromHost();
        team_number = _team_number;
        AstronautInstance.User.TSSurl = "http://" + IP_host + ":" + "14141";
        // Debug.Log("Connecting to TSS at: " + AstronautInstance.User.TSSurl);
        // Test connection to frontend
        StartCoroutine(GetRequest(AstronautInstance.User.TSSurl));
    }

    IEnumerator LookForConnection()
    {
        while (true)
        {
            if (!connected && IPaddr.Length > 0 && !IPaddr.Contains("/"))
            {
                ConnectToHost(IPaddr, 1); // CHANGE 0 TO ACTUAL TEAM NUMBER IN HOUSTON
            }
            yield return new WaitForSeconds(5);
        }
    }


    // called from main connection
    public void TSSConnect(string ip)
    {
        IPaddr = ip;
        Debug.Log("IPAddr: " + IPaddr);
        StartCoroutine(LookForConnection());
    }


    // attach to final screen disconnect button
    public void DisconnectFromHost()
    {
        connected = false;
    }


    void Start()
    {
        connected = false;
    }


    void Update()
    {
        // If you are connected to TSS
        Debug.Log("Connected: " + connected);
        if (connected)
        {
            // Each Second
            time_since_last_update += Time.deltaTime;
            if (time_since_last_update > 1.0f)
            {
                // Pull TSS Updates
                // StartCoroutine(GetUIAState());
                StartCoroutine(GetDCUState()); 
                // StartCoroutine(GetROVERState()); PR TODO: ROVER not implemented yet
                StartCoroutine(GetSPECState());
                StartCoroutine(GetTELEMETRYState());
                StartCoroutine(GetCOMMState());
                StartCoroutine(GetIMUState());
                time_since_last_update = 0.0f;
            }
        }
    }


    IEnumerator GetRequest(string uri)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;
            Debug.Log(webRequest.result);
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    OnTSSConnectionResult?.Invoke(false);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    OnTSSConnectionResult?.Invoke(false);
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    OnTSSConnectionResult?.Invoke(false);
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("EXECUTED");
                    OnTSSConnectionResult?.Invoke(true);
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    connected = true;
                    break;
                default:
                    Debug.LogError("Unexpected UnityWebRequest result: " + webRequest.result);
                    OnTSSConnectionResult?.Invoke(false);
                    break;
            }

        }
    }


////////////////////////////  UIA  /////////////////////////////
    // IEnumerator GetUIAState()
    // {
    //     using (UnityWebRequest webRequest = UnityWebRequest.Get(TSSurl + "/json_data/UIA.json"))
    //     {
    //         // Request and wait for the desired page.
    //         yield return webRequest.SendWebRequest();

    //         switch (webRequest.result)
    //         {
    //             case UnityWebRequest.Result.Success:
    //                 if (UIAJsonString != webRequest.downloadHandler.text)
    //                 {
    //                     UIAJsonString = webRequest.downloadHandler.text;
    //                     AstronautInstance.User.uia = JsonUtility.FromJson<UIA>(UIAJsonString);

    //                     // EventBus.Publish(new UIAChanged(AstronautInstance.User.uia));
    //                 }
    //                 break;
    //         }

    //     }
    // }


    ////////////////////////////  DCU  /////////////////////////////
    IEnumerator GetDCUState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(AstronautInstance.User.TSSurl + "/json_data/DCU.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (DCUJsonString != webRequest.downloadHandler.text)
                    {
                        DCUJsonString = webRequest.downloadHandler.text;
                        AstronautInstance.User.dcu = JsonUtility.FromJson<DCU>(DCUJsonString);

                        if (AstronautInstance.User.id == 1)
                        {
                            // EventBus.Publish(new DCUChanged(AstronautInstance.User.dcu.dcu.eva1));
                        }
                        else
                        {
                            // EventBus.Publish(new DCUChanged(AstronautInstance.User.dcu.dcu.eva2));
                        }
                    }
                    break;
            }

        }
    }


    ////////////////////////////  SPEC  /////////////////////////////
    IEnumerator GetSPECState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(AstronautInstance.User.TSSurl + "/json_data/SPEC.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (SPECJsonString != webRequest.downloadHandler.text)
                    {
                        SPECJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.spec = JsonUtility.FromJson<SPEC>(SPECJsonString);
                        if (AstronautInstance.User.id == 1)
                        {
                            // EventBus.Publish<XRFScanEvent>(new XRFScanEvent(AstronautInstance.User.spec.spec.eva1.data));
                        } 
                        else
                        {
                            // EventBus.Publish<XRFScanEvent>(new XRFScanEvent(AstronautInstance.User.spec.spec.eva2.data));
                        }
                            
                    }
                    break;
            }

        }
    }


    // IEnumerator GetROVERState()
    // {
    //     using (UnityWebRequest webRequest = UnityWebRequest.Get(TSSurl "/json_data/ROVER.json"))
    //     {
    //         // Request and wait for the desired page.
    //         yield return webRequest.SendWebRequest();

    //         switch (webRequest.result)
    //         {
    //             case UnityWebRequest.Result.Success:
    //                 if (ROVERJsonString != webRequest.downloadHandler.text)
    //                 {
    //                     ROVERJsonString = webRequest.downloadHandler.text;
    //                     AstronautInstance.User.rover = JsonUtility.FromJson<ROVER>(this.ROVERJsonString);
    //                     // EventBus.Publish(new RoverChanged(AstronautInstance.User.rover.rover));
    //                 }
    //                 break;
    //         }

    //     }
    // }


    ////////////////////////////  EVA VITALS /////////////////////////////
    IEnumerator GetTELEMETRYState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(AstronautInstance.User.TSSurl + "/json_data/teams/" + this.team_number + "/TELEMETRY.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (TELEMETRYJsonString != webRequest.downloadHandler.text)
                    {
                        TELEMETRYJsonString = webRequest.downloadHandler.text;
                        AstronautInstance.User.telemetry = JsonUtility.FromJson<TELEMETRY>(this.TELEMETRYJsonString);
                        if (AstronautInstance.User.id == 1)
                        {
                            CopyVitals(AstronautInstance.User.vitals, AstronautInstance.User.telemetry.telemetry.eva1);
                            CopyVitals(AstronautInstance.User.fellowAstronaut.vitals, AstronautInstance.User.telemetry.telemetry.eva2);
                        } 
                        else
                        {
                            CopyVitals(AstronautInstance.User.vitals, AstronautInstance.User.telemetry.telemetry.eva2);
                            CopyVitals(AstronautInstance.User.fellowAstronaut.vitals, AstronautInstance.User.telemetry.telemetry.eva1);
                        }

                        AstronautInstance.User.vitals.eva_time = AstronautInstance.User.telemetry.telemetry.eva_time;
                        Debug.Log("TELEMETRY: " + TELEMETRYJsonString);
                        EventBus.Publish<UpdatedVitalsEvent>(new UpdatedVitalsEvent(AstronautInstance.User.vitals));
                        EventBus.Publish<UpdatedFellowAstronautVitalsEvent>(new UpdatedFellowAstronautVitalsEvent(AstronautInstance.User.fellowAstronaut.vitals));
                    }
                    break;
            }

        }
    }

    private void CopyVitals(Vitals vital, EvaTelemetryDetails t)
    {
        vital.batt_time_left = t.batt_time_left;
        vital.oxy_pri_storage = t.oxy_pri_storage;
        vital.oxy_sec_storage = t.oxy_sec_storage;
        vital.oxy_pri_pressure = t.oxy_pri_pressure;
        vital.oxy_sec_pressure = t.oxy_sec_pressure;
        vital.oxy_time_left = t.oxy_time_left;
        vital.heart_rate = t.heart_rate;
        vital.oxy_consumption = t.oxy_consumption;
        vital.co2_production = t.co2_production;
        vital.suit_pressure_oxy = t.suit_pressure_oxy;
        vital.suit_pressure_co2 = t.suit_pressure_co2;
        vital.suit_pressure_other = t.suit_pressure_other;
        vital.suit_pressure_total = t.suit_pressure_total;
        vital.fan_pri_rpm = t.fan_pri_rpm;
        vital.fan_sec_rpm = t.fan_sec_rpm;
        vital.helmet_pressure_co2 = t.helmet_pressure_co2;
        vital.scrubber_a_co2_storage = t.scrubber_a_co2_storage;
        vital.scrubber_b_co2_storage = t.scrubber_b_co2_storage;
        vital.temperature = t.temperature;
        vital.coolant_m = t.coolant_m;
        vital.coolant_gas_pressure = t.coolant_gas_pressure;
        vital.coolant_liquid_pressure = t.coolant_liquid_pressure;
    }


    ////////////////////////////  COMMS  /////////////////////////////
    IEnumerator GetCOMMState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(AstronautInstance.User.TSSurl + "/json_data/COMM.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (COMMJsonString != webRequest.downloadHandler.text)
                    {
                        COMMJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.comm = JsonUtility.FromJson<COMM>(this.COMMJsonString);

                        // EventBus.Publish(new CommChanged(AstronautInstance.User.comm.comm));
                    }
                    break;
            }

        }
    }


    ////////////////////////////  IMU/GPS  /////////////////////////////
    IEnumerator GetIMUState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(AstronautInstance.User.TSSurl + "/json_data/IMU.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (IMUJsonString != webRequest.downloadHandler.text)
                    {
                        IMUJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.imu = JsonUtility.FromJson<IMU>(this.IMUJsonString);

                        if (AstronautInstance.User.id == 1)
                        {
                            // EventBus.Publish(new IMUChanged(AstronautInstance.User.imu.imu.eva1));
                            // EventBus.Publish(new FellowIMUChanged(AstronautInstance.User.imu.imu.eva2));
                        }
                        else
                        {
                            // EventBus.Publish(new IMUChanged(AstronautInstance.User.imu.imu.eva2));
                            // EventBus.Publish(new FellowIMUChanged(AstronautInstance.User.imu.imu.eva1));
                        }
                    }
                    break;
            }

        }
    }
}
