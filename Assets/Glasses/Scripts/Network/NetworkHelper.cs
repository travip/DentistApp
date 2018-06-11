using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public static class NetworkHelper
{
    public static string GetLocalIPAddress()
    {
        if (NetworkInterface.GetIsNetworkAvailable())
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new NetworkException("Local IP Address Not Found!");
        }
        else
        {
            throw new NetworkException("Not connected to a network");
        }
    }

    public static byte[] FloatHostToNetworkOrder(float host)
    {
        byte[] bytes = BitConverter.GetBytes(host);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return bytes;
    }

    public static float FloatNetworkToHostOrder(int network)
    {
        byte[] bytes = BitConverter.GetBytes(network);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToSingle(bytes, 0);
    }

    public static byte[] CreateDatagram(byte pType, byte[] message)
    {
        int size;
        if (message == null)
            size = 0;
        else
            size = message.Length;

        byte[] packet = new byte[size + 5];
        size = IPAddress.HostToNetworkOrder(size);

        packet[0] = pType;
        BitConverter.GetBytes(size).CopyTo(packet, 1);
        if(message != null)
            message.CopyTo(packet, 5);

        return packet;
    }
}

public class NetworkException : Exception
{
    public NetworkException()
    {
    }

    public NetworkException(string message)
        : base(message)
    {
    }

    public NetworkException(string message, Exception inner)
        : base(message, inner)
    {
    }
    
}

public static class PacketType
{
    public const byte CAM_DISCOVERY = 0x01;
    public const byte CAM_DISCONNECT = 0x02;
    public const byte CAM_IMAGE_CAPTURE = 0x05;

    public const byte PIP_DISCOVERY = 0x11;
    public const byte PIP_DISCONNECT = 0x12;
    public const byte PIP_START = 0x13;
    public const byte PIP_STOP = 0x14;
    public const byte PIP_GYRODATA = 0x15;
    public const byte PIP_REJECT = 0x16;

}
