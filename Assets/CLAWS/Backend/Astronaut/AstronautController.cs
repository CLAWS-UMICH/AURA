using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using System.Numerics;
using Unity.Collections.LowLevel.Unsafe;
using GLTFast.Schema;


/////////////////////////////////////////////////////////////////////
////////////////////////////  FEATURES  /////////////////////////////
/////////////////////////////////////////////////////////////////////

[System.Serializable]
public class Data
{
    public string client; // Target client (e.g., "hololens_1", "hololens_2", "pr_client")
    public string room;   // Room name (e.g., "VITALS", "WAYPOINTS", "MESSAGES")
    public Dictionary<string, object> data; // The message to send, dependent on what the room is
}

////////////////////////////  LOCATION  /////////////////////////////
[System.Serializable]
public class Location /// UNITY LOCATION ///
{
    public double posX; // X ACIS
    public double posY;
    public double posZ; // Y AXIS
    public double Heading;

    public Location() { }

    public Location(double posx, double posy, double posz, double heading)
    {
        this.posX = posx;
        this.posY = posy;
        this.posZ = posz;
        this.Heading = heading;
    }
    
        public override int GetHashCode()
        {
            return (posX, posY, posZ, Heading).GetHashCode();
        }
    

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Location otherLoc = (Location)obj;
        return posX == otherLoc.posX &&
               posY == otherLoc.posY &&
               posZ == otherLoc.posZ &&
               Heading == otherLoc.Heading;
    }
}


////////////////////////////  WAYPOINTS  /////////////////////////////
[System.Serializable]
public class Waypoint
{
    public string  Use { get; set; } // Use this to determine if the waypoint is added/deleted
    public int Id { get; set; } // Sequential ID
    public string Name { get; set; } // Name of the waypoint
    public double IMUposX { get; set; } // LUNAR X coordinate 
    public double IMUposY { get; set; } // LUNAR Y coordinate
    public WaypointType Type { get; set; } // Enum for waypoint type
    public AuthorType Author { get; set; } // Enum for author type

    public Waypoint() { }

    public Waypoint(string use, int waypointId, string name, double posx, double posy, WaypointType type, AuthorType author)
    {
        Use = use;
        Id = waypointId;
        Name = name;
        IMUposX = posx;
        IMUposY = posy;
        Type = type;
        Author = author;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Waypoint otherWaypoint = (Waypoint)obj;
        return Use == otherWaypoint.Use && 
               Id == otherWaypoint.Id && 
               Name == otherWaypoint.Name &&
               IMUposX == otherWaypoint.IMUposX &&
               IMUposY == otherWaypoint.IMUposY &&
               Type == otherWaypoint.Type &&
               Author == otherWaypoint.Author;
    }
    public override int GetHashCode()
    {
        return (Use, Id, Name, IMUposX, IMUposY, Type, Author).GetHashCode();
    }
}

public enum WaypointType
{
    POI,
    GEO,
    DANGER,
    STATION
}

public enum AuthorType
{
    EV1,
    EV2,
    PR
}



[System.Serializable]
public class AllBreadCrumbs
{
    public List<Breadcrumb> AllCrumbs = new List<Breadcrumb>();

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        AllBreadCrumbs otherCrumbs = (AllBreadCrumbs)obj;

        // Compare lists using the SequenceEqual method from System.Linq
        return AllCrumbs.SequenceEqual(otherCrumbs.AllCrumbs);
    }

    public override int GetHashCode()
    {
        return AllCrumbs != null ? AllCrumbs.Aggregate(0, (hash, crumb) => hash ^ crumb.GetHashCode()) : 0;
    }
}

[System.Serializable]
public class Breadcrumb
{
    public int crumb_id;
    public Location location;
    public int type; // 0: backtracking and 1: navigation

    public Breadcrumb(int crumbId, Location location, int type)
    {
        this.crumb_id = crumbId;
        this.location = location;
        this.type = type;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Breadcrumb otherBread = (Breadcrumb)obj;
        return crumb_id == otherBread.crumb_id &&
               location.Equals(otherBread.location) &&
               type == otherBread.type;
    }

    public override int GetHashCode()
    {
        return (crumb_id, location, type).GetHashCode();
    }
}


////////////////////////////  VITALS  /////////////////////////////
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
}

