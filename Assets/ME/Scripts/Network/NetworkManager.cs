using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;

public class UdpState
{
    public IPEndPoint e;
    public UdpClient u;
}

public class NetworkManager : MonoBehaviour {

    public static NetworkManager instance { get; private set; }

    public int myPort = 12345;
    public int discoverPort = 52524;
    public int dataPort = 56567;
    public string myIpString;

    private IPAddress localAddr;
    private IPEndPoint broadcastAddr;

    private IPEndPoint pipAddr;
    private UdpState udpState;

    private IPEndPoint any;

    private bool discoveryingStream = false;
    private bool pipConnected = false;
    private bool camConnected = false;

    private TcpClient tcpClient = null;
    private NetworkStream tcpStream = null;
    private UdpClient udpClient = null;

    public Texture2D myImage;

    private byte[] pic;
    private byte[] gyroData;

    // Use this for initialization
    private void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        pic = new byte[5];
        gyroData = new byte[5];

        GetIPAddress();
        udpClient = new UdpClient(myPort);
        udpClient.EnableBroadcast = true;
        broadcastAddr = new IPEndPoint(IPAddress.Parse("255.255.255.255"), discoverPort);
        Debug.Log("Broadcast client on");

        any = new IPEndPoint(IPAddress.Any, myPort);
        UdpState s = new UdpState();
        s.e = any;
        s.u = udpClient;

        Debug.Log("Any: " + any.Address + ":" + any.Port);
        udpClient.BeginReceive(new AsyncCallback(ReceiveDatagram), s);
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

    private void FlushStream()
    {
        var buffer = new byte[10000];
        while (tcpStream.DataAvailable)
        {
            int x = tcpStream.Read(buffer, 0, buffer.Length);
            Debug.Log("Clearing stream of " + x + " bytes");
        }
    }
    
    public void ReadPacket()
    {
        byte[] pType = new byte[1];
        byte[] pSize = new byte[4];
        int numBytes = 0;

        // Process packet header
        if( (numBytes = tcpStream.Read(pType, 0, 1)) != 1)
        {
            Debug.Log("Failed to read packet type");
            FlushStream();
            return;
        }
        if ((numBytes = tcpStream.Read(pSize, 0, 4)) != 4)
        {
            Debug.Log("Failed to read packet size");
            FlushStream();
            return;
        }
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(pSize);
        }
        int size = (int)BitConverter.ToUInt32(pSize, 0);

