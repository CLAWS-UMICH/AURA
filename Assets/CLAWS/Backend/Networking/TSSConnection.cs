using System.Collections;
using UnityEngine; 
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using TMPro;
using System.IO;

public class TSSConnection : MonoBehaviour
{
    [SerializeField] private GameObject minimap;
    private string IPaddr;
    int team_number;
    bool connected;
    float time_since_last_update;
    public Action<bool> OnTSSConnectionResult;
    private bool imuInitialized = false;


    // Database Jsons
    string UIAJsonString;
    string DCUJsonString;
    string ErrorJsonString;
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
        Debug.Log("Connecting to TSS at: " + AstronautInstance.User.TSSurl);
        // Test connection to frontend
        StartCoroutine(GetRequest(AstronautInstance.User.TSSurl));
    }

    public void LookForConnection()
    {
        if (!connected && IPaddr.Length > 0 && !IPaddr.Contains("/"))
        {
            ConnectToHost(IPaddr, 0); // CHANGE 0 TO ACTUAL TEAM NUMBER IN HOUSTON
        }
    }


    // called from main connection
    public void TSSConnect(string ip)
    {
        IPaddr = ip;
        Debug.Log("IPAddr: " + IPaddr);
        LookForConnection();
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
        if (connected)
        {
            // Each Second
            time_since_last_update += Time.deltaTime;
            if (time_since_last_update > 1.0f)
            {
                // Pull TSS Updates
                StartCoroutine(GetDCUState());
                StartCoroutine(GetDCUError());
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
    IEnumerator GetUIAState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(AstronautInstance.User.TSSurl + "/json_data/UIA.json"))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (UIAJsonString != webRequest.downloadHandler.text)
                    {
                        UIAJsonString = webRequest.downloadHandler.text;

                        AstronautInstance.User.uia = JsonUtility.FromJson<UIA>(UIAJsonString);

                        EventBus.Publish(new UIAUpdatedEvent(AstronautInstance.User.uia));
                    }
                    break;
            }

        }
    }



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
                        Debug.Log("DCU STATE" + DCUJsonString);
                        EventBus.Publish(new DCUChangedEvent(AstronautInstance.User.dcu.dcu.eva1));
                        EventBus.Publish(new FellowDCUChangedEvent(AstronautInstance.User.dcu.dcu.eva2));
                    }
                    break;
            }

        }
    }

    IEnumerator GetDCUError()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(AstronautInstance.User.TSSurl + "/json_data/ERROR.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (ErrorJsonString != webRequest.downloadHandler.text)
                    {
                        ErrorJsonString = webRequest.downloadHandler.text;
                        ErrorMsg e;
                        e = JsonUtility.FromJson<ErrorMsg>(ErrorJsonString);
                        EventBus.Publish(new DCUErrorEvent(e));
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
                            EventBus.Publish<XRFScanEvent>(new XRFScanEvent(AstronautInstance.User.spec.spec.eva1.data));
                        } 
                        else
                        {
                            EventBus.Publish<XRFScanEvent>(new XRFScanEvent(AstronautInstance.User.spec.spec.eva2.data));
                        }
                            
                    }
                    break;
            }

        }
    }


    IEnumerator GetROVERState()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(AstronautInstance.User.TSSurl + "/json_data/ROVER.json"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    if (ROVERJsonString != webRequest.downloadHandler.text)
                    {
                        ROVERJsonString = webRequest.downloadHandler.text;
                        AstronautInstance.User.rover = JsonUtility.FromJson<ROVER>(ROVERJsonString);
                        EventBus.Publish(new RoverUpdatedEvent(AstronautInstance.User.rover.rover));
                    }
                    break;
            }

        }
    }


    ////////////////////////////  EVA VITALS /////////////////////////////
    IEnumerator GetTELEMETRYState()
    {
        // Debug.Log(AstronautInstance.User.TSSurl + "/json_data/teams/" + this.team_number + "/TELEMETRY.json");
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
                        Debug.Log("Telemetry" + TELEMETRYJsonString);

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
                        EventBus.Publish<UpdatedVitalsEvent>(new UpdatedVitalsEvent(AstronautInstance.User.vitals));
                        EventBus.Publish<UpdatedFellowAstronautVitalsEvent>(new UpdatedFellowAstronautVitalsEvent(AstronautInstance.User.fellowAstronaut.vitals));
                    }
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("Connection error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Data processing error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP error: " + webRequest.error);
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

                        // Parse the IMU data
                        AstronautInstance.User.imu = JsonUtility.FromJson<IMU>(this.IMUJsonString);
                        float EV1_posX = (float)AstronautInstance.User.imu.imu.eva1.posx;
                        float EV1_posY = (float)AstronautInstance.User.imu.imu.eva1.posy;
                        float EV2_posX = (float)AstronautInstance.User.imu.imu.eva2.posx;
                        float EV2_posY = (float)AstronautInstance.User.imu.imu.eva2.posy;
                        // Check if this is the first IMU update -- should only enter condition once
                        if (!imuInitialized)
                        {
                            // If the IMU data is not (0, 0), initialize the minimap
                            if (EV1_posX != 0 || EV1_posY != 0)
                            {
                                imuInitialized = true;
                                if (AstronautInstance.User.id == 1)
                                {
                                    //AstronautInstance.User.origin.posX = EV1_posX;
                                    //AstronautInstance.User.origin.posY = EV1_posY;
                                }
                                else
                                {
                                    //AstronautInstance.User.origin.posX = EV2_posX;
                                    //AstronautInstance.User.origin.posY = EV2_posY;
                                }
                            }
                        }
                        if (AstronautInstance.User.id == 1)
                        {
                            Debug.Log("EV1: " + EV1_posX + " " + EV1_posY);
                            Debug.Log("EV2 " + EV2_posX + " " + EV2_posY);
                            Location newLocation = new Location(EV1_posX - AstronautInstance.User.origin.posX, 0, EV1_posY - AstronautInstance.User.origin.posZ, AstronautInstance.User.imu.imu.eva1.heading);
                            AstronautInstance.User.current = newLocation;
                            Location newEV2Location = new Location(EV2_posX - AstronautInstance.User.origin.posX, 0, EV2_posY - AstronautInstance.User.origin.posZ, AstronautInstance.User.imu.imu.eva2.heading);
                            EventBus.Publish(new EV2_LocationUpdatedEvent(newEV2Location));
                        }
                        else {
                            
                            Location newLocation = new Location(EV2_posX - AstronautInstance.User.origin.posX, 0, EV2_posY - AstronautInstance.User.origin.posZ, AstronautInstance.User.imu.imu.eva2.heading);
                            AstronautInstance.User.current = newLocation;
                            Location newEV2Location = new Location(EV1_posX - AstronautInstance.User.origin.posX, 0, EV1_posY - AstronautInstance.User.origin.posZ, AstronautInstance.User.imu.imu.eva1.heading);
                            EventBus.Publish(new EV2_LocationUpdatedEvent(newEV2Location));
                        }
                    }
                    break;
            }
        }
    }
}
