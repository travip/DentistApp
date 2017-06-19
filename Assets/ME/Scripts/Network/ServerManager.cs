using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

// Packet container
internal class Packet
{
    internal byte packetType;
    internal int size;
    internal byte[] body;
}

public class ServerManager : MonoBehaviour {


    public int myPort = 54231;
    public string myIpString;
    private IPAddress localAddr;

    private TcpListener listener = null;
    private TcpClient client = null;
    private NetworkStream stream = null;

    // Use this for initialization
    private void Awake()
    {
        GetIPAddress();
        client = new TcpClient("192.168.1.115", 54321);
        stream = client.GetStream();
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

    // Begin listening for connections
    public void BeginListening()
    {
        // If listener exists, remove it
        if(listener != null)
        {
            Debug.Log("Listener exists");
            try
            {
                listener.Stop();
            }
            catch { }
            listener = null;
        }

        listener = new TcpListener(localAddr, myPort);
        listener.Start();
        Debug.Log("Began listening.");
    }

    // Accept a pending connection request
    private void AcceptConnection()
    {
        client = listener.AcceptTcpClient();
        stream = client.GetStream();
        Debug.Log("Accepted a connection");
    }

    public void CloseConnection()
    {
        if(client != null)
        {
            client.Close();
            client = null;
            stream = null;
        }
    }

    public void ReadPacket()
    {
        /*
        Packet p = new Packet();
        byte[] buf = new byte[5];

        if (stream.Read(buf, 0, 5) != 5){
            throw new NetworkException("Failed to read packet header");
        }

        p.packetType = buf[0];
        p.size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buf, 1));
        p.body = new byte[p.size];

        if(stream.Read(p.body, 0, p.size) != p.size)
        {
            throw new NetworkException("Failed to read packet body");
        }

        switch (p.packetType)
        {
            case PacketType.CameraStream:
                Debug.Log("Camera Stream Packet");
                break;
        }
    }
    */

        byte[] buf = new byte[1000000];
        int numBytes = stream.Read(buf, 0, 1000000);
        Debug.Log("Read " + numBytes + " bytes!");
    }

    public void WriteData()
    {
        byte[] buf = Encoding.ASCII.GetBytes("1234567890");
        stream.Write(buf, 0, buf.Length);
    }

    /*
    private void Update()
    {
        if (stream != null)
        {
            if (stream.DataAvailable)
            {
                Debug.Log("Read a packet!");
                ReadPacket();
            }
        }
    }
    */
}
