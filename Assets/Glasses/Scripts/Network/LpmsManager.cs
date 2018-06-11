
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

    private BluetoothDevice device;

    public Quaternion sensorOrientation;

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
    }

    void HandleOnDisconnected(BluetoothDevice device)
    {
        PIPController.instance.pipSending = false;
    }

    void HandleOnReadingStarted( BluetoothDevice device)
    {
        PIPController.instance.pipSending = true;
    }

    void HandleOnReadingStopped(BluetoothDevice device)
    {
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

    public void CheckPacket(int length, float[] quat)
    {
        sensorOrientation = new Quaternion(quat[0], quat[1], quat[2], quat[3]);
    }

    void OnDestroy()
    {
        BluetoothAdapter.OnDevicePicked -= HandleOnDevicePicked;
        BluetoothAdapter.OnDeviceOFF -= HandleOnDeviceOff;
    }
}