[System.Serializable]
public class FellowAstronaut
{
    public int astronaut_id;
    public string name;
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
    public override int GetHashCode()
    {
        return (astronaut_id, location, color, vitals, navigating, bread_crumbs).GetHashCode();
    }
}


////////////////////////////  ALERTS  /////////////////////////////
[System.Serializable]
public class Alerts
{
    public List<AlertObj> AllAlerts = new List<AlertObj>();
}
[System.Serializable]
public class AlertObj
{
    public int alert_id; // starting from 0 and going up 1 
    public int astronaut_in_danger; // ID who is in danger
    public string vital; // vital that is in danger
    public float vital_val; // that vital's value

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        AlertObj otherAlert = (AlertObj)obj;
        return alert_id == otherAlert.alert_id &&
               astronaut_in_danger == otherAlert.astronaut_in_danger &&
               vital == otherAlert.vital &&
               vital_val == otherAlert.vital_val;
    }

    public override int GetHashCode()
    {
        return (alert_id, astronaut_in_danger, vital, vital_val).GetHashCode();
    }
}


////////////////////////////  ROVER  /////////////////////////////

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
    public double poi_1_x;
    public double poi_1_y;
    public double poi_2_x;
    public double poi_2_y;
    public double poi_3_x;
    public double poi_3_y;
    public bool ping;
}


////////////////////////////  TASKLIST  /////////////////////////////
// [System.Serializable]
// public class TasklistObj
// {
 
//     public List<TaskObj> Tasklist = new List<TaskObj>();
//     public TaskObj currentTask = new TaskObj();

//     public TasklistObj() { }
//     public TasklistObj(List<TaskObj> data)
//     {
//         foreach (TaskObj task_d in data)
//         {
//             TaskObj task = new TaskObj(task_d.task_id, task_d.status, task_d.title, task_d.taskType, task_d.description, task_d.isEmergency, task_d.isShared, task_d.isSubtask, task_d.location, task_d.astronauts, task_d.subtasks);
//             Tasklist.Add(task);
//         }
//     }

//     public void add(TaskObj t)
//     {
//         Tasklist.Add(t);
//     }

//     public void insert(int pos, TaskObj t)
//     {
//         Tasklist.Insert(pos, t);
//     }

//     public void update(TaskObj newT)
//     {
//         for (int i = 0; i < Tasklist.Count; i++)
//         {
//             if (Tasklist[i].task_id == newT.task_id)
//             {
//                 Tasklist[i] = newT;
//                 break;
//             }
//         }
//     }
// }

// public class TaskObj
// {
//     public int task_id;
//     public int status;
//     public string title;
//     public string taskType;
//     public string description;
//     public bool isEmergency;
//     public bool isShared;
//     public bool isSubtask;


//     // change later for location constructor if sent in tuple
//     public string location;
//     public List<int> astronauts;
//     public List<TaskObj> subtasks;
//     private int numSub;
//     private int comSub;
//     public TaskObj()
//     {
//         task_id = 0;
//         status = 0;
//         title = "";
//         description = "";
//         taskType = "";
//         isEmergency = false;
//         isShared = false;
//         isSubtask = false;
//         location = "";
//         astronauts = new List<int>();
//         subtasks = new List<TaskObj>();
//         numSub = 0;
//         comSub = 0;
//     }
//     public TaskObj(int t_id, int st, string tle, string desc, string t_type, bool em, bool sh, bool sut, string loc, List<int> astrs, List<TaskObj> subts)
//     {
//         task_id = t_id;
//         status = st;
//         title = tle;
//         description = desc;
//         taskType = t_type;
//         isEmergency = em;
//         isShared = sh;
//         isSubtask = sut;
//         location = loc;
//         astronauts = new List<int>();
//         foreach (int a in astrs)
//         {
//             astronauts.Add(a);
//         }
//         subtasks = new List<TaskObj>();
//         if (!isSubtask)
//         {
//             if (subts.Count > 0)
//             {
//                 foreach (TaskObj t in subts)
//                 {
//                     subtasks.Add(t);
//                 }
//                 numSub = subts.Count;
//                 comSub = 0;
//             }
//             else
//             {
//                 numSub = -1;
//                 comSub = -1;
//             }
//         }
//         else
//         {
//             numSub = -1;
//             comSub = -1;
//         }
//     }

