using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;

public class PIPSensor : MonoBehaviour {

    public int appPort = 12345;
    public int myPort = 44344;

    public long datagramCount = 0;

    public string myIpString;

    private UdpClient udpClient;
    private UdpState udpState;

    private IPAddress localAddr;

    private IPEndPoint broadcastAddr;
    private IPEndPoint currentListenAddr;
    private IPEndPoint targetAddr;
    private IPEndPoint any;

    private bool pipConnected = false;
    private bool sending = false;
    private float timeSinceDiscover;

    public Text displayText;
    public Text countText;
    public Text connText;
    public string currentMsg = "None";

    // Use this for initialization
    void Start ()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        GetIPAddress();
        any = new IPEndPoint(IPAddress.Any, appPort);
        udpClient = new UdpClient(myPort);
        udpClient.EnableBroadcast = true;
		broadcastAddr = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 12345);

		currentListenAddr = any;

        UdpState s = new UdpState();
        s.e = currentListenAddr;
        s.u = udpClient;

        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), s);

        timeSinceDiscover = 0f;
    }

    private void ReportMessage()
    {
        displayText.text = currentMsg;
        countText.text = datagramCount.ToString();
        connText.text = "(Connected, Sending)\n(" + pipConnected.ToString() + ", " + sending.ToString() + ")";
    }
	
    private bool GetIPAddress()
    {
        try
        {
            myIpString = NetworkHelper.GetLocalIPAddress();
        }
        catch (NetworkException e)
        {
            Debug.Log(e);
            return false;
        }
        localAddr = IPAddress.Parse(myIpString);
        return true;
    }

    public void ReceiveCallback(IAsyncResult ar)
    {
        UdpClient u = ((UdpState)(ar.AsyncState)).u;
        IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

        byte[] recv = u.EndReceive(ar, ref e);

        switch (recv[0])
        {
            case PacketType.PIP_DISCOVERY:
                if (!pipConnected)
                {
                    currentMsg = "Got a reply from headset";
                    targetAddr = e;
                    currentListenAddr = e;
                    pipConnected = true;
                }
                break;
            case PacketType.PIP_REJECT:
                currentMsg = "Rejected for connection";
                break;
            case PacketType.PIP_DISCONNECT:
                currentMsg = "Pip disconnected";
                currentListenAddr = any;
                pipConnected = false;
                sending = false;
                break;
            case PacketType.PIP_START:
                currentMsg = "Pip STARTING";
                sending = true;
                break;
            case PacketType.PIP_STOP:
                currentMsg = "Pip STOPPING";
                sending = false;
                break;
            case PacketType.PIP_GYRODATA:
                currentMsg = "gyrodata";
                break;
            default:
                currentMsg = "Unknown datagram";
                break;
        }
        // Get next UDP Datagram
        datagramCount++;
        UdpState s = new UdpState();
        s.e = currentListenAddr;
        s.u = udpClient;
        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), s);
    }

    private void SendGyroData()
    {
        byte[] msg = Encoding.ASCII.GetBytes(Input.gyro.attitude.x.ToString() + ":" +
                                             Input.gyro.attitude.y.ToString() + ":" +
                                             Input.gyro.attitude.z.ToString() + ":" +
                                             Input.gyro.attitude.w.ToString());
        msg = NetworkHelper.CreateDatagram(PacketType.PIP_GYRODATA, msg);

        udpClient.Send(msg, msg.Length, targetAddr);
    }

    public void BeginDiscovery()
    {
        if (!pipConnected)
        {
            currentMsg = "Discovering headset..";
            byte[] msg = NetworkHelper.CreateDatagram(PacketType.PIP_DISCOVERY, null);
            udpClient.Send(msg, msg.Length, any);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pipConnected && sending)
        {
            SendGyroData();
        }

        ReportMessage();
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
