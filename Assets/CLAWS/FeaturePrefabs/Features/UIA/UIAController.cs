using System.Collections;
using System.Collections.Generic;
using GLTFast;
using TMPro;
using UnityEngine;

public class UIAController : MonoBehaviour
{
    public GameObject uiaScreen;
    public GameObject main;
    public GameObject loadingBars;
    public GameObject initializationScreen;
    public GameObject procedureScreen;
    public LMCCWebSocketClient webSocketClient;
    public GameObject stepsScreen;
    public TextMeshPro stepNumber;
    public TextMeshPro stepText;
    public GameObject DCUPanel;
    public GameObject primaryO2;
    public GameObject secondaryO2;
    public List<string> EgressSteps = new List<string>();
    public List<string> IngressSteps = new List<string>();
    private bool uiaUpdatedReceived = false;
    private bool nextStep = false;
    private bool value = false;
    public event System.Action OnUIAOpened;

    [SerializeField] private GameObject NaViPrefab; //s1mple the goat
    [SerializeField] private GameObject SamplingPrefab;

    [SerializeField] private NavigationFrontend NavControl;
    [SerializeField] private GeoSampleFrontend GeoControl;

    private float geoZone1X = -5635f;
    private float geoZone1Z = -9970f;

    private float geoZone2X = -5610f;
    private float geoZone2Z = -9971f;

    private float geoZone3X = -5615f;
    private float geoZone3Z = -9995f;

    private int zone2go = 1;

    private bool flipflop = false; //boolean that flip flops constantly and causes geosampling to end when a sample is saved

    /*
     Rundown of the system so far:
     UIA button clicked in menu -> egress begins immediately, triggered by its coroutine in a big coroutine called "fullsequence" which is supposed
     to hold all the other coroutines in the order they're supposed to be executed
     that means last egress task leads into...
    
     navigation popup -> position monitored and compared to hardcoded geosampling positions every 1 second
     geosampling position reached -> geosampling popup immediately
     geosampling popup accepted -> geosampling procedure begins. continues until sample is collected and "save" button clicked, triggering SaveCheck(). You
     can see this bad boy hooked up to the actual save button in the unity inspector alongside its main purpose

     cycle will repeat from nav popup to bad boy 3 times total before the coroutine ends, and ingress procedure should start


     NOTE:
     The "alert" prefabs have a bug in them somewhere I think. Everything compiles and runs fine, but something about them cause a flood of errors for as
     long as they're on screen. Works OK though.

     Tested nav -> geosample transition by forcing astronaut position into zone and it worked well there. Force triggered nav by making a button that launched
     testingPurposesOnly() and the mid coroutine.
     */

