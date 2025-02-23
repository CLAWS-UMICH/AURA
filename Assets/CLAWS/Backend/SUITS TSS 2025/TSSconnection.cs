using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using TMPro;

public class TSScConnection : MonoBehaviour
{
    [SerializeField] string connectionURL = "";
    bool autoConnect = false;
    // Connection
    string host;
    string port;
    string url;
    int    team_number;
    bool   connected;
    float  time_since_last_update;

    // Database Jsons
    bool UIAUpdated;
    string UIAJsonString;
    bool DCUUpdated;
    string DCUJsonString;
    bool ROVERUpdated;
    string ROVERJsonString;
    bool SPECUpdated;
    string SPECJsonString;
    bool TELEMETRYUpdated;
    string TELEMETRYJsonString;
    bool COMMUpdated;
    string COMMJsonString;
    bool IMUUpdated;
    string IMUJsonString;

    // Connect to TSSc with a specific team number
    public void ConnectToHost(string host, int team_number)
    {
        this.host = host;
        this.port = "14141";
        this.team_number = team_number;
        this.url = "http://" + this.host + ":" + this.port;

        // Test Connection
        StartCoroutine(GetRequest(this.url));
    }

    IEnumerator _LookForConnection()
    {
        while (true)
        {
            if (!this.connected && connectionURL.Length > 0 && !connectionURL.Contains("/"))
            {
                ConnectToHost(connectionURL, 2);
            }
            yield return new WaitForSeconds(5);
        }
    }

    public void DisconnectFromHost()
    {
        this.connected = false;
    }

    public void TSSConnect(string url)
    {
        connectionURL = url;
        StartCoroutine(_LookForConnection());
    }

    // This Function is called when the program begins

    void Start()
    {

        //this.connected = false;
        //StartCoroutine(_LookForConnection());
    }

    // This Function is called each render frame
    void Update()
    {
        // If you are connected to TSSc
        if (this.connected)
        {
            // Each Second
            time_since_last_update += Time.deltaTime;
            if (time_since_last_update > 1.0f)
            {
                // Pull TSSc Updates
                StartCoroutine(GetUIAState());
                StartCoroutine(GetDCUState()); 
                StartCoroutine(GetROVERState());
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

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    this.connected = true;
                    break;
            }

        }
    }

    ///////////////////////////////////////////// UIA

