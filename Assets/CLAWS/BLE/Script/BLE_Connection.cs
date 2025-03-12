using UnityEngine;
using ArduinoBluetoothAPI;

public class ESP32BLE : MonoBehaviour
{
    private BluetoothHelper bluetoothHelper;
    private string deviceName = "ESP32-C3-MINI-1";
    private string serviceUUID = "12345678-1234-5678-1234-56789abcdef0";
    private string characteristicUUID = "87654321-4321-6789-4321-fedcba987654";

    void Start()
    {
        bluetoothHelper = BluetoothHelper.GetInstance(deviceName);
        bluetoothHelper.OnConnected += (helper) => {
            Debug.Log("Connected to ESP32-C3!");
            bluetoothHelper.Subscribe(new BluetoothHelperCharacteristic(serviceUUID));
        };

        bluetoothHelper.OnDataReceived += (helper) => {
            string receivedData = helper.Read();
            Debug.Log("Received: " + receivedData);
        };

        bluetoothHelper.OnCharacteristicChanged += (helper, value, characteristic) => {
            Debug.Log("Characteristic Updated: " + System.Text.Encoding.UTF8.GetString(value));
        };

        bluetoothHelper.setDeviceName(deviceName);
        bluetoothHelper.Connect();
    }

    void OnDestroy()
    {
        bluetoothHelper.Disconnect();
    }
}