    void Start()
    {

        NaViPrefab.SetActive(false);
        SamplingPrefab.SetActive(false);

        // Initialize EGRESS steps
        EgressSteps.Add("Connect the umbilical cord from the DCU to the UIA Panel");

        // 1 - 2 
        EgressSteps.Add("Switch EV-1 EMU Power to ON");
        EgressSteps.Add("Switch EV-2 EMU Power to ON");

        EgressSteps.Add("On the DCU Panel to the left, switch BATTERY from LOCAL to UMBILICAL Power");
        EgressSteps.Add("Switch the DEPRESS PUMP Power to ON");
        EgressSteps.Add("OPEN your OXYGEN VENT and wait for the PRIMARY and SECONDARY OXYGEN tanks to be under 10psi");
        EgressSteps.Add("CLOSE your OXYGEN VENT");
        EgressSteps.Add("On the DCU Panel to the left, switch OXYGEN to PRIMARY");

        // 8 - 9
        EgressSteps.Add("Switch the EMU-1 OXYGEN to OPEN, and wait for the PRIMARY O2 tank to be above 3000psi");
        EgressSteps.Add("Switch the EMU-2 OXYGEN to OPEN, and wait for the PRIMARY O2 tank to be above 3000psi");
        // 10 - 11
        EgressSteps.Add("Switch the EMU-1 OXYGEN to CLOSE");
        EgressSteps.Add("Switch the EMU-2 OXYGEN to CLOSE");

        EgressSteps.Add("On the DCU Panel to the left, switch OXYGEN to SECONDARY");

        // 13 - 14
        EgressSteps.Add("Switch the EMU-1 OXYGEN to OPEN, and wait for the SECONDARY O2 tank to be above 3000psi");
        EgressSteps.Add("Switch the EMU-2 OXYGEN to OPEN, and wait for the SECONDARY O2 tank to be above 3000psi");
        // 15 - 16
        EgressSteps.Add("Switch the EMU-1 OXYGEN to CLOSE");
        EgressSteps.Add("Switch the EMU-2 OXYGEN to CLOSE");

        EgressSteps.Add("On the DCU Panel to the left, switch OXYGEN to PRIMARY");
        EgressSteps.Add("Wait until your SUIT PRESSURE and OXYGEN PRESSURE equal 4psi");
        EgressSteps.Add("Switch the DEPRESS PUMP Power to OFF");
        EgressSteps.Add("On the DCU Panel to the left, switch BATTERY from UMBILICAL to LOCAL Power");

        // 21 - 22
        EgressSteps.Add("Switch EV-1 EMU Power to OFF");
        EgressSteps.Add("Switch EV-2 EMU Power to OFF");

        EgressSteps.Add("On the DCU Panel to the left, verify that OXYGEN is set to PRIMARY");
        EgressSteps.Add("On the DCU Panel to the left, verify that COMMS are set to A");
        EgressSteps.Add("On the DCU Panel to the left, verify that FAN is set to PRIMARY");
        EgressSteps.Add("On the DCU Panel to the left, verify that PUMP is CLOSED");
        EgressSteps.Add("On the DCU Panel to the left, verify that CO2 is set to A");
        EgressSteps.Add("Disconnect the umbilical cord from the DCU and UIA Panel");


        // // Initialize INGRESS steps
        IngressSteps.Add("Connect the umbilical cord from the DCU to the UIA Panel");

        // 1 - 2
        IngressSteps.Add("Switch EV-1 EMU Power to ON");
        IngressSteps.Add("Switch EV-2 EMU Power to ON");

        IngressSteps.Add("On the DCU Panel to the left, switch BATTERY from LOCAL to UMBILICAL Power");
        IngressSteps.Add("OPEN your OXYGEN VENT and wait for the PRIMARY and SECONDARY OXYGEN tanks to be under 10psi");
        IngressSteps.Add("CLOSE your OXYGEN VENT");
        IngressSteps.Add("On the DCU Panel to the left, switch your PUMP to OPEN");

        // 7 - 8
        IngressSteps.Add("OPEN your EV-1 WASTE WATER and wait for the EV-1 coolant tank to be UNDER 5%");
        IngressSteps.Add("OPEN your EV-2 WASTE WATER and wait for the EV-2 coolant tank to be UNDER 5%");
        // 9 - 10
        IngressSteps.Add("CLOSE your EV-1 WASTE WATER");
        IngressSteps.Add("CLOSE your EV-2 WASTE WATER");
        // 11 - 12
        IngressSteps.Add("Switch EV-1 EMU Power to OFF");
        IngressSteps.Add("Switch EV-2 EMU Power to OFF");

        IngressSteps.Add("Disconnect the umbilical cord from the DCU and UIA Panel");
        OnUIAOpened += HandleUIAOpened;
    }

    IEnumerator midMidMidCoroutine() //"coroutine in the middle of everything" between egress ingress
    {
        for (int i = 0; i < 2; i++)
        {
            NaViPrefab.SetActive(true);
            // this will pause here until SecPosCheck() is done:
            Debug.Log("Navigation popup up.");
            yield return StartCoroutine(SecPosCheck());
            Debug.Log("Navigation SecPosCheck execution OK.");

            SamplingPrefab.SetActive(true);
            Debug.Log("Sampling popup up.");
            // and then pause here until WaitForSave() is done:
            yield return StartCoroutine(WaitForSave());
            Debug.Log("Sampling WaitForSave execution OK.");
        }
    }

