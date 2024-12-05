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
    public double batt_time_left;
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

    public double batt_percentage;
    public double oxy_percentage;
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
        public TaskObj()
        {
            task_id = 0;
            status = 0;
            title = "";
            description = "";
            isEmergency = false;
            isShared = false;
            isSubtask = false;
            location = "";
            astronauts = new List<int>();
            subtasks = new List<TaskObj>();
        }
        public TaskObj(int t_id, int st, string tle, string desc, bool em, bool sh, bool sut, string loc, List<int> astrs, List<TaskObj> subts)
        {
            task_id = t_id;
            status = st;
            title = tle;
            description = desc;
            isEmergency = em;
            isShared = sh;
            isSubtask = sut;
            location = loc;
            astronauts = new List<int>();
            foreach(int a in astrs)
            {
                astronauts.Add(a);
            }
            subtasks = new List<TaskObj>();
            if (description.Length > 0){
                foreach (TaskObj t in subts)
                {
                    subtasks.Add(t);
                }
            }
        }
    }
    List<TaskObj> Tasklist = new List<TaskObj>();

    public TasklistObj()
    {
        //do nothing for now
    }
    public TasklistObj(List<TaskObj> data)
    {
        foreach (TaskObj task_d in data)
        {
            TaskObj task = new TaskObj(task_d.task_id, task_d.status, task_d.title, task_d.description, task_d.isEmergency, task_d.isShared, task_d.location, task_d.astronauts, task_d.subtasks);
            Tasklist.Add(task);
        }
    }
}