        switch (pType[0])
        {
            case PacketType.CAM_IMAGE_CAPTURE:
                Array.Resize(ref pic, size);
                int bytesRead = 0;
                if (0 < size && size < 5000000)
                {
                    while (bytesRead < size)
                    {
                        try
                        {
                            numBytes = tcpStream.Read(pic, bytesRead, size - bytesRead);
                        }
                        catch(Exception e)
                        {
                            Debug.Log(e.ToString());
                            FlushStream();
                            return;
                        }
   
                        bytesRead += numBytes;
                        Debug.Log("Read " + numBytes + " bytes");
                    }
                    if(bytesRead != size)
                    {
                        Debug.Log("Read too many bytes?");
                    }
                    myImage.LoadImage(pic);
                }
                else
                {
                    FlushStream();
                }
                break;
        }      
    }

    public void StartPIPDataStream()
    {
        if (pipConnected)
        {
            byte[] msg = NetworkHelper.CreateDatagram(PacketType.PIP_START, null);
            udpClient.Send(msg, msg.Length, pipAddr);
            PIPController.instance.pipSending = true;
        }
        else
        {
            Debug.Log("StartPIP: PIP not connected");
        }

    }

    public void StopPIPDataStream()
    {
        if (pipConnected)
        {
            byte[] msg = NetworkHelper.CreateDatagram(PacketType.PIP_STOP, null);
            udpClient.Send(msg, msg.Length, pipAddr);
            PIPController.instance.pipSending = false;
        }
        else
        {
            Debug.Log("StartPIP: PIP not connected");
        }

    }
    public void DisconnectPIP()
    {
        if (pipConnected)
        {
            byte[] msg = NetworkHelper.CreateDatagram(PacketType.PIP_DISCONNECT, null);
            udpClient.Send(msg, msg.Length, pipAddr);
            pipAddr = null;
            pipConnected = false;
        }
        else
        {
            Debug.Log("StartPIP: PIP not connected");
        }
    }

    private void GetPIPGyroData(byte[] recv)
    {
        int size = BitConverter.ToInt32(recv, 1);
        size = IPAddress.NetworkToHostOrder(size);

        Array.Resize(ref gyroData, size);
        Array.Copy(recv, 5, gyroData, 0, size);

        string data = Encoding.ASCII.GetString(gyroData);
        string[] gyro = data.Split(':');

        Quaternion receivedQuart = new Quaternion(float.Parse(gyro[0]),
            float.Parse(gyro[1]),
            float.Parse(gyro[2]),
            float.Parse(gyro[3]));

        PIPController.instance.orientation = receivedQuart;
    }

    public void ReceiveDatagram(IAsyncResult ar)
    {
        UdpClient u = ((UdpState)(ar.AsyncState)).u;
        IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

        byte[] recv = u.EndReceive(ar, ref e);

        switch (recv[0])
        {
            case PacketType.CAM_DISCOVERY:
                if (discoveryingStream)
                {
                    Debug.Log("Found a streaming client");
                    Thread.Sleep(1000);
                    tcpClient = new TcpClient(e.Address.ToString(), dataPort);
                    tcpStream = tcpClient.GetStream();
                    Debug.Log("Connected to streaming client");
                    camConnected = true;
                }
                else
                    Debug.Log("Got CAM_DISCOVERY whilst not discovering");
                break;
            case PacketType.PIP_DISCOVERY:
                if (!pipConnected)
                {
                    Debug.Log("Pip attempting to connect");
                    byte[] reply = NetworkHelper.CreateDatagram(PacketType.PIP_DISCOVERY, null);
                    pipAddr = e;
                    udpClient.Send(reply, reply.Length, pipAddr);
                    pipConnected = true;
                }
                else
                {
                    byte[] reply = NetworkHelper.CreateDatagram(PacketType.PIP_REJECT, null);
                    udpClient.Send(reply, reply.Length, e);
                }
                break;
            case PacketType.PIP_DISCONNECT:
                Debug.Log("Pip disconnected");
                pipAddr = null;
                pipConnected = false;
                PIPController.instance.pipSending = false;
                break;
            case PacketType.PIP_GYRODATA:
                if(pipAddr.Address.ToString() == e.Address.ToString())
                    GetPIPGyroData(recv);
                break;
            default:
                Debug.Log("Unknown datagram");
                break;
        }
        // Get next UDP Datagram
        UdpState s = new UdpState();
        s.e = any;
        s.u = udpClient;
        udpClient.BeginReceive(new AsyncCallback(ReceiveDatagram), s);
    }

    public void BeginCamDiscovery()
    {
        Debug.Log("Discovering stream..");
        byte[] msg = NetworkHelper.CreateDatagram(PacketType.CAM_DISCOVERY, null);
        udpClient.Send(msg, msg.Length,broadcastAddr);
        discoveryingStream = true;
    }

    public void CloseCamConnection()
    {
        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
            tcpStream = null;
            camConnected = false;
        }
    }

    private void Update()
    {
        if (camConnected)
        {
            if (tcpStream.DataAvailable)
            {
                ReadPacket();
            }
        }
    }

    void OnApplicationQuit()
    {
        if(tcpClient != null)
            tcpClient.Close();
        if (pipConnected)
        {
            byte[] msg = NetworkHelper.CreateDatagram(PacketType.PIP_DISCONNECT, null);
            udpClient.Send(msg, msg.Length, pipAddr);
        }
        udpClient.Close();
    }
}
