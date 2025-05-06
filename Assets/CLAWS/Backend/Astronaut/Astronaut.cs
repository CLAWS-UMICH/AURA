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

    // URL info
    public string LMCCurl;
    public string TSSurl;

    // Feature Info
    public Location location;
    public Vitals vitals;
    public Messaging messages;


    // TSS Info
    public COMM comm;
    public DCU dcu;
    public IMU imu;
    public SPEC spec;
    public TELEMETRY telemetry;
}
