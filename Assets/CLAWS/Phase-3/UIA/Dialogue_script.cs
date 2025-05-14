using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue_script : MonoBehaviour
{
    public TextMeshProUGUI messageText; // Assign in Inspector
    private int counter = 0;
    private List<string> messages = new List<string>();

    void Start()
    {
        // Initialize your dialogue steps here in the correct order
        messages.Add("Power EMU1 ON");
        messages.Add("Switch BATT to UMB");
        messages.Add("Depress Pump Power ON");
        messages.Add("OPEN Oxygen O2 Vent");
        messages.Add("CLOSE Oxygen O2 Vent");
        messages.Add("Switch OXY to PRI TANK");
        messages.Add("OPEN Oxygen EMU-1");
        messages.Add("CLOSE Oxygen EMU-1");
        messages.Add("Switch OXY to SEC TANK");
        messages.Add("OPEN Oxygen EMU-1");
        messages.Add("CLOSE Oxygen EMU-1");
        messages.Add("Switch OXY to PRI TANK");
        messages.Add("Depress Pump Power OFF");
        messages.Add("Switch BATT Local");
        messages.Add("Power EV-1 OFF");
        messages.Add("Verify Oxygen set to Primary");
        messages.Add("Verify COMMS set to A");
        messages.Add("Verify FANS set to Primary");
        messages.Add("Verify PUMP to CLOSED");
        messages.Add("Verify CO2 set to A");
        messages.Add("EV1 disconnect UIA and DCU umbilical");
        messages.Add("EV1 connect UIA and DCU umbilical");
        messages.Add("Power EMU EMU ON");
        messages.Add("Switch BATT to UMB");
        messages.Add("Oxygen O2 Vent OPEN");
        messages.Add("Close Oxygen O2 VENT");
        messages.Add("OPEN PUMP");
        messages.Add("EV-1 Waste Water OPEN");
        messages.Add("Close EV-1 Waste Water");
        messages.Add("Power EMU1 OFF");
        messages.Add("EV1 disconnect UIA and DCU umbilical");

        messageText.text = "Ready for action. Hit the button! 🚀";
    }

    // Call this from the Button's OnClick()
    public void NextStep()
    {
        if (counter < messages.Count)
        {
            messageText.text = messages[counter];
            counter++;
        }
        else
        {
            messageText.text = "All steps complete ✅";
        }
    }
}