    IEnumerator GetUIAState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/UIA.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.UIAJsonString != webRequest.downloadHandler.text)
                    {
                        this.UIAUpdated = true;
                        this.UIAJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.uia = JsonUtility.FromJson<UIA>(this.UIAJsonString);

                        EventBus.Publish(new UIAChanged(AstronautInstance.User.uia.uia));
                    }
                    break;
            }

        }
    }

    public string GetUIAJsonString()
    {
        UIAUpdated = false;
        return this.UIAJsonString;
    }

    public bool isUIAUpdated()
    {
        return UIAUpdated;
    }

    ///////////////////////////////////////////// DCU

    IEnumerator GetDCUState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/DCU.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.DCUJsonString != webRequest.downloadHandler.text)
                    {
                        this.DCUUpdated = true;
                        this.DCUJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.dcu = JsonUtility.FromJson<DCU>(this.DCUJsonString);

                        if (AstronautInstance.User.id == 0)
                        {
                            EventBus.Publish(new DCUChanged(AstronautInstance.User.dcu.dcu.eva1));
                        }
                        else
                        {
                            EventBus.Publish(new DCUChanged(AstronautInstance.User.dcu.dcu.eva2));
                        }
                    }
                    break;
            }

        }
    }

    public string GetDCUJsonString()
    {
        DCUUpdated = false;
        return this.DCUJsonString;
    }

    public bool isDCUUpdated()
    {
        return DCUUpdated;
    }

    ///////////////////////////////////////////// ROVER

    IEnumerator GetROVERState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/ROVER.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.ROVERJsonString != webRequest.downloadHandler.text)
                    {
                        this.ROVERUpdated = true;
                        this.ROVERJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.rover = JsonUtility.FromJson<ROVER>(this.ROVERJsonString);
                        EventBus.Publish(new RoverChanged(AstronautInstance.User.rover.rover));
                    }
                    break;
            }

        }
    }

    public string GetROVERJsonString()
    {
        ROVERUpdated = false;
        return this.ROVERJsonString;
    }

    public bool isROVERUpdated()
    {
        return ROVERUpdated;
    }

    ///////////////////////////////////////////// SPEC

    IEnumerator GetSPECState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/SPEC.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.SPECJsonString != webRequest.downloadHandler.text)
                    {
                        this.SPECUpdated = true;
                        this.SPECJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.spec = JsonUtility.FromJson<SPEC>(this.SPECJsonString);
                        if (AstronautInstance.User.id == 0)
                        {
                            EventBus.Publish<XRFScanEvent>(new XRFScanEvent(AstronautInstance.User.spec.spec.eva1.data));
                        } else
                        {
                            EventBus.Publish<XRFScanEvent>(new XRFScanEvent(AstronautInstance.User.spec.spec.eva2.data));
                        }
                            
                    }
                    break;
            }

        }
    }

    public string GetSPECJsonString()
    {
        SPECUpdated = false;
        return this.SPECJsonString;
    }

    public bool isSPECUpdated()
    {
        return SPECUpdated;
    }

    ///////////////////////////////////////////// TELEMETRY

    IEnumerator GetTELEMETRYState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/teams/" + this.team_number + "/TELEMETRY.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.TELEMETRYJsonString != webRequest.downloadHandler.text)
                    {
                        this.TELEMETRYUpdated = true;
                        this.TELEMETRYJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.telemetry = JsonUtility.FromJson<TELEMETRY>(this.TELEMETRYJsonString);
                        if (AstronautInstance.User.id == 0)
                        {
                            CopyVitals(AstronautInstance.User.VitalsData, AstronautInstance.User.telemetry.telemetry.eva1);
                            CopyVitals(AstronautInstance.User.FellowAstronautsData.vitals, AstronautInstance.User.telemetry.telemetry.eva2);
                        } else
                        {
                            CopyVitals(AstronautInstance.User.VitalsData, AstronautInstance.User.telemetry.telemetry.eva2);
                            CopyVitals(AstronautInstance.User.FellowAstronautsData.vitals, AstronautInstance.User.telemetry.telemetry.eva1);
                        }

                        AstronautInstance.User.VitalsData.eva_time = AstronautInstance.User.telemetry.telemetry.eva_time;

                        EventBus.Publish<VitalsUpdatedEvent>(new VitalsUpdatedEvent(AstronautInstance.User.VitalsData));
                        EventBus.Publish<FellowAstronautVitalsDataChangeEvent>(new FellowAstronautVitalsDataChangeEvent(AstronautInstance.User.FellowAstronautsData.vitals));
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

    public string GetTELEMETRYJsonString()
    {
        TELEMETRYUpdated = false;
        return this.TELEMETRYJsonString;
    }

    public bool isTELEMETRYUpdated()
    {
        return TELEMETRYUpdated;
    }

    ///////////////////////////////////////////// COMM

    IEnumerator GetCOMMState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/COMM.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.COMMJsonString != webRequest.downloadHandler.text)
                    {
                        this.COMMUpdated = true;
                        this.COMMJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.comm = JsonUtility.FromJson<COMM>(this.COMMJsonString);

                        EventBus.Publish(new CommChanged(AstronautInstance.User.comm.comm));
                    }
                    break;
            }

        }
    }

    public string GetCOMMJsonString()
    {
        COMMUpdated = false;
        return this.COMMJsonString;
    }

    public bool isCOMMUpdated()
    {
        return COMMUpdated;
    }

    ///////////////////////////////////////////// IMU

    IEnumerator GetIMUState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(this.url + "/json_data/IMU.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (this.IMUJsonString != webRequest.downloadHandler.text)
                    {
                        this.IMUUpdated = true;
                        this.IMUJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.imu = JsonUtility.FromJson<IMU>(this.IMUJsonString);

                        if (AstronautInstance.User.id == 0)
                        {
                            EventBus.Publish(new IMUChanged(AstronautInstance.User.imu.imu.eva1));
                            EventBus.Publish(new FellowIMUChanged(AstronautInstance.User.imu.imu.eva2));
                        }
                        else
                        {
                            EventBus.Publish(new IMUChanged(AstronautInstance.User.imu.imu.eva2));
                            EventBus.Publish(new FellowIMUChanged(AstronautInstance.User.imu.imu.eva1));
                        }
                    }
                    break;
            }

        }
    }

    public string GetIMUJsonString()
    {
        IMUUpdated = false;
        return this.IMUJsonString;
    }

    public bool isIMUUpdated()
    {
        return IMUUpdated;
    }




}
