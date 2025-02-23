using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;

// Location
[System.Serializable]
public class Location
{
    public double latitude;
    public double longitude;

    public Location() { }

    public Location(double lat, double lon)
    {
        latitude = lat;
        longitude = lon;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Location otherLoc = (Location)obj;
        return latitude == otherLoc.latitude &&
               longitude == otherLoc.longitude;
    }
}

// Vitals
[System.Serializable]
public class Vitals
{
    public int eva_time;
    public int batt_time_left;
    public double oxy_pri_storage;
    public double oxy_sec_storage;
    public double oxy_pri_pressure;
    public double oxy_sec_pressure;
    public int oxy_time_left;
    public double heart_rate;
    public double oxy_consumption;
    public double co2_production;
    public double suit_pressure_oxy;
    public double suit_pressure_co2;
    public double suit_pressure_other;
    public double suit_pressure_total;
    public double fan_pri_rpm;
    public double fan_sec_rpm;
    public double helmet_pressure_co2;
    public double scrubber_a_co2_storage;
    public double scrubber_b_co2_storage;
    public double temperature;
    public double coolant_m;
    public double coolant_gas_pressure;
    public double coolant_liquid_pressure;
}

// TestWebObj
[System.Serializable]
public class TestWebObj
{
    public int num;

    public TestWebObj() { 
        num = 0;
    }

    public TestWebObj(int _num)
    {
        num = _num;
    }
}

// TasklistObj
[System.Serializable]
public class TasklistObj
{
 
    public List<TaskObj> Tasklist = new List<TaskObj>();
    public TaskObj currentTask = new TaskObj();

    public TasklistObj()
    {
        //do nothing for now
    }
    public TasklistObj(List<TaskObj> data)
    {
        foreach (TaskObj task_d in data)
        {
            TaskObj task = new TaskObj(task_d.task_id, task_d.status, task_d.title, task_d.taskType, task_d.description, task_d.isEmergency, task_d.isShared, task_d.isSubtask, task_d.location, task_d.astronauts, task_d.subtasks);
            Tasklist.Add(task);
        }
    }

    public void add(TaskObj t)
    {
        Tasklist.Add(t);
    }

    public void insert(int pos, TaskObj t)
    {
        Tasklist.Insert(pos, t);
    }

    public void update(TaskObj newT)
    {
        for (int i = 0; i < Tasklist.Count; i++)
        {
            if (Tasklist[i].task_id == newT.task_id)
            {
                Tasklist[i] = newT;
                break;
            }
        }
    }
}

public class TaskObj
{
    public int task_id;
    public int status;
    public string title;
    public string taskType;
    public string description;
    public bool isEmergency;
    public bool isShared;
    public bool isSubtask;

    // change later for location constructor if sent in tuple
    public string location;
    public List<int> astronauts;
    public List<TaskObj> subtasks;
    private int numSub;
    private int comSub;
    public TaskObj()
    {
        task_id = 0;
        status = 0;
        title = "";
        description = "";
        taskType = "";
        isEmergency = false;
        isShared = false;
        isSubtask = false;
        location = "";
        astronauts = new List<int>();
        subtasks = new List<TaskObj>();
        numSub = 0;
        comSub = 0;
    }
    public TaskObj(int t_id, int st, string tle, string desc, string t_type, bool em, bool sh, bool sut, string loc, List<int> astrs, List<TaskObj> subts)
    {
        task_id = t_id;
        status = st;
        title = tle;
        description = desc;
        taskType = t_type;
        isEmergency = em;
        isShared = sh;
        isSubtask = sut;
        location = loc;
        astronauts = new List<int>();
        foreach (int a in astrs)
        {
            astronauts.Add(a);
        }
        subtasks = new List<TaskObj>();
        if (!isSubtask)
        {
            if (subts.Count > 0)
            {
                foreach (TaskObj t in subts)
                {
                    subtasks.Add(t);
                }
                numSub = subts.Count;
                comSub = 0;
            }
            else
            {
                numSub = -1;
                comSub = -1;
            }
        }
        else
        {
            numSub = -1;
            comSub = -1;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        TaskObj otherTask = (TaskObj)obj;
        return task_id == otherTask.task_id &&
               title == otherTask.title &&
               astronauts.Equals(otherTask.astronauts) &&
               subtasks.Equals(otherTask.subtasks) &&
               status == otherTask.status &&
               isEmergency == otherTask.isEmergency &&
               isSubtask == otherTask.isSubtask && 
               description == otherTask.description &&
               isShared == otherTask.isShared &&
               location == otherTask.location;
    }

    public void addCom()
    {
        comSub += 1;
    }

    public int getNumSub()
    {
        return numSub;
    }
    public int getComSub()
    {
        return comSub;
    }
}

//Messaging
[System.Serializable]
public class Messaging
{
    public List<Message> AllMessages = new List<Message>();
}

[System.Serializable]
public class Message
{
    static int global_message_id = 0;

