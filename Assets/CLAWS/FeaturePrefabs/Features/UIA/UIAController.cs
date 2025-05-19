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
    public GameObject stepsScreen;
    public TextMeshPro stepNumber;
    public TextMeshPro stepText;
    public GameObject DCUPanel;
    public List<string> EgressSteps = new List<string>();
    public List<string> IngressSteps = new List<string>();
    private bool uiaUpdatedReceived = false;
    private bool nextStep = false;
    private bool value = false;


    void Start()
    {
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
    }

    public void openFeatureScreen()
    {
        uiaScreen.SetActive(true);
        foreach (Transform child in uiaScreen.transform)
        {
            child.gameObject.SetActive(true);
        }
        main.SetActive(false);
        loadingBars.SetActive(false);
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


    // public void EgressProcedure()
    // {
    //     initializationScreen.SetActive(true);
    //     loadingBars.SetActive(true);
    //     procedureScreen.SetActive(false);
    //     StartCoroutine(EgressProcedureCoroutine());
    // }

    // private IEnumerator EgressProcedureCoroutine()
    // {
    //     initializationScreen.SetActive(false);
    //     procedureScreen.SetActive(true);
    //     stepsScreen.SetActive(true);
    //     stepsScreen.transform.Find("StepNumber").gameObject.SetActive(true);
    //     stepsScreen.transform.Find("StepText").gameObject.SetActive(true);

    //     for (int i = 0; i < EgressSteps.Count; i++)
    //     {
    //         if (i == 0)
    //         {
    //             stepNumber.text = (i + 1).ToString();
    //             stepText.text = EgressSteps[i];
    //             yield return new WaitForSeconds(10f);
    //             break;
    //         }
    //         else if (i == 1 || i == 2)
    //         {
    //             if (AstronautInstance.User.id == 1)
    //             {
    //                 stepNumber.text = (1).ToString();
    //                 stepText.text = EgressSteps[1];
    //                 value = true;
    //                 // falls through to json to PR
    //             }
    //             else
    //             {
    //                 stepNumber.text = (1).ToString();
    //                 stepText.text = EgressSteps[2];
    //                 value = true;
    //                 // falls through to json to PR
    //             }
    //         }
    //         // DCU - SHOW TO EV1 AND EV2
    //         else if (i == 3)
    //         {
    //             DCUPanel.SetActive(true);
    //             stepNumber.text = (i + 1).ToString();
    //             stepText.text = EgressSteps[i];
    //             yield return new WaitUntil(() => AstronautInstance.User.dcu.dcu.eva1.batt == true);
    //             break;
    //         }
    //         else if (i == 4)
    //         {
    //             stepNumber.text = (1).ToString();
    //             stepText.text = EgressSteps[2];
    //             // falls through to json to PR
    //         }
    //         else if (i == 5)
    //         {

    //         }
    //         else if (i == 5)
    //         {


    //         }




    //         var jsonData = new Dictionary<string, object>
    //         {
    //             { "id", AstronautInstance.User.id },
    //             { "step", i + 1 },
    //             { "value", value },
    //             { "confirm", false}
    //         };
    //         nextStep = false;
    //         yield return StartCoroutine(SendToPRAndWaitForUIAUpdate(jsonData));
    //         yield return new WaitUntil(() => nextStep);
    //     }
    //     Debug.Log("Egress procedure complete!");
    // }

    

    // private IEnumerator SendToPRAndWaitForUIAUpdate(Dictionary<string, object> jsonData)
    // {
    //     uiaUpdatedReceived = false;
    //     EventBus.Subscribe<UIAUpdatedEvent>(OnUIAUpdated);
    //     LMCCWebSocketClient webSocketClient = FindObjectOfType<LMCCWebSocketClient>();
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
    //     EventBus.Unsubscribe<UIAUpdatedEvent>(OnUIAUpdated);
    //     Debug.Log("UIA update received, continuing to next step...");
    //     nextStep = true;
    // }

    // private void OnUIAUpdated(UIAUpdatedEvent e)
    // {
    //     uiaUpdatedReceived = true;
    // }
}
