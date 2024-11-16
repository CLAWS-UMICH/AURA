using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Waypoint
{   
    public int Id { get; set; }
    public string Name { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z {get; set; }
    public string Letter {get; set;}
    public string Type{get; set;}
    public GameObject WaypointObject { get; set; }

    public Waypoint(int id, string name, float x, float y, float z, string letter, string type, GameObject waypointObject)
    {
        Id = id;
        Name = name;
        X = x;
        Y = y;
        Z = z;
        Letter = letter;
        Type = type;
        WaypointObject = waypointObject;
    }
    
    public string getJsonString(string use) {
        /*
        Waypoint json:
        {
            "type": "WAYPOINT",
            "use": "<GET/POST/PUT/DELETE>",
            "data": {
                "id": <number>,
                "name": <string>,
                "location": <Location>,
                "type": <string>,
                "author": <string>
            }
        }
        */
        string jsonString = "{"
            + "\"type\": \"WAYPOINT\","
            + "\"use\": \"" + use + "\","
            + "\"data\": {"
                + "\"id\": " + Id + ","
                + "\"name\": \"" + Name + "\","
                + "\"location\": {"
                    + "\"x\": " + X + ","
                    + "\"y\": " + Y + ","
                    + "\"z\": " + Z
                + "},"
                + "\"type\": \"" + Type + "\","
                + "\"author\": \"" + Author + "\""
            + "}"
        + "}";
        return jsonString;
    }
   
}

