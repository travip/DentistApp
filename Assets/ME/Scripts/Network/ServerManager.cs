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

    public Texture2D myImage;

    // Use this for initialization
    private void Awake()
    {
        GetIPAddress();
        client = new TcpClient(myIpString, 54321);
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
        Debug.Log("Reading packet!");
        byte[] buf = new byte[4];
        int numBytes = stream.Read(buf, 0, 4);
        Debug.Log("Read " + numBytes + " bytes!");
        if (numBytes != 4)
        {
            Debug.Log("Failed to read packet header");
        }
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(buf);
        }
        int picSize = (int)BitConverter.ToUInt32(buf, 0);
        Debug.Log("Picture Size is " + picSize);
        byte[] pic = new byte[picSize];
        if (picSize < 5000000)
        {
            numBytes = stream.Read(pic, 0, picSize);
            Debug.Log("Read " + numBytes + " bytes");
            myImage.LoadImage(pic);
        }
        else
        {
            Debug.Log("Picsize error");
            pic = null;
            var buffer = new byte[1000000];
            while (stream.DataAvailable)
            {
                int x = stream.Read(buffer, 0, buffer.Length);
                Debug.Log("Clearing stream of " + x + " bytes");
            }
        }        
    }

    public void WriteData()
    {
        byte[] buf = Encoding.ASCII.GetBytes("1234567890");
        stream.Write(buf, 0, buf.Length);
    }

    
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
    
}
