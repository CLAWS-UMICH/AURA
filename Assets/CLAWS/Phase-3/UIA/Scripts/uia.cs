using System;
using System.Collections.Generic;

// EventArgs derivative that carries sensor update details.
public class TelemetryEventArgs : EventArgs
{
    public string SensorName { get; set; }
    // New sensor value: true indicates the sensor is in the "active" state (e.g., ON or OPEN)
    public bool NewValue { get; set; }
}

// This class simulates the telemetry stream server (CAPCOM Web Interface for S.U.I.T.S. Telemetry Server C)
// which fires an event every time a sensor event occurs.
public class TelemetryStreamServer
{
    // Event that subscribers (such as our handler) can listen to
    public event EventHandler<TelemetryEventArgs> TelemetryChanged;

    // Call this method to simulate a sensor state update.
    public void UpdateSensor(string sensorName, bool newValue)
    {
        // Raise the event if any subscribers exist.
        TelemetryChanged?.Invoke(this, new TelemetryEventArgs { SensorName = sensorName, NewValue = newValue });
    }
}

// This class sets up the event subscription and contains the event handler that is executed upon telemetry updates.
public class TelemetryHandler
{
    // A dictionary to store the latest state text for each sensor.
    private Dictionary<string, string> sensorStatus = new Dictionary<string, string>();

    // Constructor subscribes to telemetry events from the TelemetryStreamServer.
    public TelemetryHandler(TelemetryStreamServer server)
    {
        // Subscribe to the event.
        server.TelemetryChanged += OnTelemetryChanged;
    }

    // Event handler that processes sensor update events.
    private void OnTelemetryChanged(object sender, TelemetryEventArgs e)
    {
        // Convert the boolean sensor value to a textual representation based on the sensor type.
        string newState = MapSensorValueToText(e.SensorName, e.NewValue);
        // Update the local sensor status dictionary.
        sensorStatus[e.SensorName] = newState;
        // Here, we simply output the update; in a real application, you might update UI components.
        Console.WriteLine($"Sensor: {e.SensorName} updated to: {newState}");
    }

    // This helper method maps a sensor's boolean value to its corresponding text.
    // It uses your provided table:
    //   EMUx POWER: true = "ON", false = "OFF"
    //   EVx SUPPLY, EVx WASTE, EVx OXYGEN, O2 Vent: true = "OPEN", false = "CLOSED"
    //   And for any others it will simply show "True" or "False"
    private string MapSensorValueToText(string sensorName, bool value)
    {
        // Normalize the sensor name for a simple check.
        sensorName = sensorName.ToUpper();

        // EMU POWER and DEPRESS PUMP sensors use ON/OFF
        if (sensorName.Contains("EMU") || sensorName.Contains("DEPRESS PUMP"))
        {
            return value ? "ON" : "OFF";
        }
        // EVA related sensors use OPEN/CLOSED
        else if (sensorName.Contains("EV") || sensorName.Contains("O2 VENT"))
        {
            return value ? "OPEN" : "CLOSED";
        }
        // Fallback for any unspecified sensor type.
        return value ? "True" : "False";
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // Instantiate the telemetry stream server.
        TelemetryStreamServer server = new TelemetryStreamServer();
        
        // Instantiate the telemetry handler and subscribe it to telemetry events.
        TelemetryHandler handler = new TelemetryHandler(server);

        // Simulate some telemetry events being received.
        // These events simulate sensor state changes as defined in your table.
        server.UpdateSensor("EMU1 POWER", true);    // Expect: "ON"
        server.UpdateSensor("EV1 SUPPLY", true);     // Expect: "OPEN"
        server.UpdateSensor("EV1 WASTE", false);     // Expect: "CLOSED"
        server.UpdateSensor("EV1 OXYGEN", true);       // Expect: "OPEN"
        server.UpdateSensor("EMU2 POWER", false);     // Expect: "OFF"
        server.UpdateSensor("EV2 SUPPLY", true);       // Expect: "OPEN"
        server.UpdateSensor("EV2 WASTE", false);       // Expect: "CLOSED"
        server.UpdateSensor("EV2 OXYGEN", false);      // Expect: "CLOSED"
        server.UpdateSensor("O2 Vent", true);          // Expect: "OPEN"
        server.UpdateSensor("DEPRESS PUMP", true);     // Expect: "ON"

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
