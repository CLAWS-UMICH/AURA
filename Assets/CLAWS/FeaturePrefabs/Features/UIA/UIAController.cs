using System.Collections;
using System.Collections.Generic;
using System.Net;
using GLTFast;
using TMPro;
using UnityEngine;

public class UIAController : MonoBehaviour
{
    public GameObject uiaScreen;
    public GameObject main;
    public GameObject EgressInitializationScreen;
    public GameObject IngressInitializationScreen;
    public GameObject procedureScreen;
    public LMCCWebSocketClient webSocketClient;
    public GameObject stepsScreen;
    public TextMeshPro stepNumber;
    public TextMeshPro stepText;
    public GameObject vitals;
    public GameObject previousButton;
    public GameObject nextButton;

    public List<string> EgressSteps = new List<string>();
    public List<string> IngressSteps = new List<string>();
    private bool egressComplete = false;
    private int counter = 0;
    public event System.Action OnUIAOpened;


    void Start()
    {
        // Initialize EGRESS steps
        EgressSteps.Add("Connect the umbilical cord from the DCU to the UIA Panel");

        // 1 - 2 
        EgressSteps.Add("On the UIA panel in front of you, Switch EV-1 EMU Power to ON");
        //EgressSteps.Add("Switch EV-2 EMU Power to ON");

        EgressSteps.Add("On your DCU panel on your chest, switch BATTERY from LOCAL to UMBILICAL Power");
        EgressSteps.Add("On the UIA Panel, Switch the DEPRESS PUMP Power to ON");
        EgressSteps.Add("On the UIA Panel, OPEN your OXYGEN VENT and wait for the PRIMARY and SECONDARY OXYGEN tanks to be under 10psi");
        EgressSteps.Add("On the UIA Panel, CLOSE your OXYGEN VENT");
        EgressSteps.Add("On the DCU Panel, switch OXYGEN to PRIMARY");

        // 8 - 9
        EgressSteps.Add("On the UIA Panel, Switch the EMU-1 OXYGEN to OPEN, and wait for the PRIMARY O2 tank to be above 3000psi");
        // EgressSteps.Add("Switch the EMU-2 OXYGEN to OPEN, and wait for the PRIMARY O2 tank to be above 3000psi");
        // 10 - 11
        EgressSteps.Add("On the UIA Panel, Switch the EMU-1 OXYGEN to CLOSE");
        //EgressSteps.Add("Switch the EMU-2 OXYGEN to CLOSE");

        EgressSteps.Add("On the DCU Panel to the left, switch OXYGEN to SECONDARY");

        // 13 - 14
        EgressSteps.Add("On the UIA Panel, Switch the EMU-1 OXYGEN to OPEN, and wait for the SECONDARY O2 tank to be above 3000psi");
        //EgressSteps.Add("Switch the EMU-2 OXYGEN to OPEN, and wait for the SECONDARY O2 tank to be above 3000psi");
        // 15 - 16
        EgressSteps.Add("On the UIA Panel, Switch the EMU-1 OXYGEN to CLOSE");
        // EgressSteps.Add("Switch the EMU-2 OXYGEN to CLOSE");

        EgressSteps.Add("On the DCU Panel, switch OXYGEN to PRIMARY and wait until your SUIT PRESSURE and OXYGEN PRESSURE equal 4psi");
        EgressSteps.Add("On the UIA Panel, Switch the DEPRESS PUMP Power to OFF");
        EgressSteps.Add("On the DCU Panel, switch BATTERY from UMBILICAL to LOCAL Power");

        // 21 - 22
        EgressSteps.Add("On the UIA Panel, Switch EV-1 EMU Power to OFF");
        // EgressSteps.Add("Switch EV-2 EMU Power to OFF");

        EgressSteps.Add("On the DCU Panel, verify that OXYGEN is set to PRIMARY");
        EgressSteps.Add("On the DCU Panel, verify that COMMS are set to A");
        EgressSteps.Add("On the DCU Pane, verify that FAN is set to PRIMARY");
        EgressSteps.Add("On the DCU Panel, verify that PUMP is CLOSED");
        EgressSteps.Add("On the DCU Panel, verify that CO2 is set to A");
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

    public void HandleUIAOpened()
    {
        Debug.Log("UIA screen opened");
        if (!egressComplete)
        {
            Debug.Log("Egress Procedure started");
            EgressProcedure();
        }
        else
        {
            IngressProcedure();
        }
    }

    public void openFeatureScreen()
    {
        uiaScreen.SetActive(true);
        foreach (Transform child in uiaScreen.transform)
        {
            child.gameObject.SetActive(true);
        }
        main.SetActive(false);
        OnUIAOpened?.Invoke();
    }

    public void closeFeatureScreen()
    {
        uiaScreen.SetActive(false);
        foreach (Transform child in uiaScreen.transform)
        {
            child.gameObject.SetActive(false);
        }
        main.SetActive(true);
    }


    public void EgressProcedure()
    {
        Debug.Log("Egress Procedure started");
        EgressInitializationScreen.SetActive(true);
        IngressInitializationScreen.SetActive(false);
        procedureScreen.SetActive(false);
        StartCoroutine(EgressProcedureCoroutine());
        procedureScreen.SetActive(true);
        stepsScreen.SetActive(true);
    }

    private IEnumerator EgressProcedureCoroutine()
    {
        yield return new WaitForSeconds(5f);
        EgressInitializationScreen.SetActive(false);
    }

    private IEnumerator IngressProcedureCoroutine()
    {
        yield return new WaitForSeconds(5f);
        EgressInitializationScreen.SetActive(false);
    }

    public void IngressProcedure()
    {
        IngressInitializationScreen.SetActive(true);
        EgressInitializationScreen.SetActive(false);
        procedureScreen.SetActive(false);
        StartCoroutine(IngressProcedureCoroutine());
        procedureScreen.SetActive(true);
        stepsScreen.SetActive(true);
    }

    public void PrevStep()
    {
        if (counter > 0)
        {
            counter--;
            counter--;
            if (egressComplete)
            {
                IngressStep();
            }
            else
            {
                EgressStep();
            }
        }
    }

    public void NextStep()
    {
        if (egressComplete)
        {
            IngressStep();
        }
        else
        {
            EgressStep();
        }
    }


    public void EgressStep()
    {
        if (counter < EgressSteps.Count)
        {
            stepNumber.text = (counter + 1).ToString();
            stepText.text = EgressSteps[counter];
            counter++;
        }
        else
        {
            stepNumber.text = "";
            egressComplete = true;
            stepText.text = "All steps complete. Look for the notification to the left to proceed.";
        }
    }

    public void IngressStep()
    {
        if (counter < IngressSteps.Count)
        {
            stepNumber.text = (counter + 1).ToString();
            stepText.text = IngressSteps[counter];
            counter++;
        }
        else
        {
            stepNumber.text = "";
            stepText.text = "All steps complete. Look for the notification to the left to proceed.";
        }
    }

    public void OpenVitals()
    {
        vitals.SetActive(true);
        previousButton.SetActive(false);
        nextButton.SetActive(false);
        foreach (Transform child in vitals.transform)
        {
            child.gameObject.SetActive(true);
        }
        vitals.transform.Find("VitalsSecondAstronaut").gameObject.SetActive(false);
    }

    public void CloseVitals()
    {
        vitals.SetActive(false);
        previousButton.SetActive(true);
        nextButton.SetActive(true);
        foreach (Transform child in vitals.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

        public void openMain()
    {
        main.transform.localPosition = new Vector3(0, 0.121f, 0);

    }

    public void closeMain()
    {
        main.transform.localPosition = new Vector3(0, -0.121f, 0);
    }
}



    //     Debug.Log("Egress Procedure started");
    //     yield return new WaitForSeconds(3f);
    //     initializationScreen.SetActive(false);
    //     procedureScreen.SetActive(true);
    //     loadingBars.SetActive(false);
    //     stepsScreen.SetActive(true);
    //     stepsScreen.transform.Find("NumText").gameObject.SetActive(true);
    //     stepsScreen.transform.Find("StepText").gameObject.SetActive(true);

    //     for (int i = 0; i < EgressSteps.Count; i++)
    //     {
    //         Debug.Log("STEP INDEX: " + i);
    //         if (i == 0) // 0 connect umbilical
    //         {

    //             loadingBars.SetActive(false);
    //             procedureScreen.SetActive(true);
    //             stepNumber.text = "";
    //             stepText.text = EgressSteps[0];
    //             Debug.Log("Step index: " + i + " Step text: " + EgressSteps[0] + " Value: " + value);
    //             yield return new WaitForSeconds(10f);
    //             procedureScreen.SetActive(true);
    //             stepsScreen.SetActive(false);
    //             loadingBars.SetActive(true);
    //             continue;
    //         }
    //         else if (i == 1 || i == 2) // 1 switch EV1 emu power 2 switch ev2 emu power
    //         {
    //             Debug.Log("stepped");
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepsScreen.SetActive(true);
    //                 stepNumber.text = (1).ToString();
    //                 stepText.text = EgressSteps[1];
    //                 value = true;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[1] + " Value: " + value);
    //                 procedureScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //                 // falls through to json to PR
    //             }
    //             else
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepsScreen.SetActive(true);
    //                 stepNumber.text = (1).ToString();
    //                 stepText.text = EgressSteps[2];
    //                 value = true;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[2] + " Value: " + value);
    //                 stepsScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //                 // falls through to json to PR
    //             }
    //         }
    //         // DCU - SHOW TO EV1 AND EV2 EMU POWER
    //         else if (i == 3)
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepsScreen.SetActive(true);
    //                 DCUPanel.SetActive(true);
    //                 stepNumber.text = (2).ToString();
    //                 stepText.text = EgressSteps[3];
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[3] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.batt == true);
    //                 DCUPanel.SetActive(false);
    //                 stepsScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //                 continue;
    //             }
    //             else
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 DCUPanel.SetActive(true);
    //                 stepNumber.text = (2).ToString();
    //                 stepText.text = EgressSteps[3];
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[3] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.batt == true);
    //                 DCUPanel.SetActive(false);
    //                 stepsScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //                 continue;
    //             }
    //         }
    //         else if (i == 4) // 3 depress open
    //         {
    //             loadingBars.SetActive(false);
    //             procedureScreen.SetActive(true);
    //             stepNumber.text = (3).ToString();
    //             stepText.text = EgressSteps[4];
    //             value = true;
    //             Debug.Log("Step index: " + i + " Step text: " + EgressSteps[4] + " Value: " + value);
    //             procedureScreen.SetActive(false);
    //             loadingBars.SetActive(true);
    //             // falls through to json to PR
    //         }
    //         else if (i == 5) // 4 open o2 vent UIA
    //         {
    //             loadingBars.SetActive(false);
    //             procedureScreen.SetActive(true);
    //             stepNumber.text = (4).ToString();
    //             stepText.text = EgressSteps[5];
    //             value = true;
    //             Debug.Log("Step index: " + i + " Step text: " + EgressSteps[5] + " Value: " + value);
    //             yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_pri_pressure > 10);
    //             yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_sec_pressure > 10);
    //             procedureScreen.SetActive(false);
    //             loadingBars.SetActive(true);
    //             // falls through to json to PR
    //         }
    //         else if (i == 6) // 5 close o2 vent UIA
    //         {
    //             loadingBars.SetActive(false);
    //             procedureScreen.SetActive(true);
    //             stepNumber.text = (5).ToString();
    //             stepText.text = EgressSteps[6];
    //             value = true;
    //             Debug.Log("Step index: " + i + " Step text: " + EgressSteps[6] + " Value: " + value);
    //             procedureScreen.SetActive(false);
    //             loadingBars.SetActive(true);
    //             // falls through to json to PR
    //         }
    //         else if (i == 7) // 6 DCU o2 to primary 
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 DCUPanel.SetActive(true);
    //                 stepNumber.text = (6).ToString();
    //                 stepText.text = EgressSteps[7];
    //                 value = true;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[7] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.oxy == true);
    //                 DCUPanel.SetActive(false);
    //                 continue;
    //             }
    //             else
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 DCUPanel.SetActive(true);
    //                 stepNumber.text = (6).ToString();
    //                 stepText.text = EgressSteps[7];
    //                 value = true;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[7] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.oxy == true);
    //                 DCUPanel.SetActive(false);
    //                 continue;
    //             }
    //         }
    //         else if (i == 8) // 7 open emu1 o2
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (7).ToString();
    //                 stepText.text = EgressSteps[8];
    //                 value = true;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[8] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_pri_pressure > 3000);
    //                 procedureScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //                 // falls through to json to PR
    //             }
    //         }
    //         else if (i == 9) // 7 open emu2 o2
    //         {
    //             if (AstronautInstance.User.id == 2)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (7).ToString();
    //                 stepText.text = EgressSteps[9];
    //                 value = true;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[9] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_pri_pressure > 3000);
    //                 loadingBars.SetActive(true);
    //             }
    //         }
    //         else if (i == 10) // 8 close emu1 o2
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (8).ToString();
    //                 stepText.text = EgressSteps[10];
    //                 value = false;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[10] + " Value: " + value);
    //                 loadingBars.SetActive(true);
    //             }
    //         }
    //         else if (i == 11) // 8 close emu2 o2
    //         {
    //             if (AstronautInstance.User.id == 2)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (8).ToString();
    //                 stepText.text = EgressSteps[11];
    //                 value = false;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[11] + " Value: " + value);
    //                 loadingBars.SetActive(true);
    //             }
    //         }
    //         else if (i == 12) // 9 switch to secondary
    //         {
    //             loadingBars.SetActive(false);
    //             procedureScreen.SetActive(true);
    //             DCUPanel.SetActive(true);
    //             stepNumber.text = (9).ToString();
    //             stepText.text = EgressSteps[12];
    //             value = true;
    //             Debug.Log("Step index: " + i + " Step text: " + EgressSteps[12] + " Value: " + value);
    //             yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.oxy == false);
    //             DCUPanel.SetActive(false);
    //             continue;
    //         }
    //         else if (i == 13) // 10 open emu1 oxy to open
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (10).ToString();
    //                 stepText.text = EgressSteps[13];
    //                 value = true;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[13] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_sec_pressure > 3000);
    //                 procedureScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //             }
    //         }
    //         else if (i == 14) // 10 open emu2 oxy to open
    //         {
    //             if (AstronautInstance.User.id == 2)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (10).ToString();
    //                 stepText.text = EgressSteps[14];
    //                 value = true;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[14] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.fellowAstronaut.vitals.oxy_sec_pressure > 3000);
    //                 procedureScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //             }
    //         }
    //         else if (i == 15) // 11 close emu1 oxy to close
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (11).ToString();
    //                 stepText.text = EgressSteps[15];
    //                 value = false;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[15] + " Value: " + value);
    //                 procedureScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //                 // falls through to json to PR
    //             }
    //         }
    //         else if (i == 16) // 11 close emu2 oxy to close
    //         {
    //             if (AstronautInstance.User.id == 2)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (11).ToString();
    //                 stepText.text = EgressSteps[16];
    //                 value = false;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[16] + " Value: " + value);
    //             }
    //         }
    //         else if (i == 17) // 12 DCU o2 to primary 
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 DCUPanel.SetActive(true);
    //                 stepNumber.text = (12).ToString();
    //                 stepText.text = EgressSteps[17];
    //                 value = true;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[17] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.oxy == true);
    //                 DCUPanel.SetActive(false);
    //                 continue;
    //             }
    //             else
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 DCUPanel.SetActive(true);
    //                 stepNumber.text = (12).ToString();
    //                 stepText.text = EgressSteps[17];
    //                 value = true;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[17] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.oxy == true);
    //                 DCUPanel.SetActive(false);
    //                 continue;
    //             }
    //         }
    //         else if (i == 18) // 13 wait for suit and oxy pressure
    //         {
    //             loadingBars.SetActive(false);
    //             procedureScreen.SetActive(true);
    //             stepNumber.text = (13).ToString();
    //             stepText.text = EgressSteps[18];
    //             value = true;
    //             Debug.Log("Step index: " + i + " Step text: " + EgressSteps[18] + " Value: " + value);
    //             yield return new WaitUntil(() => AstronautInstance.User.vitals.suit_pressure_total > 4);
    //             yield return new WaitUntil(() => AstronautInstance.User.vitals.oxy_pri_pressure > 4);
    //             procedureScreen.SetActive(false);
    //             continue;
    //         }
    //         else if (i == 19) // 14 depress power to off
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (14).ToString();
    //                 stepText.text = EgressSteps[19];
    //                 value = false;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[19] + " Value: " + value);
    //                 procedureScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //                 // falls through to json to PR
    //             }
    //             else
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (14).ToString();
    //                 stepText.text = EgressSteps[19];
    //                 value = false;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[19] + " Value: " + value);
    //                 procedureScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //                 // falls through to json to PR
    //             }
    //         }
    //         else if (i == 20) // 15 DCU battery from umb to local power
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 DCUPanel.SetActive(true);
    //                 stepNumber.text = (15).ToString();
    //                 stepText.text = EgressSteps[20];
    //                 value = false;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[20] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.batt == false);
    //                 DCUPanel.SetActive(false);
    //                 continue;
    //             }
    //             else
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 DCUPanel.SetActive(true);
    //                 stepNumber.text = (15).ToString();
    //                 stepText.text = EgressSteps[20];
    //                 value = false;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[20] + " Value: " + value);
    //                 yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.batt == false);
    //                 DCUPanel.SetActive(false);
    //                 continue;
    //             }
    //         }
    //         else if (i == 21) // 16 ev1 emu power to off
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (15).ToString();
    //                 stepText.text = EgressSteps[21];
    //                 value = false;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[21] + " Value: " + value);
    //                 procedureScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //                 // falls through to json to PR
    //             }
    //         }
    //         else if (i == 22) // 16 ev2 emu power to off
    //         {
    //             if (AstronautInstance.User.id == 2)
    //             {
    //                 loadingBars.SetActive(false);
    //                 procedureScreen.SetActive(true);
    //                 stepNumber.text = (15).ToString();
    //                 stepText.text = EgressSteps[21];
    //                 value = false;
    //                 Debug.Log("Step index: " + i + " Step text: " + EgressSteps[21] + " Value: " + value);
    //                 procedureScreen.SetActive(false);
    //                 loadingBars.SetActive(true);
    //                 // falls through to json to PR
    //             }
    //         }
    //         else if (i == 23) // 17 DCU verify oxygen == primary;    
    //         {
    //             procedureScreen.SetActive(true);
    //             loadingBars.SetActive(false);
    //             stepNumber.text = (17).ToString();
    //             stepText.text = EgressSteps[23];
    //             yield return new WaitForSeconds(3f);
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 if (AstronautInstance.User.dcu.dcu.eva1.oxy == true)
    //                 {
    //                     yield return new WaitForSeconds(3f);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
    //                 }
    //                 else
    //                 {
    //                     yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.oxy == true);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
    //                 }
    //             }
    //             else if (AstronautInstance.User.id == 2)
    //             {
    //                 if (AstronautInstance.User.dcu.dcu.eva2.oxy == true)
    //                 {
    //                     yield return new WaitForSeconds(3f);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
    //                 }
    //                 else
    //                 {
    //                     yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.oxy == true);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
    //                 }
    //             }
    //             continue;
    //         }
    //         else if (i == 24) // 18 DCU verify comms == A
    //         {
    //             loadingBars.SetActive(false);
    //             procedureScreen.SetActive(true);
    //             stepNumber.text = (18).ToString();
    //             stepText.text = EgressSteps[24];
    //             yield return new WaitForSeconds(3f);
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 if (AstronautInstance.User.dcu.dcu.eva1.comm == true)
    //                 {
    //                     yield return new WaitForSeconds(3f);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[24]);
    //                 }
    //                 else
    //                 {
    //                     yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.comm == true);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[24]);
    //                 }
    //             }
    //             else if (AstronautInstance.User.id == 2)
    //             {
    //                 if (AstronautInstance.User.dcu.dcu.eva2.comm == true)
    //                 {
    //                     yield return new WaitForSeconds(3f);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[24]);
    //                 }
    //                 else
    //                 {
    //                     yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.comm == true);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[24]);
    //                 }
    //             }
    //             continue;
    //         }
    //         else if (i == 25) // 19 DCU verify fan == primary
    //         {
    //             procedureScreen.SetActive(true);
    //             loadingBars.SetActive(false);
    //             stepNumber.text = (19).ToString();
    //             stepText.text = EgressSteps[25];
    //             yield return new WaitForSeconds(3f);
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 if (AstronautInstance.User.dcu.dcu.eva1.fan == true)
    //                 {
    //                     yield return new WaitForSeconds(3f);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
    //                 }
    //                 else
    //                 {
    //                     yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.fan == true);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
    //                 }
    //             }
    //             else if (AstronautInstance.User.id == 2)
    //             {
    //                 if (AstronautInstance.User.dcu.dcu.eva2.fan == true)
    //                 {
    //                     yield return new WaitForSeconds(3f);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
    //                 }
    //                 else
    //                 {
    //                     yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.fan == true);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[23]);
    //                 }
    //             }
    //             continue;
    //         }
    //         else if (i == 26) // 20 DCU verify pump == closed
    //         {
    //             procedureScreen.SetActive(true);
    //             loadingBars.SetActive(false);
    //             stepNumber.text = (20).ToString();
    //             stepText.text = EgressSteps[26];
    //             yield return new WaitForSeconds(3f);
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 if (AstronautInstance.User.dcu.dcu.eva1.pump == false)
    //                 {
    //                     yield return new WaitForSeconds(3f);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[26]);
    //                 }
    //                 else
    //                 {
    //                     yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.pump == false);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[26]);
    //                 }
    //             }
    //             else if (AstronautInstance.User.id == 2)
    //             {
    //                 if (AstronautInstance.User.dcu.dcu.eva2.pump == false)
    //                 {
    //                     yield return new WaitForSeconds(3f);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[26]);
    //                 }
    //                 else
    //                 {
    //                     yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva2.pump == false);
    //                     Debug.Log("Step index: " + i + " Step text: " + EgressSteps[26]);
    //                 }
    //             }
    //             continue;
    //         }
    //         else if (i == 27) // 21 co2 == A
    //         {
    //             loadingBars.SetActive(false);
    //             procedureScreen.SetActive(true);
    //             stepNumber.text = (21).ToString();
    //             stepText.text = EgressSteps[27];
    //             value = true;
    //             Debug.Log("Step index: " + i + " Step text: " + EgressSteps[27] + " Value: " + value);
    //             yield return new WaitForSeconds(3f);
    //             continue;
    //         }
    //         else if (i == 28) // 22 disconnect umbilical
    //         {
    //             loadingBars.SetActive(false);
    //             procedureScreen.SetActive(true);
    //             stepNumber.text = (22).ToString();
    //             stepText.text = EgressSteps[28];
    //             continue;
    //         }
    //             yield return new WaitForSeconds(3f);
    //             var jsonData = new Dictionary<string, object>
    //             {
    //                 { "id", AstronautInstance.User.id },
    //                 { "step", i },
    //                 { "value", value },
    //                 { "confirm", false }
    //             };
    //             nextStep = false;
    //             yield return StartCoroutine(SendToPRAndWaitForUIAUpdate(jsonData));
    //             yield return new WaitUntil(() => nextStep);
    //             Debug.Log("Egress procedure complete!");
    //         }
    // }



    // private IEnumerator SendToPRAndWaitForUIAUpdate(Dictionary<string, object> jsonData)
    // {
    //     uiaUpdatedReceived = false;
    //     EventBus.Subscribe<UIAUpdatedEvent>(OnUIAUpdated);
    //     if (webSocketClient != null)
    //     {
    //         webSocketClient.SendJsonData(jsonData, "UIA", 3);
    //     }
    //     else
    //     {
    //         Debug.LogError("LMCCWebSocketClient not found!");
    //         yield break;
    //     }
    //     yield return new WaitUntil(() => uiaUpdatedReceived);
    //     //EventBus.Unsubscribe<UIAUpdatedEvent>(OnUIAUpdated);
    //     Debug.Log("UIA update received, continuing to next step...");
    //     nextStep = true;
    // }

    // private void OnUIAUpdated(UIAUpdatedEvent e)
    // {
    //  