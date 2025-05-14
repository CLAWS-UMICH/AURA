using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue_script : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    private int counter = 0;

    private List<string> currentMessages = new List<string>();

    private List<string> egressSteps = new List<string>();
    private List<string> ingressSteps = new List<string>();

    void Start()
    {
        // Initialize EGRESS steps
        egressSteps.Add("Please verify there is an umbilical connection between the UIA and your DCU.");
        egressSteps.Add("Switch EV-1 POWER to ON.");
        egressSteps.Add("Switch your BATTERY to UMBILICAL.");
        egressSteps.Add("Switch your DEPRESS PUMP power to ON.");
        egressSteps.Add("Please OPEN your OXYGEN VENT and wait for confirmation that both your PRIMARY and SECONDARY OXYGEN tanks are under 10psi.");
        egressSteps.Add("Please CLOSE your OXYGEN VENT.");
        egressSteps.Add("Please set your OXYGEN to PRIMARY.");
        egressSteps.Add("Please switch the EMU-1 OXYGEN to OPEN, and wait for confirmation that your PRIMARY O2 tank is above 3000psi.");
        egressSteps.Add("Please switch the EMU-1 OXYGEN to CLOSE.");
        egressSteps.Add("Please set your OXYGEN to SECONDARY.");
        egressSteps.Add("Please switch the EMU-1 OXYGEN to OPEN, and wait for confirmation that your PRIMARY O2 tank is above 3000psi.");
        egressSteps.Add("Please switch the EMU-1 OXYGEN to CLOSE.");
        egressSteps.Add("Please set your OXYGEN to PRIMARY.");
        egressSteps.Add("Please wait until and confirm that your SUIT P and O2 P are 4.");
        egressSteps.Add("Please switch the DEPRESS PUMP POWER to OFF.");
        egressSteps.Add("Please switch your BATTERY to LOCAL.");
        egressSteps.Add("Please switch the EV-1 POWER to OFF.");
        egressSteps.Add("Verify that your OXYGEN is set to PRIMARY.");
        egressSteps.Add("Verify that your COMMS are set to A.");
        egressSteps.Add("Verify that your FAN is set to PRIMARY.");
        egressSteps.Add("Verify that your PUMP is CLOSED.");
        egressSteps.Add("Verify that your CO2 is set to A.");

        // Initialize INGRESS steps
        ingressSteps.Add("Please verify there is an unbilical connection between the UIA and your DCU.");
        ingressSteps.Add("Switch EV-1 EMU POWER to ON.");
        ingressSteps.Add("Switch your BATTERY to UMBILICAL.");
        ingressSteps.Add("Please OPEN your OXYGEN VENT and wait for confirmation that both your PRIMARY and SECONDARY OXYGEN tanks are under 10psi.");
        ingressSteps.Add("Please CLOSE your OXYGEN VENT.");
        ingressSteps.Add("Please switch your PUMP to OPEN.");
        ingressSteps.Add("Please OPEN your EV-1 WASTE WATER to OPEN and wait for confirmation that the EV-1 coolant tank is UNDER 5%.");
        ingressSteps.Add("Please CLOSE your EV-1 WASTE WATER.");
        ingressSteps.Add("Please switch EV-1 EMU POWER to OFF.");
        ingressSteps.Add("Please DISCONNECT your ubilical connection.");

        messageText.text = "Select Egress or Ingress to begin, then press Next Step.";
    }

    // Call from the "Next Step" button
    public void NextStep()
    {
        if (counter < currentMessages.Count)
        {
            messageText.text = currentMessages[counter];
            counter++;
        }
        else
        {
            messageText.text = "All steps complete. Please start a new procedure, or switch to a different app.";
        }
    }

    //"Start Egress" button
    public void StartEgress()
    {
        counter = 0;
        currentMessages = new List<string>(egressSteps);
        messageText.text = "Starting egress procedure...";
    }

    //"Start Ingress" button
    public void StartIngress()
    {
        counter = 0;
        currentMessages = new List<string>(ingressSteps);
        messageText.text = "Starting ingress procedure...";
    }
}
