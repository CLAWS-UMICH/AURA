using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Assumes the earth is a spheroid with a longer major axis than minor
// Will increase our accuracy of geodesic lengths but perhaps maybe more computationally expensive than last year
public class GPSUtils : MonoBehaviour
{
    static public Location originGPSCoords; // Origin coordinates of our player

    static decimal earthMajorAxisLengthInMeters = 6378137.0m; // Semi-major axis length of the earth. This was the value used last year when assuming the earth was a sphere
    static decimal flatteningValue = 1 / 298.257223563m; // Value describing the compression of a circle with semi major axis radius to form a ellipse

    static public void ChangeOriginGPSCoords(Location newOrigin)
    {
        originGPSCoords = newOrigin;
        Vector2 orig = new Vector2((float)newOrigin.latitude, (float)newOrigin.longitude);
        GPSEncoder.SetLocalOrigin(orig);
    }

    // Function used last year with the haversine formula, not as accurate but is more computationally efficient
    // Ran when Vincenty's algorithm doesn't converge in 1000 iterations.
    static private (decimal, decimal) DistanceAndAngleBetweenCoords(Location coords1, Location coords2)
    {
        //Debug.Log("Vincenty's failed, running Haversine");
        //Debug.Log(coords2);

        // Credit to https://www.movable-type.co.uk/scripts/latlong.html

        decimal theta_1 = (decimal)coords1.latitude * (decimal)Math.PI / 180;
        decimal theta_2 = (decimal)coords2.latitude * (decimal)Math.PI / 180;
        decimal delta_theta = ((decimal)coords2.latitude - (decimal)coords1.latitude) * (decimal)Math.PI / 180;
        decimal delta_lambda = ((decimal)coords2.longitude - (decimal)coords1.longitude) * (decimal)Math.PI / 180;

        decimal a = (decimal)Math.Sin((double)(delta_theta / 2)) * (decimal)Math.Sin((double)(delta_theta / 2)) +
                    (decimal)Math.Cos((double)theta_1) * (decimal)Math.Cos((double)theta_2) *
                    (decimal)Math.Sin((double)(delta_lambda / 2)) * (decimal)Math.Sin((double)(delta_lambda / 2));
        decimal c = 2 * (decimal)Math.Atan2(Math.Sqrt((double)a), Math.Sqrt((double)(1 - a)));

        decimal d = earthMajorAxisLengthInMeters * c;

        decimal y = (decimal)Math.Sin((double)delta_lambda) * (decimal)Math.Cos((double)theta_2);
        decimal x = (decimal)Math.Cos((double)theta_1) * (decimal)Math.Sin((double)theta_2) -
                    (decimal)Math.Sin((double)theta_1) * (decimal)Math.Cos((double)theta_2) * (decimal)Math.Cos((double)delta_lambda);
        decimal theta = (decimal)Math.Atan2((double)y, (double)x);

        decimal angle = theta * 180 / (decimal)Math.PI;

        return (d, angle + 180);
    }

    static public Location AppPositionToGPSCoords(Vector3 appPosition)
    {
        /*decimal latitude = (decimal)originGPSCoords.latitude * (decimal)Math.PI / 180;  // Assuming originGPSCoords is known
        decimal longitude = (decimal)originGPSCoords.longitude * (decimal)Math.PI / 180;

        decimal delta = (decimal)Math.Sqrt(Math.Pow(appPosition.x, 2) + Math.Pow(appPosition.z, 2)) / (decimal)earthMajorAxisLengthInMeters;
        decimal angle = (decimal)Math.Atan2((double)appPosition.z, (double)appPosition.x) * 180 / (decimal)Math.PI;

        decimal newLatitude = (decimal)Math.Asin((double)(Math.Sin((double)latitude) * Math.Cos((double)delta) + Math.Cos((double)latitude) * Math.Sin((double)delta) * Math.Cos((double)(angle * (decimal)Math.PI / 180))));
        decimal newLongitude = longitude + (decimal)Math.Atan2(Math.Sin((double)(angle * (decimal)Math.PI / 180)) * Math.Sin((double)delta) * Math.Cos((double)latitude),
                                                      Math.Cos((double)delta) - Math.Sin((double)latitude) * Math.Sin((double)newLatitude));

        newLatitude *= 180 / (decimal)Math.PI;
        newLongitude *= 180 / (decimal)Math.PI;

        return new Location((double)newLatitude, (double)newLongitude);*/
        Vector2 gpsCoords = GPSEncoder.USCToGPS(appPosition);
        return new Location((double)gpsCoords.x, (double)gpsCoords.y);
    }

    static public Vector3 GPSCoordsToAppPosition(Location coords)
    {
        /*// (double distanceFromOrigin, double angleFromOrigin) = GPSCoordsAndAngleBetweenCoords(coords, originGPSCoords);
        (decimal distanceFromOrigin, decimal angleFromOrigin) = DistanceAndAngleBetweenCoords(coords, originGPSCoords);
        decimal distanceFromOriginX = distanceFromOrigin * (decimal)Math.Sin((double)(angleFromOrigin * (decimal)Math.PI / 180));
        decimal distanceFromOriginZ = distanceFromOrigin * (decimal)Math.Cos((double)(angleFromOrigin * (decimal)Math.PI / 180));

        return new Vector3((float)distanceFromOriginX, 0f, (float)distanceFromOriginZ);*/

        Vector2 orig = new Vector2((float)coords.latitude, (float)coords.longitude);
        return GPSEncoder.GPSToUCS(orig);
    }

    static public Vector3 LetterToPosition(int num, char letter)
    {
        double bottomRow = 29.564429362;
        double bottomCol = -95.081985581;
        int row = letter - 'A';
        int col = num;
        double stepR = ((-95.081985581 + 95.080944569) / 28.0);
        double stepC = ((29.565400966 - 29.564429362) / 26.0);

        Location loc = new Location();
        loc.longitude = bottomCol + (stepR * col);
        loc.latitude = bottomRow + (stepC * row);

        return GPSCoordsToAppPosition(loc);
    }

    static public Location LetterToLocation(int num, char letter)
    {
        double bottomRow = 29.564429362;
        double bottomCol = -95.081985581;
        int row = letter - 'A';
        int col = num;
        double step = 0.00003594;

        Location loc = new Location();
        loc.longitude = bottomCol + (step * col);
        loc.latitude = bottomRow + (step * row);

        return loc;
    }


}