    public int message_id; // starting from 0 and going up 1
    public int sent_to; // Astronaut ID it was sent to  //Astrounaut1 = 1, Astronaut2 = 2, LMCC = 3, Group = 4
    public string message; 
    public int from; // Astronaut ID it who sent the message    //Astrounaut1 = 1, Astronaut2 = 2, LMCC = 3, Group = 4

    public Message()
    {
        global_message_id++;
    }

    public Message(int init_sent_to, string init_message, int init_from)
    {
        message_id = global_message_id++;
        sent_to = init_sent_to;
        message = init_message;
        from = init_from;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Message otherMessage = (Message)obj;
        return message_id == otherMessage.message_id &&
               sent_to == otherMessage.sent_to &&
               message == otherMessage.message &&
               from == otherMessage.from;
    }
}


[System.Serializable]
public class SPEC
{
    public SpecEVAs spec;
}

[System.Serializable]
public class SpecEVAs
{
    public EvaData eva1;
    public EvaData eva2;
}

[System.Serializable]
public class EvaData 
{
    public string name; // Name of rock
    public int id; // id of rock from NASA
    public DataDetails data; // data of rock
}
[System.Serializable]
public class DataDetails
{
    public double SiO2;
    public double TiO2;
    public double Al2O3;
    public double FeO;
    public double MnO;
    public double MgO;
    public double CaO;
    public double K2O;
    public double P2O3;
}

// Data of other Fellow Astronauts
[System.Serializable]
public class FellowAstronaut
{
    public int astronaut_id;
    public Location location;
    public string color;
    public Vitals vitals;
    public bool navigating;
    public AllBreadCrumbs bread_crumbs;

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        FellowAstronaut otherA = (FellowAstronaut)obj;
        return astronaut_id == otherA.astronaut_id &&
               location.Equals(otherA.location) &&
               color == otherA.color &&
               vitals.Equals(otherA.vitals) &&
               navigating == otherA.navigating &&
               bread_crumbs.Equals(otherA.bread_crumbs);
    }
}


// TSS
[System.Serializable]
public class COMM
{
    public CommDetails comm;
}

[System.Serializable]
public class CommDetails
{
    public bool comm_tower;
}

[System.Serializable]
public class DCU
{
    public DCUData dcu;
}

[System.Serializable]
public class DCUData
{
    public EvaDetails eva1;
    public EvaDetails eva2;
}

[System.Serializable]
public class IMU
{
    public IMUEVAs imu;
}

[System.Serializable]
public class IMUEVAs
{
    public IMUData eva1;
    public IMUData eva2;
}

[System.Serializable]
public class IMUData
{
    public double posx;
    public double posy;
    public double heading;
}


[System.Serializable]
public class EvaDetails
{
    public bool batt;
    public bool oxy;
    public bool comm;
    public bool fan;
    public bool pump;
    public bool co2;
}

[System.Serializable]
public class ROVER
{
    public RoverDetails rover;
}

[System.Serializable]
public class RoverDetails
{
    public double posx;
    public double posy;
    public int qr_id;
}

[System.Serializable]
public class TELEMETRY
{
    public TelemetryDetails telemetry;
}

[System.Serializable]
public class TelemetryDetails
{
    public int eva_time;
    public EvaTelemetryDetails eva1;
    public EvaTelemetryDetails eva2;
}

[System.Serializable]
public class EvaTelemetryDetails
{
    public int eva_time;
    public int batt_time_left;
    public double oxy_pri_storage;
    public double oxy_sec_storage;
    public double oxy_pri_pressure;
    public double oxy_sec_pressure;
    public int oxy_time_left;
    public double heart_rate;
    public double oxy_consumption;
    public double co2_production;
    public double suit_pressure_oxy;
    public double suit_pressure_co2;
    public double suit_pressure_other;
    public double suit_pressure_total;
    public double fan_pri_rpm;
    public double fan_sec_rpm;
    public double helmet_pressure_co2;
    public double scrubber_a_co2_storage;
    public double scrubber_b_co2_storage;
    public double temperature;
    public double coolant_m;
    public double coolant_gas_pressure;
    public double coolant_liquid_pressure;
}

[System.Serializable]
public class UIA
{
    public UiDetails uia;
}

[System.Serializable]
public class UiDetails
{
    public bool eva1_power;
    public bool eva1_oxy;
    public bool eva1_water_supply;
    public bool eva1_water_waste;
    public bool eva2_power;
    public bool eva2_oxy;
    public bool eva2_water_supply;
    public bool eva2_water_waste;
    public bool oxy_vent;
    public bool depress;
}


