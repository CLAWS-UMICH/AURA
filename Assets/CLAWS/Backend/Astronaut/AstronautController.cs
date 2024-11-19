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