//     public override bool Equals(object obj)
//     {
//         if (obj == null || GetType() != obj.GetType())
//         {
//             return false;
//         }

//         TaskObj otherTask = (TaskObj)obj;
//         return task_id == otherTask.task_id &&
//                title == otherTask.title &&
//                astronauts.Equals(otherTask.astronauts) &&
//                subtasks.Equals(otherTask.subtasks) &&
//                status == otherTask.status &&
//                isEmergency == otherTask.isEmergency &&
//                isSubtask == otherTask.isSubtask && 
//                description == otherTask.description &&
//                isShared == otherTask.isShared &&
//                location == otherTask.location;
//     }

//     public override int GetHashCode()
//     {
//         return (task_id, title, astronauts, subtasks, status, isEmergency, isSubtask, description, isShared, location).GetHashCode();
//     }

//     public void addCom()
//     {
//         comSub += 1;
//     }

//     public int getNumSub()
//     {
//         return numSub;
//     }
//     public int getComSub()
//     {
//         return comSub;
//     }
// }


////////////////////////////  GEO SAMPLES  /////////////////////////////
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
    public string texture;
    public string note;
    public int id;  // NASA GIVEN ID
    public bool isSignificant;
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
    public GeoSample(string nameIn, string zoneIn, string typeIn, string shapeIn, string colorIn, string textureIn, string noteIn, int idIn, bool isSignificant, List<Composition> compIn) {
        name = nameIn;
        zone = zoneIn;
        type = typeIn;
        shape = shapeIn;
        color = colorIn;
        texture = textureIn;
        note = noteIn;
        id = idIn;
        comp = new List<Composition>(compIn);
    }
}
[System.Serializable]
public class GeoSampleDB {
    public List<GeoSample> samples;

    public GeoSampleDB() {
        samples = new List<GeoSample>();
    }
     public GeoSampleDB(List<GeoSample> data)
    {
        foreach (GeoSample sample in data)
        {
            GeoSample geo = new GeoSample(sample.name, sample.zone, sample.type, sample.shape, sample.color, sample.texture, sample.note, sample.id, sample.isSignificant,sample.comp);
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

    public void update(GeoSample newSample)
    {
        for (int i = 0; i < samples.Count; i++)
        {
            if (samples[i].id == newSample.id)
            {
                samples[i] = newSample;
                break;
            }
        }
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

[System.Serializable]
public class GeosampleZone
{
    public int id;
    public Location origin;
    public GeoSampleDB TotalGeoSamples;
    public GeoSampleDB SignificantGeoSamples;
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        GeosampleZone otherGeoZone = (GeosampleZone)obj;
        return id == otherGeoZone.id &&
               TotalGeoSamples.Equals(otherGeoZone.TotalGeoSamples) &&
               SignificantGeoSamples.Equals(otherGeoZone.SignificantGeoSamples);
    }

    public override int GetHashCode()
    {
        return (id, TotalGeoSamples, SignificantGeoSamples).GetHashCode();
    }
}


////////////////////////////  MESSAGES  /////////////////////////////
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
    public int from; // Astronaut ID it who sent the message    //Astrounaut1 = 1, Astronaut2 = 2, LMCC = 3

    public override int GetHashCode()
    {
        return (message_id, sent_to, message, from).GetHashCode();
    }





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




/////////////////////////////////////////////////////////////////////
//////////////////////////////  TSS  ////////////////////////////////
/////////////////////////////////////////////////////////////////////

///////////////////////////  COMM TOWER  ////////////////////////////
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


/////////////////////////////  DCU  //////////////////////////////
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
public class EvaDetails
{
    public bool batt;
    public bool oxy;
    public bool comm;
    public bool fan;
    public bool pump;
    public bool co2;
}

///////////////////////////  IMU  ////////////////////////////
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


// ///////////////////////////  ROVER  ////////////////////////////
// [System.Serializable]
// public class ROVER
// {
//     public RoverDetails rover;
// }

// [System.Serializable]
// public class RoverDetails
// {
//     public double posx;
//     public double posy;
//     public int qr_id;
// }



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
}


///////////////////////////  UIA  ////////////////////////////
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