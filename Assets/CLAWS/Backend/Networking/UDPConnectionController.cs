using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Serialization;

public class UDPConnectionController : MonoBehaviour
{
    private string tssIP;
    private int tssPort;

    public string prTeamIP; // GET FROM SETUP
    public int prTeamPort; // GET FROM SETUP

    private UdpClient tssClient;
    private UdpClient prClient;
    private IPEndPoint tssEndPoint;
    private IPEndPoint prEndPoint;

    public Action<uint, uint, float> OnTSSDataReceived;
    public Action<string> OnPRDataReceived;


    void Start()
    {
        tssIP = new Uri(AstronautInstance.User.TSSurl).Host;
         // Initialize TSS client
        tssClient = new UdpClient();
        tssEndPoint = new IPEndPoint(IPAddress.Parse(tssIP), tssPort);
        tssClient.Client.Bind(new IPEndPoint(IPAddress.Any, tssPort));
        tssClient.BeginReceive(OnReceiveTSSUDP, null);

        // Initialize PR client
        // prClient = new UdpClient();
        // prEndPoint = new IPEndPoint(IPAddress.Parse(prTeamIP), prTeamPort);
        // prClient.Client.Bind(new IPEndPoint(IPAddress.Any, prTeamPort));
        // prClient.BeginReceive(OnReceivePRUDP, null);
    }


    public void SendTSSCommand(uint commandNumber)
    {
        uint timestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        byte[] packet = new byte[8];
        Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)timestamp)), 0, packet, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)commandNumber)), 0, packet, 4, 4);
        tssClient.Send(packet, packet.Length, tssEndPoint);
    }

    // SENDING
            // CAMERA FEED
            // WAYPOINTS
            // MESSAGES
            // LIVE EV LOCATIONS
            // GEOSAMPLES
    public void SendToPRTeam(string type, string message)
    {
        // message is already a JSON string, so we don't need to serialize it again
        string jsonString = $"{{\"type\":\"{type}\",\"message\":{message}}}";
        // Convert the JSON string to a byte array
        byte[] dataBytes = Encoding.UTF8.GetBytes(jsonString);
        prClient.Send(dataBytes, dataBytes.Length, prEndPoint);
    }


    private void OnReceiveTSSUDP(IAsyncResult ar)
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = tssClient.EndReceive(ar, ref remoteEP);

      // Handle TSS response (header + float)
        if (data.Length >= 12)
        {
            uint timestamp = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, 0));
            uint command = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, 4));
            float value = BitConverter.ToSingle(data, 8);
            OnTSSDataReceived?.Invoke(timestamp, command, value);
        }

        tssClient.BeginReceive(OnReceiveTSSUDP, null);
    }


    private void OnReceivePRUDP(IAsyncResult ar)
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = prClient.EndReceive(ar, ref remoteEP);

        // Handle PR message (assume UTF-8 string)
        string message = Encoding.UTF8.GetString(data);
        OnPRDataReceived.Invoke(message);

        prClient.BeginReceive(OnReceivePRUDP, null);
    }
}

