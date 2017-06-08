using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ServerManager : MonoBehaviour {


    public int myPort = 54231;
    public string myIpString;
    private IPAddress localAddr;

    private TcpListener listener = null;
    private TcpClient client = null;
    private NetworkStream stream = null;

	// Use this for initialization
	void Start () {
        GetIPAddress();
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
    }

    // Accept a pending connection request
    private void AcceptConnection()
    {
        client = listener.AcceptTcpClient();
        stream = client.GetStream();
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
        byte[] buf = new byte[5];

        if (stream.Read(buf, 0, 5) != 5){
            throw new NetworkException("Failed to read packet header");
        }

        NetworkHelper.PacketType pType = (NetworkHelper.PacketType) buf[0];
        int pSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buf, 1));
    }
}
