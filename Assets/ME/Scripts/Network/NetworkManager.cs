using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UdpState
{
    public IPEndPoint e;
    public UdpClient u;
}

public class NetworkManager : MonoBehaviour {

    public NetworkManager instance;

    public int myPort = 12345;
    public int discoverPort = 52524;
    public int dataPort = 56567;
    public string myIpString;

    private IPAddress localAddr;
    private IPEndPoint broadcastAddr;

    private IPEndPoint currentAddr;
    private UdpState udpState;
    private bool isConnected = false;

    private TcpClient tcpClient = null;
    private NetworkStream tcpStream = null;
    private UdpClient udpClient = null;

    public Texture2D myImage;

    private byte[] pic;

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

        GetIPAddress();
        pic = new byte[4];
        udpClient = new UdpClient(myPort);
        udpClient.EnableBroadcast = true;
        broadcastAddr = new IPEndPoint(IPAddress.Parse("255.255.255.255"), discoverPort);
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

    public void CloseConnection()
    {
        if(tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
        }
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
            case PacketType.IMAGE_CAPTURE:
                Array.Resize(ref pic, size);
                if (0 < size && size < 5000000)
                {
                    numBytes = tcpStream.Read(pic, 0, size);
                    //Debug.Log("Read " + numBytes + " bytes");
                    myImage.LoadImage(pic);
                }
                else
                {
                    FlushStream();
                }
                break;
        }      
    }

    public void ReceiveCallback(IAsyncResult ar)
    {
        UdpClient u = ((UdpState)(ar.AsyncState)).u;
        IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

        byte[] recv = u.EndReceive(ar, ref e);
        Debug.Log(e.Address.ToString() + ":" + e.Port.ToString());
        Debug.Log(Encoding.ASCII.GetString(recv));
        Thread.Sleep(1000);
        tcpClient = new TcpClient(e.Address.ToString(), dataPort);
        tcpStream = tcpClient.GetStream();
        Debug.Log("Connected!");
        isConnected = true;
    }

    public void BeginDiscovery()
    {
        Debug.Log("Discovering..");
        udpClient.Send(BitConverter.GetBytes(true), 1, broadcastAddr);

        IPEndPoint e = new IPEndPoint(IPAddress.Any, myPort);
        UdpState s = new UdpState();
        s.e = e;
        s.u = udpClient;

        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), s);
    }
    
    private void Update()
    {
        if (isConnected)
        {
            if (tcpStream.DataAvailable)
            {
                //Debug.Log("Read a packet!");
                ReadPacket();
            }
        }
    }
    
}
