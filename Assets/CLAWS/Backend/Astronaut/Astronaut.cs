using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Astronaut
{
    public int id;
    public Location location;
    public Vitals vitals;
    public TasklistObj tasklist;
    public Messaging messages;
    public FellowAstronaut FellowAstronautsData;

    // TSS Info
    public COMM comm;
    public DCU dcu;
    public IMU imu;
    public ROVER rover;
    public SPEC spec;
    public TELEMETRY telemetry;
    public UIA uia;
}