    IEnumerator WaitForSave() //"wait for save button to be pressed"
    {
        yield return new WaitUntil(() => flipflop);

        // then reset and continue
        flipflop = false;
        Debug.Log("flipflop flipped and flopped. flip flop goldfish OK.");
        GeoControl.closeGeoSampleFeature();

    }

    public void saveCheck() //atached to geosampling's save button as the "end" to the sampling process. saving ends the geosample procedure for this
    {
        flipflop = true;
        Debug.Log("flipflop flipped.");
    }

    IEnumerator SecPosCheck() //every second, position check
    {
        Debug.Log("Astronaut position being checked every 1 second starting now.");
        while (!PosCheck()) //constant 1 second interval position checks when navigation is up
        {
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Now no longer checking position.");
    }

    private bool PosCheck() //the actual check
    {
        if (zone2go == 1 && (AstronautInstance.User.origin.posX < geoZone1X + 5 && AstronautInstance.User.origin.posX > geoZone1X - 5) && (AstronautInstance.User.origin.posZ < geoZone1Z + 5 && AstronautInstance.User.origin.posZ > geoZone1Z - 5))
        {
            zone2go++;
            NavControl.closeScreens();
            return true; //position checks out and ends navigation when the user is within bounds of a geosampling area
            
        } 
        else if (zone2go == 2 && (AstronautInstance.User.origin.posX < geoZone2X + 5 && AstronautInstance.User.origin.posX > geoZone2X - 5) && (AstronautInstance.User.origin.posZ < geoZone2Z + 5 && AstronautInstance.User.origin.posZ > geoZone2Z - 5)) 
        {
            zone2go++;
            NavControl.closeScreens();
            return true;

        }
        else if (zone2go == 3 && (AstronautInstance.User.origin.posX < geoZone3X + 5 && AstronautInstance.User.origin.posX > geoZone3X - 5) && (AstronautInstance.User.origin.posZ < geoZone3Z + 5 && AstronautInstance.User.origin.posZ > geoZone3Z - 5))
        {
            zone2go++;
            NavControl.closeScreens();
            return true; 
        }

        return false;
    }

    public void onSamplingClick() //when OK is pressed on the geosampling alert
    {
        SamplingPrefab.SetActive(false);
        Debug.Log("geosampling alert clicked");
        GeoControl.openFeatureScreen();
    }

    public void onNaViClick() //when OK is pressed on the navigation alert
    {
        NaViPrefab.SetActive(false);
        Debug.Log("navigation alert clicked");
        NavControl.openFeatureScreen();
    }

    public void HandleUIAOpened()
    {
        Debug.Log("UIA screen opened");
        EgressProcedure();
    }

    public void openFeatureScreen()
    {
        uiaScreen.SetActive(true);
        foreach (Transform child in uiaScreen.transform)
        {
            if (child.gameObject == NaViPrefab || child.gameObject == SamplingPrefab)
            {
                continue; //STOPS ALERT PREFABS FROM DISAPPEARING!!! THIS BYPASS IS IMPORTANT!!
            }
            child.gameObject.SetActive(true);
        }
        main.SetActive(false);
        loadingBars.SetActive(false);
        OnUIAOpened?.Invoke();
    }

    public void closeFeatureScreen()
    {
        uiaScreen.SetActive(false);
        foreach (Transform child in uiaScreen.transform)
        {
            if (child.gameObject == NaViPrefab || child.gameObject == SamplingPrefab)
            {
                continue;
            }
            child.gameObject.SetActive(false);
        }
        main.SetActive(true);
    }


    public void EgressProcedure()
    {
        initializationScreen.SetActive(true);
        loadingBars.SetActive(true);
        procedureScreen.SetActive(false);
        StartCoroutine(FullSequence());
    }

    private IEnumerator FullSequence() 
    {
        yield return StartCoroutine(EgressProcedureCoroutine());
        yield return StartCoroutine(midMidMidCoroutine());

        //then probably ingress here
    }

    /*public void testingPurposesOnly()
    {
        StartCoroutine(midMidMidCoroutine());
    }*/

    private IEnumerator EgressProcedureCoroutine()
    {
        Debug.Log("Egress Procedure started");
        yield return new WaitForSeconds(3f);
        initializationScreen.SetActive(false);
        procedureScreen.SetActive(true);
        loadingBars.SetActive(false);
        stepsScreen.SetActive(true);
        stepsScreen.transform.Find("NumText").gameObject.SetActive(true);
        stepsScreen.transform.Find("StepText").gameObject.SetActive(true);

        for (int i = 0; i < EgressSteps.Count; i++)
        {
            Debug.Log("STEP INDEX: " + i);
            if (i == 0) // 0 connect umbilical
            {

                loadingBars.SetActive(false);
                procedureScreen.SetActive(true);
                stepNumber.text = "";
                stepText.text = EgressSteps[0];
                Debug.Log("Step index: " + i + " Step text: " + EgressSteps[0] + " Value: " + value);
                yield return new WaitForSeconds(10f);
                procedureScreen.SetActive(true);
                stepsScreen.SetActive(false);
                loadingBars.SetActive(true);
                continue;
            }
            else if (i == 1 || i == 2) // 1 switch EV1 emu power 2 switch ev2 emu power
            {
                Debug.Log("stepped");
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepsScreen.SetActive(true);
                    stepNumber.text = (1).ToString();
                    stepText.text = EgressSteps[1];
                    value = true;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[1] + " Value: " + value);
                    procedureScreen.SetActive(false);
                    loadingBars.SetActive(true);
                    // falls through to json to PR
                }
                else
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepsScreen.SetActive(true);
                    stepNumber.text = (1).ToString();
                    stepText.text = EgressSteps[2];
                    value = true;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[2] + " Value: " + value);
                    stepsScreen.SetActive(false);
                    loadingBars.SetActive(true);
                    // falls through to json to PR
                }
            }
            // DCU - SHOW TO EV1 AND EV2 EMU POWER
            else if (i == 3)
            {
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepsScreen.SetActive(true);
                    DCUPanel.SetActive(true);
                    stepNumber.text = (2).ToString();
                    stepText.text = EgressSteps[3];
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[3] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.batt == true);
                    DCUPanel.SetActive(false);
                    stepsScreen.SetActive(false);
                    loadingBars.SetActive(true);
                    continue;
                }
                else
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    DCUPanel.SetActive(true);
                    stepNumber.text = (2).ToString();
                    stepText.text = EgressSteps[3];
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[3] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.batt == true);
                    DCUPanel.SetActive(false);
                    stepsScreen.SetActive(false);
                    loadingBars.SetActive(true);
                    continue;
                }
            }
            else if (i == 4) // 3 depress open
            {
                loadingBars.SetActive(false);
                procedureScreen.SetActive(true);
                stepNumber.text = (3).ToString();
                stepText.text = EgressSteps[4];
                value = true;
                Debug.Log("Step index: " + i + " Step text: " + EgressSteps[4] + " Value: " + value);
                procedureScreen.SetActive(false);
                loadingBars.SetActive(true);
                // falls through to json to PR
            }
            else if (i == 5) // 4 open o2 vent UIA
            {
                loadingBars.SetActive(false);
                procedureScreen.SetActive(true);
                stepNumber.text = (4).ToString();
                stepText.text = EgressSteps[5];
                value = true;
                Debug.Log("Step index: " + i + " Step text: " + EgressSteps[5] + " Value: " + value);
                yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_pri_pressure > 10);
                yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_sec_pressure > 10);
                procedureScreen.SetActive(false);
                loadingBars.SetActive(true);
                // falls through to json to PR
            }
            else if (i == 6) // 5 close o2 vent UIA
            {
                loadingBars.SetActive(false);
                procedureScreen.SetActive(true);
                stepNumber.text = (5).ToString();
                stepText.text = EgressSteps[6];
                value = true;
                Debug.Log("Step index: " + i + " Step text: " + EgressSteps[6] + " Value: " + value);
                procedureScreen.SetActive(false);
                loadingBars.SetActive(true);
                // falls through to json to PR
            }
            else if (i == 7) // 6 DCU o2 to primary 
            {
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    DCUPanel.SetActive(true);
                    stepNumber.text = (6).ToString();
                    stepText.text = EgressSteps[7];
                    value = true;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[7] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.oxy == true);
                    DCUPanel.SetActive(false);
                    continue;
                }
                else
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    DCUPanel.SetActive(true);
                    stepNumber.text = (6).ToString();
                    stepText.text = EgressSteps[7];
                    value = true;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[7] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.oxy == true);
                    DCUPanel.SetActive(false);
                    continue;
                }
            }
            else if (i == 8) // 7 open emu1 o2
            {
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (7).ToString();
                    stepText.text = EgressSteps[8];
                    value = true;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[8] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_pri_pressure > 3000);
                    procedureScreen.SetActive(false);
                    loadingBars.SetActive(true);
                    // falls through to json to PR
                }
            }
            else if (i == 9) // 7 open emu2 o2
            {
                if (AstronautInstance.User.id == 2)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (7).ToString();
                    stepText.text = EgressSteps[9];
                    value = true;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[9] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_pri_pressure > 3000);
                    loadingBars.SetActive(true);
                }
            }
            else if (i == 10) // 8 close emu1 o2
            {
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (8).ToString();
                    stepText.text = EgressSteps[10];
                    value = false;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[10] + " Value: " + value);
                    loadingBars.SetActive(true);
                }
            }
            else if (i == 11) // 8 close emu2 o2
            {
                if (AstronautInstance.User.id == 2)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (8).ToString();
                    stepText.text = EgressSteps[11];
                    value = false;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[11] + " Value: " + value);
                    loadingBars.SetActive(true);
                }
            }
            else if (i == 12) // 9 switch to secondary
            {
                loadingBars.SetActive(false);
                procedureScreen.SetActive(true);
                DCUPanel.SetActive(true);
                stepNumber.text = (9).ToString();
                stepText.text = EgressSteps[12];
                value = true;
                Debug.Log("Step index: " + i + " Step text: " + EgressSteps[12] + " Value: " + value);
                yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.oxy == false);
                DCUPanel.SetActive(false);
                continue;
            }
            else if (i == 13) // 10 open emu1 oxy to open
            {
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (10).ToString();
                    stepText.text = EgressSteps[13];
                    value = true;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[13] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_sec_pressure > 3000);
                    procedureScreen.SetActive(false);
                    loadingBars.SetActive(true);
                }
            }
            else if (i == 14) // 10 open emu2 oxy to open
            {
                if (AstronautInstance.User.id == 2)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (10).ToString();
                    stepText.text = EgressSteps[14];
                    value = true;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[14] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.fellowAstronaut.vitals.oxy_sec_pressure > 3000);
                    procedureScreen.SetActive(false);
                    loadingBars.SetActive(true);
                }
            }
            else if (i == 15) // 11 close emu1 oxy to close
            {
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (11).ToString();
                    stepText.text = EgressSteps[15];
                    value = false;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[15] + " Value: " + value);
                    procedureScreen.SetActive(false);
                    loadingBars.SetActive(true);
                    // falls through to json to PR
                }
            }
            else if (i == 16) // 11 close emu2 oxy to close
            {
                if (AstronautInstance.User.id == 2)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (11).ToString();
                    stepText.text = EgressSteps[16];
                    value = false;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[16] + " Value: " + value);
                }
            }
            else if (i == 17) // 12 DCU o2 to primary 
            {
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    DCUPanel.SetActive(true);
                    stepNumber.text = (12).ToString();
                    stepText.text = EgressSteps[17];
                    value = true;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[17] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.oxy == true);
                    DCUPanel.SetActive(false);
                    continue;
                }
                else
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    DCUPanel.SetActive(true);
                    stepNumber.text = (12).ToString();
                    stepText.text = EgressSteps[17];
                    value = true;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[17] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.oxy == true);
                    DCUPanel.SetActive(false);
                    continue;
                }
            }
            else if (i == 18) // 13 wait for suit and oxy pressure
            {
                loadingBars.SetActive(false);
                procedureScreen.SetActive(true);
                stepNumber.text = (13).ToString();
                stepText.text = EgressSteps[18];
                value = true;
                Debug.Log("Step index: " + i + " Step text: " + EgressSteps[18] + " Value: " + value);
                yield return new WaitUntil(() => AstronautInstance.User.vitals.suit_pressure_total > 4);
                yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_pri_pressure > 4);
                procedureScreen.SetActive(false);
                continue;
            }
            else if (i == 19) // 14 depress power to off
            {
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (14).ToString();
                    stepText.text = EgressSteps[19];
                    value = false;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[19] + " Value: " + value);
                    procedureScreen.SetActive(false);
                    loadingBars.SetActive(true);
                    // falls through to json to PR
                }
                else
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (14).ToString();
                    stepText.text = EgressSteps[19];
                    value = false;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[19] + " Value: " + value);
                    procedureScreen.SetActive(false);
                    loadingBars.SetActive(true);
                    // falls through to json to PR
                }
            }
            else if (i == 20) // 15 DCU battery from umb to local power
            {
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    DCUPanel.SetActive(true);
                    stepNumber.text = (15).ToString();
                    stepText.text = EgressSteps[20];
                    value = false;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[20] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.batt == false);
                    DCUPanel.SetActive(false);
                    continue;
                }
                else
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    DCUPanel.SetActive(true);
                    stepNumber.text = (15).ToString();
                    stepText.text = EgressSteps[20];
                    value = false;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[20] + " Value: " + value);
                    yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.batt == false);
                    DCUPanel.SetActive(false);
                    continue;
                }
            }
            else if (i == 21) // 16 ev1 emu power to off
            {
                if (AstronautInstance.User.id == 1)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (15).ToString();
                    stepText.text = EgressSteps[21];
                    value = false;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[21] + " Value: " + value);
                    procedureScreen.SetActive(false);
                    loadingBars.SetActive(true);
                    // falls through to json to PR
                }
            }
            else if (i == 22) // 16 ev2 emu power to off
            {
                if (AstronautInstance.User.id == 2)
                {
                    loadingBars.SetActive(false);
                    procedureScreen.SetActive(true);
                    stepNumber.text = (15).ToString();
                    stepText.text = EgressSteps[21];
                    value = false;
                    Debug.Log("Step index: " + i + " Step text: " + EgressSteps[21] + " Value: " + value);
                    procedureScreen.SetActive(false);
                    loadingBars.SetActive(true);
                    // falls through to json to PR
                }
            }
            else if (i == 23) // 17 DCU verify oxygen == primary;    
            {
                procedureScreen.SetActive(true);
                loadingBars.SetActive(false);
                stepNumber.text = (17).ToString();
                stepText.text = EgressSteps[23];
                yield return new WaitForSeconds(3f);
                if (AstronautInstance.User.id == 1)
                {
                    if (AstronautInstance.User.dcu.dcu.eva1.oxy == true)
                    {
                        yield return new WaitForSeconds(3f);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
                    }
                    else
                    {
                        yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.oxy == true);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
                    }
                }
                else if (AstronautInstance.User.id == 2)
                {
                    if (AstronautInstance.User.dcu.dcu.eva2.oxy == true)
                    {
                        yield return new WaitForSeconds(3f);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
                    }
                    else
                    {
                        yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.oxy == true);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
                    }
                }
                continue;
            }
            else if (i == 24) // 18 DCU verify comms == A
            {
                loadingBars.SetActive(false);
                procedureScreen.SetActive(true);
                stepNumber.text = (18).ToString();
                stepText.text = EgressSteps[24];
                yield return new WaitForSeconds(3f);
                if (AstronautInstance.User.id == 1)
                {
                    if (AstronautInstance.User.dcu.dcu.eva1.comm == true)
                    {
                        yield return new WaitForSeconds(3f);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[24]);
                    }
                    else
                    {
                        yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.comm == true);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[24]);
                    }
                }
                else if (AstronautInstance.User.id == 2)
                {
                    if (AstronautInstance.User.dcu.dcu.eva2.comm == true)
                    {
                        yield return new WaitForSeconds(3f);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[24]);
                    }
                    else
                    {
                        yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.comm == true);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[24]);
                    }
                }
                continue;
            }
            else if (i == 25) // 19 DCU verify fan == primary
            {
                procedureScreen.SetActive(true);
                loadingBars.SetActive(false);
                stepNumber.text = (19).ToString();
                stepText.text = EgressSteps[25];
                yield return new WaitForSeconds(3f);
                if (AstronautInstance.User.id == 1)
                {
                    if (AstronautInstance.User.dcu.dcu.eva1.fan == true)
                    {
                        yield return new WaitForSeconds(3f);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
                    }
                    else
                    {
                        yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.fan == true);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
                    }
                }
                else if (AstronautInstance.User.id == 2)
                {
                    if (AstronautInstance.User.dcu.dcu.eva2.fan == true)
                    {
                        yield return new WaitForSeconds(3f);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
                    }
                    else
                    {
                        yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.fan == true);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
                    }
                }
                continue;
            }
            else if (i == 26) // 20 DCU verify pump == closed
            {
                procedureScreen.SetActive(true);
                loadingBars.SetActive(false);
                stepNumber.text = (20).ToString();
                stepText.text = EgressSteps[26];
                yield return new WaitForSeconds(3f);
                if (AstronautInstance.User.id == 1)
                {
                    if (AstronautInstance.User.dcu.dcu.eva1.pump == false)
                    {
                        yield return new WaitForSeconds(3f);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[26]);
                    }
                    else
                    {
                        yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.pump == false);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[26]);
                    }
                }
                else if (AstronautInstance.User.id == 2)
                {
                    if (AstronautInstance.User.dcu.dcu.eva2.pump == false)
                    {
                        yield return new WaitForSeconds(3f);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[26]);
                    }
                    else
                    {
                        yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.pump == false);
                        Debug.Log("Step index: " + i + " Step text: " + EgressSteps[26]);
                    }
                }
                continue;
            }
            else if (i == 27) // 21 co2 == A
            {
                loadingBars.SetActive(false);
                procedureScreen.SetActive(true);
                stepNumber.text = (21).ToString();
                stepText.text = EgressSteps[27];
                value = true;
                Debug.Log("Step index: " + i + " Step text: " + EgressSteps[27] + " Value: " + value);
                yield return new WaitForSeconds(3f);
                continue;
            }
            else if (i == 28) // 22 disconnect umbilical
            {
                loadingBars.SetActive(false);
                procedureScreen.SetActive(true);
                stepNumber.text = (22).ToString();
                stepText.text = EgressSteps[28];
                continue;
            }
                yield return new WaitForSeconds(3f);
                var jsonData = new Dictionary<string, object>
                {
                    { "id", AstronautInstance.User.id },
                    { "step", i },
                    { "value", value },
                    { "confirm", false }
                };
                nextStep = false;
                yield return StartCoroutine(SendToPRAndWaitForUIAUpdate(jsonData));
                yield return new WaitUntil(() => nextStep);
                Debug.Log("Egress procedure complete!");
            }

    }

    

    private IEnumerator SendToPRAndWaitForUIAUpdate(Dictionary<string, object> jsonData)
    {
        uiaUpdatedReceived = false;
        EventBus.Subscribe<UIAUpdatedEvent>(OnUIAUpdated);
        if (webSocketClient != null)
        {
            webSocketClient.SendJsonData(jsonData, "UIA", 3);
        }
        else
        {
            Debug.LogError("LMCCWebSocketClient not found!");
            yield break;
        }
        yield return new WaitUntil(() => uiaUpdatedReceived);
        //EventBus.Unsubscribe<UIAUpdatedEvent>(OnUIAUpdated);

        //unsubscribing causes red error. failing to unsubscribe causes red error.
        Debug.Log("UIA update received, continuing to next step...");
        nextStep = true;
    }

    private void OnUIAUpdated(UIAUpdatedEvent e)
    {

        uiaUpdatedReceived = true;
    }
}
