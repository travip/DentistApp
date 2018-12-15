using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LPMSB2;
using TechTweaking.Bluetooth;

// This class owns the bluetooth connection to the LPMSB2 Sensor, 
// and handles the sending / receiving of messages
public class LpmsConnection : MonoBehaviour
{
    private BluetoothDevice device;
    private LpmsB2 sensor;

    // Indicates a received acknowledgement or negative acknowledgement packet
    private bool gotAck;
    private bool gotNack;
    private bool canSend = true;

    public Quaternion sensorOrientation;

    // Optionally pass in a explicit Bluetooth Device to use
    LpmsConnection(BluetoothDevice device)
    {
        BluetoothAdapter.askEnableBluetooth();
        BluetoothAdapter.OnDeviceOFF += HandleOnDeviceOff;
        BluetoothAdapter.OnDevicePicked += HandleOnDevicePicked;
        BluetoothAdapter.OnDisconnected += HandleOnDisconnected;
        BluetoothAdapter.OnReadingStarted += HandleOnReadingStarted;
        BluetoothAdapter.OnReadingStoped += HandleOnReadingStopped;
        this.device = device;
        // If no device is given, check for a saved device
        if (this.device == null)
            CheckForSavedDevice();
    }

    // Clean up events when this class is destroyed
    void OnDestroy()
    {
        Disconnect();
        BluetoothAdapter.OnDeviceOFF -= HandleOnDeviceOff;
        BluetoothAdapter.OnDevicePicked -= HandleOnDevicePicked;
        BluetoothAdapter.OnDisconnected -= HandleOnDisconnected;
        BluetoothAdapter.OnReadingStarted -= HandleOnReadingStarted;
        BluetoothAdapter.OnReadingStoped -= HandleOnReadingStopped;
    }

    // Check local storage for a saved MAC address
    private void CheckForSavedDevice()
    {
        string mac = PlayerPrefs.GetString("sensor_mac", "");
        if (mac == "")
            device = null;
        else
        {
            device = new BluetoothDevice();
            device.MacAddress = mac;
            device.Name = PlayerPrefs.GetString("sensor_name", "Unknown Name");
            device.ReadingCoroutine = ManageConnection;
            Debug.Log("Preferred device found: " + device.Name + ". Attempting to connect...");
            Connect();
        }
    }

    // ----- Device event handling -----
    void HandleOnDeviceOff(BluetoothDevice dev)
    {
        if (!string.IsNullOrEmpty(dev.Name))
            Debug.Log(dev.Name + ": Unable to connect");
        else if (!string.IsNullOrEmpty(dev.MacAddress))
        {
            Debug.Log(dev.MacAddress + ": Unable to connect");
        }
    }

    void HandleOnDevicePicked(BluetoothDevice device)
    {
        this.device = device;
        device.ReadingCoroutine = ManageConnection;
        InputManager.instance.ResetCamera();
        PlayerPrefs.SetString("sensor_mac", device.MacAddress);
        PlayerPrefs.SetString("sensor_name", device.Name);
        Debug.Log(device.Name + ": Selected. Saved as preferred device.");
    }

    void HandleOnDisconnected(BluetoothDevice device)
    {
        Debug.Log(device.Name + ": Disconnected");
    }

    void HandleOnReadingStarted(BluetoothDevice device)
    {
        Debug.Log(device.Name + ": Reading");
    }

    void HandleOnReadingStopped(BluetoothDevice device)
    {
        Debug.Log(device.Name + ": Stopped");
    }
    // ----- -----

    // Read streaming data from the device
    IEnumerator ManageConnection(BluetoothDevice device)
    {

        while (device.IsReading)
        {

            byte[] msg = device.read();

            if (msg != null)
            {
                sensor.nBytes = msg.Length;
                sensor.rawRxBuffer = msg;
                sensor.parse();
            }
            yield return null;
        }
    }

    // Send a command packet to the sensor
    public bool SendCommand(byte[] data)
    {
        if (device == null || canSend == false)
            return false;
        else
        {
            StartCoroutine(Command(data));
            return true;
        }
    }

    // Process of actually sending a command
    IEnumerator Command(byte[] data)
    {
        // Send the sensor a "Set to Command Mode" instruction
        gotAck = false;
        gotNack = false;
        canSend = false;
        device.send(LpmsCommands.setCommandMode);
        yield return new WaitForAck(this);

        // Send the actual command
        gotAck = false;
        gotNack = false;
        device.send(data);
        yield return new WaitForAck(this);

        // Return the sensor to Stream Modes
        gotAck = false;
        gotNack = false;
        device.send(LpmsCommands.setStreamMode);
        yield return new WaitForAck(this);

        canSend = true;
    }

    // Check the received packet (expect only ack, nack, and quaternion for now)
    public void CheckPacket(int func, int length, float[] quat)
    {
        // REPLY_ACK
        if (func == 0)
        {
            gotAck = true;
        }
        // REPLY_NACK
        else if (func == 1)
        {
            gotNack = true;
        }
        // Store the sensor orientation in local variable
        else
            sensorOrientation = new Quaternion(-quat[0], -quat[1], quat[2], quat[3]);
    }

    public void SetOrientationOffset(int offset)
    {
        sensor.setOrientationOffset(offset);
    }

    // Attempt to connect to the device
    public void Connect()
    {
        if (device != null)
        {
            device.connect();
            device.send(LpmsCommands.setStreamMode);
            Debug.Log(device.Name + ": Connecting");
        }
    }

    // Disconnect from the device
    public void Disconnect()
    {
        if (device != null)
            device.close();
    }

    // WaitForAck yield class - takes a LpmsConnection in the constructor
    private class WaitForAck : CustomYieldInstruction
    {
        LpmsConnection conn;
        public WaitForAck(LpmsConnection conn)
        {
            this.conn = conn;
        }

        public override bool keepWaiting
        {
            get
            {
                if (conn.gotAck == true)
                {
                    return false;
                }
                else if (conn.gotNack == true)
                {
                    return false;
                }
                else
                    return true;
            }
        }
    }
}
