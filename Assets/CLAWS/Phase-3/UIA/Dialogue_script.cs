using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue_script : MonoBehaviour
{
    public TextMeshPro messageText; // Assign this in the inspector

    void Update()
    {
        // EVA Egress
        if (/* verify EV1 umbilical connection from UIA to DCU */ true)
        {
            messageText.text = "Power EMU1 ON";
        }
        if (/* EMU1 powered on */ true)
        {
            messageText.text = "Switch BATT to UMB";
        }
        if(/* BATT switched to UMB */ true)
        {
            messageText.text = "Depress Pump Power ON";
        }
        if(/* Depress Pump powered on */ true)
        {
            messageText.text = "OPEN Oxygen O2 Vent";
        }
        if(/* primary and secondary OXY tanks are < 10psi */ true)
        {
            messageText.text = "CLOSE Oxygen O2 Vent";
        }
        if(/* OXYGEN O2 vent closed */ true)
        {
            messageText.text = "Switch OXY to PRI TANK";
        }
        if(/* switched to primary oxygen tank */ true)
        {
            messageText.text = "OPEN Oxygen EMU-1";
        }
        if(/* EV1 primary o2 tank > 3000 psi */ true)
        {
            messageText.text = "CLOSE Oxygen EMU-1";
        }
        if(/* oxygen EMU-1 closed */ true)
        {
            messageText.text = "Switch OXY to SEC TANK";
        }
        if(/* oxygen switched to secondary tank */ true)
        {
            messageText.text = "OPEN Oxygen EMU-1";
        }
        if(/* EV1 secondary O2 tank > 3000 psi */ true)
        {
            messageText.text = "CLOSE Oxygen EMU-1";
        }
        if(/*  oxygen EMU-1 Closed */ true)
        {
            messageText.text = "Switch OXY to PRI TANK";
        }

        if(/*  Wait until SUIT P, O2 P = 4 */ true)
        {
            messageText.text = "Depress Pump Power OFF";
        }
        if(/* DEPRESS PUMP PWR – OFF */ true)
        {
            messageText.text = "Switch BATT Local";
        }
        if(/* BATT – LOCAL */ true)
        {
            messageText.text = "Power EV-1 OFF";
        }
        if(/* EMU1 PWR - OFF */ true){
            messageText.text = "Verify Oxygen set to Primary";
        }
        if(/* Verify OXY – PR */ true){
            messageText.text = "Verify COMMS set to A";
        }
        if(/* Verify COMMS – A */ true){
            messageText.text = "Verify FANS set to Primary";
        }
        if(/* Verify PUMP - CLOSED */ true){
            messageText.text = "Verify PUMP to CLOSED";
        }
        if(/* Verify FAN – PR */ true){
            messageText.text = "Verify CO2 set to A";
        }
        if(/* Verify CO2 – A # */ true){
            messageText.text = "EV1 disconnect UIA and DCU umbilical";
        }
        
        
        // EVA Ingress
        if(/* started depress */ true){
            messageText.text = "EV1 connect UIA and DCU umbilical";
        }
        if(/* EV1 connect UIA and DCU umbilical  */ true){
            messageText.text = "Power EMU EMU ON";
        }
        if(/* EMU1 PWR – ON */ true)
        {
            messageText.text = "Switch BATT to UMB";
        }
        if(/* BATT – UMB */ true){
            messageText.text = "Oxygen O2 Vent OPEN";
        }
        if(/* OXYGEN O2 VENT – OPEN */ true){
            messageText.text = "Close Oxygen O2 VENT";
        }
        if(/* OXYGEN O2 VENT – CLOSE */ true)
        {
            messageText.text = "OPEN PUMP";
        }
        if(/* PUMP – OPEN */ true){
            messageText.text = "EV-1 Waste Water OPEN";
        }
        if(/* EV-1 WASTE WATER – OPEN */ true){
            messageText.text = "Close EV-1 Waste Water";
        }
        if(/* EV-1, WASTE WATER – CLOSE */ true){
            messageText.text = "Power EMU1 OFF";
        }
        if(/* EV-1 EMU PWR – OFF */ true)
        {
            messageText.text = "EV1 disconnect UIA and DCU umbilical";
        }


    }
}
