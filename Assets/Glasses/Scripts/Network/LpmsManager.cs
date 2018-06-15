
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using LPMSB2;
using TechTweaking.Bluetooth;
using UnityEngine.UI;

public class LpmsManager : MonoBehaviour
{

    public static LpmsManager instance;

    LpmsB2 Sensor;
    public Text sensorText;
    public Text sensorName;
    public bool gotAck;
    public bool gotNack;
    int cnt = 0;


    private BluetoothDevice device;

    public Quaternion sensorOrientation;

    private readonly byte[] resetOrientation = new byte[] { 0x3A, 0x01, 0x00, 0x52, 0x00, 0x00, 0x00, 0x53, 0x00, 0x0D, 0x0A };
    private readonly byte[] setObjectOrientation = new byte[] { 0x3A, 0x01, 0x00, 0x12, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x13, 0x00, 0x0D, 0x0A };
    private readonly byte[] setHeadingOrientation = new byte[] { 0x3A, 0x01, 0x00, 0x12, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00, 0x14, 0x00, 0x0D, 0x0A };
    private readonly byte[] setAlignmentOrientation = new byte[] { 0x3A, 0x01, 0x00, 0x12, 0x00, 0x04, 0x00, 0x02, 0x00, 0x00, 0x00, 0x15, 0x00, 0x0D, 0x0A };
    private readonly byte[] setCommandMode = new byte[] { 0x3A, 0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x07, 0x00, 0x0D, 0x0A };
    private readonly byte[] setStreamMode = new byte[] { 0x3A, 0x01, 0x00, 0x07, 0x00, 0x00, 0x00, 0x08, 0x00, 0x0D, 0x0A };

    void Awake()
    {
        if(instance == null)
            instance = this;
        BluetoothAdapter.askEnableBluetooth();
        BluetoothAdapter.OnDeviceOFF += HandleOnDeviceOff;
        BluetoothAdapter.OnDevicePicked += HandleOnDevicePicked;
        BluetoothAdapter.OnDisconnected += HandleOnDisconnected;
        BluetoothAdapter.OnReadingStarted += HandleOnReadingStarted;
        BluetoothAdapter.OnReadingStoped += HandleOnReadingStopped;
    }

    private void Start()
    {
        Sensor = new LpmsB2(this);
    }

    void HandleOnDeviceOff(BluetoothDevice dev)
    {
        if(!string.IsNullOrEmpty(dev.Name))
            sensorText.text = dev.Name + ": Unable to connect";
        else if(!string.IsNullOrEmpty(dev.MacAddress))
        {
            sensorText.text = dev.MacAddress + ": Unable to connect";
        }
    }

    void HandleOnDevicePicked(BluetoothDevice device)
    {
        this.device = device;
        device.ReadingCoroutine = ManageConnection;
        InputManager.instance.ResetCamera();
        sensorText.text = device.Name + ": Sensor Idle";
        sensorName.text = device.Name;
    }

    void HandleOnDisconnected(BluetoothDevice device)
    {
        PIPController.instance.pipSending = false;
    }

    void HandleOnReadingStarted( BluetoothDevice device)
    {
        sensorText.text = device.Name + ": Reading";
        PIPController.instance.pipSending = true;
    }

    void HandleOnReadingStopped(BluetoothDevice device)
    {
        sensorText.text = device.Name + ": Stopped";
        PIPController.instance.pipSending = false;
    }
    public void showDevices()
    {
        BluetoothAdapter.showDevices();
    }

    public void Connect()
    {
        if(device != null)
        {
            device.connect();
            device.send(setStreamMode);
            sensorText.text = device.Name +  ": Connecting";
        }
    }

    public void Disconnect()
    {
        if(device != null)
            device.close();
    }

    IEnumerator ManageConnection(BluetoothDevice device)
    {

        while(device.IsReading)
        {

            byte[] msg = device.read();

            if(msg != null)
            {
                Sensor.nBytes = msg.Length;
                Sensor.rawRxBuffer = msg;
                Sensor.parse();
            }
            yield return null;
        }
    }

    public void CheckPacket(int func, int length, float[] quat)
    {
        // REPLY_ACK
        if(func == 0)
        {
            gotAck = true;
            //sensorText.text = "GOT AN ACK!";
        }
        // REPLY_NACK
        else if(func == 1)
        {
            gotNack = true;
            //sensorText.text = "GOT A NACK";
        }
        else
            sensorOrientation = new Quaternion(quat[0], quat[1], quat[2], quat[3]);
    }

    public void ResetOrientation()
    {
        // Reset Orientation packet
        cnt++;
        StartCoroutine(ChangeModes(resetOrientation));
    }

    public void ResetOrientationObject()
    {
        // Reset Orientation packet
        cnt++;
        StartCoroutine(ChangeModes(setObjectOrientation));
    }

    public void ResetOrientationHeading()
    {
        // Reset Orientation packet
        cnt++;
        StartCoroutine(ChangeModes(setHeadingOrientation));
    }

    public void ResetOrientationAlignment()
    {
        // Reset Orientation packet
        cnt++;
        StartCoroutine(ChangeModes(setAlignmentOrientation));
    }

    IEnumerator ChangeModes(byte[] cmd)
    {
        gotAck = false;
        device.send(setCommandMode);
        yield return new WaitForAck();
        if(gotAck == true)
        {
            gotAck = false;
            device.send(cmd);
            yield return new WaitForAck();
            if(gotAck == false)
                sensorText.text = "Failed to reset orientation!";
            else
            {
                gotAck = false;
                sensorText.text = "Reset Orientation! " + cnt;
            }
        }
        else
        {
            sensorText.text = "Failed to reset orientation!";
        }
        device.send(setStreamMode);
        yield return new WaitForAck();
        gotAck = false;

    }

    public void ShowAddress(int a)
    {
        sensorText.text = "ADDRESS = " + a;
    }

    void OnDestroy()
    {
        Disconnect();
        BluetoothAdapter.OnDevicePicked -= HandleOnDevicePicked;
        BluetoothAdapter.OnDeviceOFF -= HandleOnDeviceOff;
    }
}