using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using System.Numerics;
using Unity.Collections.LowLevel.Unsafe;

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

public struct Composition {
    string element;
    float amount;
    public Composition(string elt, float amt) {
        element = elt;
        amount = amt;
    }
}
[System.Serializable]
public class GeoSample {
    public string name;
    public string zone;
    public string type;
    public string shape;
    public string color;
    public string note;
    public int id;
    public List<Composition> comp;
    public GeoSample() {
        name = "";
        zone = "";
        type = "";
        shape = "";
        color = "";
        note = "";
        id = 0;
        comp = new List<Composition>();
    }
    public GeoSample(string nameIn, string zoneIn, string typeIn, string shapeIn, string colorIn, string noteIn, int idIn, List<Composition> compIn) {
        name = nameIn;
        zone = zoneIn;
        type = typeIn;
        shape = shapeIn;
        color = colorIn;
        note = noteIn;
        id = idIn;
        comp = new List<Composition>(compIn);
    }
}
[System.Serializable]
public class GeoSampleDB {
    List<GeoSample> samples;
    GeoSample curr;
    public GeoSampleDB() {
        samples = new List<GeoSample>();
    }
     public GeoSampleDB(List<GeoSample> data)
    {
        foreach (GeoSample sample in data)
        {
            GeoSample geo = new GeoSample(sample.name, sample.zone, sample.type, sample.shape, sample.color, sample.note, sample.id, sample.comp);
            samples.Add(geo);
        }
    }

    public void add(GeoSample g)
    {
        samples.Add(g);
    }

    public void insert(int pos, GeoSample g)
    {
        samples.Insert(pos, g);
    }

    public void update(GeoSample newG)
    {
        for (int i = 0; i < samples.Count; i++)
        {
            if (samples[i].id == newG.id)
            {
                samples[i] = newG;
                break;
            }
        }
    }

}