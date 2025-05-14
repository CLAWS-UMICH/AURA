using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Astronaut
{
    // EV Info
    public int id;
    public string name;
    public string avatarColor;
    public FellowAstronaut fellowAstronaut;

    // Initial Location data
    public double latitude;
    public double longitude;
    public Location origin;

    // URL info
    public string LMCCurl;
    public string TSSurl;

    // Feature Info
    public Location current;
    public Vitals vitals;
    public Messaging messages;
    public Data currentData;


    // TSS Info
    public COMM comm;
    public DCU dcu;
    public IMU imu;
    public SPEC spec;
    public ROVER rover;
    public TELEMETRY telemetry;
}
