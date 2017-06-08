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
    public const byte CameraStream = 0x01;
}
