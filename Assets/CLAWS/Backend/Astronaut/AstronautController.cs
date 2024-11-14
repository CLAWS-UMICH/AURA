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