using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections;

namespace LPMSB2
{
    public class LpmsB2 
    {
        //
        //////////////////////////////////////////////
        // Stream frequency enable bits
        //////////////////////////////////////////////
        public const int LPMS_STREAM_FREQ_5HZ = 5;
        public const int LPMS_STREAM_FREQ_10HZ = 10;
        public const int LPMS_STREAM_FREQ_25HZ = 25;
        public const int LPMS_STREAM_FREQ_50HZ = 50;
        public const int LPMS_STREAM_FREQ_100HZ = 100;
        public const int LPMS_STREAM_FREQ_200HZ = 200;
        public const int LPMS_STREAM_FREQ_400HZ = 400;

        public const int LPMS_FILTER_GYR = 0;
        public const int LPMS_FILTER_GYR_ACC = 1;
        public const int LPMS_FILTER_GYR_ACC_MAG = 2;
        public const int LPMS_FILTER_DCM_GYR_ACC = 3;
        public const int LPMS_FILTER_DCM_GYR_ACC_MAG = 4;
        //Magnetometer correction
        public const int LPMS_FILTER_PRESET_DYNAMIC = 0;
        public const int LPMS_FILTER_PRESET_STRONG = 1;
        public const int LPMS_FILTER_PRESET_MEDIUM = 2;
        public const int LPMS_FILTER_PRESET_WEAK = 3;
        //Low Pass Filter
        public const int LPMS_LOW_FILTER_OFF = 0;
        public const int LPMS_LOW_FILTER_40HZ = 1;
        public const int LPMS_LOW_FILTER_20HZ = 2;
        public const int LPMS_LOW_FILTER_4HZ = 3;
        public const int LPMS_LOW_FILTER_2HZ = 4;
        public const int LPMS_LOW_FILTER_04HZ = 5;
        // Gyro Range
        public const int LPMS_GYR_RANGE_125DPS = 125;
        public const int LPMS_GYR_RANGE_245DPS = 245;
        public const int LPMS_GYR_RANGE_250DPS = 250;
        public const int LPMS_GYR_RANGE_500DPS = 500;
        public const int LPMS_GYR_RANGE_1000DPS = 1000;
        public const int LPMS_GYR_RANGE_2000DPS = 2000;
        // Acc Range
        public const int LPMS_ACC_RANGE_2G = 2;
        public const int LPMS_ACC_RANGE_4G = 4;
        public const int LPMS_ACC_RANGE_8G = 8;
        public const int LPMS_ACC_RANGE_16G = 16;
        // Mag Range
        public const int LPMS_MAG_RANGE_4GAUSS = 4;
        public const int LPMS_MAG_RANGE_8GAUSS = 8;
        public const int LPMS_MAG_RANGE_12GAUSS = 12;
        public const int LPMS_MAG_RANGE_16GAUSS = 16;
        public const int PARAMETER_SET_DELAY = 1;
        // Connection status
        public const int SENSOR_STATUS_CONNECTED = 1;
        public const int SENSOR_STATUS_CONNECTING = 2;
        public const int SENSOR_STATUS_DISCONNECTED = 3;
        public const int SENSOR_STATUS_ERROR = 4;
        // Offset mode
        public const int LPMS_OFFSET_MODE_OBJECT = 0;
        public const int LPMS_OFFSET_MODE_HEADING = 1;
        public const int LPMS_OFFSET_MODE_ALIGNMENT = 2;

        const String TAG = "LpmsB2";
        const int PACKET_ADDRESS0 = 0;
        const int PACKET_ADDRESS1 = 1;
        const int PACKET_FUNCTION0 = 2;
        const int PACKET_FUNCTION1 = 3;
        const int PACKET_LENGTH0 = 4;
        const int PACKET_LENGTH1 = 5;
        const int PACKET_RAW_DATA = 6;
        const int PACKET_LRC_CHECK0 = 7;
        const int PACKET_LRC_CHECK1 = 8;
        const int PACKET_END = 9;

        //////////////////////////////////////////////
        // Command Registers
        //////////////////////////////////////////////
        const int REPLY_ACK = 0;
        const int REPLY_NACK = 1;
        const int UPDATE_FIRMWARE = 2;
        const int UPDATE_IAP = 3;
        const int GET_CONFIG = 4;
        const int GET_STATUS = 5;
        const int GOTO_COMMAND_MODE = 6;
        const int GOTO_STREAM_MODE = 7;
        const int GET_SENSOR_DATA = 9;
        const int SET_TRANSMIT_DATA = 10;
        const int SET_STREAM_FREQ = 11;
        // Register value save and reset
        const int WRITE_REGISTERS = 15;
        const int RESTORE_FACTORY_VALUE = 16;
        // Reference setting and offset reset
        const int RESET_REFERENCE = 17;
        const int SET_ORIENTATION_OFFSET = 18;
        const int RESET_ORIENTATION_OFFSET = 82;
        //IMU ID setting
        const int SET_IMU_ID = 20;
        const int GET_IMU_ID = 21;
        // Gyroscope settings
        const int START_GYR_CALIBRA = 22;
        const int ENABLE_GYR_AUTOCAL = 23;
        const int ENABLE_GYR_THRES = 24;
        const int SET_GYR_RANGE = 25;
        const int GET_GYR_RANGE = 26;
        // Accelerometer settings
        const int SET_ACC_BIAS = 27;
        const int GET_ACC_BIAS = 28;
        const int SET_ACC_ALIGN_MATRIX = 29;
        const int GET_ACC_ALIGN_MATRIX = 30;
        const int SET_ACC_RANGE = 31;
        const int GET_ACC_RANGE = 32;
        const int SET_GYR_ALIGN_BIAS = 48;
        const int GET_GYR_ALIGN_BIAS = 49;
        const int SET_GYR_ALIGN_MATRIX = 50;
        const int GET_GYR_ALIGN_MATRIX = 51;
        // Magnetometer settings
        const int SET_MAG_RANGE = 33;
        const int GET_MAG_RANGE = 34;
        const int SET_HARD_IRON_OFFSET = 35;
        const int GET_HARD_IRON_OFFSET = 36;
        const int SET_SOFT_IRON_MATRIX = 37;
        const int GET_SOFT_IRON_MATRIX = 38;
        const int SET_FIELD_ESTIMATE = 39;
        const int GET_FIELD_ESTIMATE = 40;
        const int SET_MAG_ALIGNMENT_MATRIX = 76;
        const int SET_MAG_ALIGNMENT_BIAS = 77;
        const int SET_MAG_REFRENCE = 78;
        const int GET_MAG_ALIGNMENT_MATRIX = 79;
        const int GET_MAG_ALIGNMENT_BIAS = 80;
        const int GET_MAG_REFERENCE = 81;
        // Filter settings
        const int SET_FILTER_MODE = 41;
        const int GET_FILTER_MODE = 42;
        const int SET_FILTER_PRESET = 43;
        const int GET_FILTER_PRESET = 44;
        const int SET_LIN_ACC_COMP_MODE = 67;
        const int GET_LIN_ACC_COMP_MODE = 68;

        //////////////////////////////////////////////
        // Status register contents
        //////////////////////////////////////////////
        const int SET_CENTRI_COMP_MODE = 69;
        const int GET_CENTRI_COMP_MODE = 70;
        const int SET_RAW_DATA_LP = 60;
        const int GET_RAW_DATA_LP = 61;
        const int SET_TIME_STAMP = 66;
        const int SET_LPBUS_DATA_MODE = 75;
        const int GET_FIRMWARE_VERSION = 47;
        const int GET_BATTERY_LEVEL = 87;
        const int GET_BATTERY_VOLTAGE = 88;
        const int GET_CHARGING_STATUS = 89;
        const int GET_SERIAL_NUMBER = 90;
        const int GET_DEVICE_NAME = 91;
        const int GET_FIRMWARE_INFO = 92;
        const int START_SYNC = 96;
        const int STOP_SYNC = 97;
        const int GET_PING = 98;
        const int GET_TEMPERATURE = 99;

        //////////////////////////////////////////////
        // Configuration register contents
        //////////////////////////////////////////////
        public const int LPMS_GYR_AUTOCAL_ENABLED = 0x00000001 << 30;
        public const int LPMS_LPBUS_DATA_MODE_16BIT_ENABLED = 0x00000001 << 22;
        public const int LPMS_LINACC_OUTPUT_ENABLED = 0x00000001 << 21;
        public const int LPMS_DYNAMIC_COVAR_ENABLED = 0x00000001 << 20;
        public const int LPMS_ALTITUDE_OUTPUT_ENABLED = 0x00000001 << 19;
        public const int LPMS_QUAT_OUTPUT_ENABLED = 0x00000001 << 18;
        public const int LPMS_EULER_OUTPUT_ENABLED = 0x00000001 << 17;
        public const int LPMS_ANGULAR_VELOCITY_OUTPUT_ENABLED = 0x00000001 << 16;
        public const int LPMS_GYR_CALIBRA_ENABLED = 0x00000001 << 15;
        public const int LPMS_HEAVEMOTION_OUTPUT_ENABLED = 0x00000001 << 14;
        public const int LPMS_TEMPERATURE_OUTPUT_ENABLED = 0x00000001 << 13;
        public const int LPMS_GYR_RAW_OUTPUT_ENABLED = 0x00000001 << 12;
        public const int LPMS_ACC_RAW_OUTPUT_ENABLED = 0x00000001 << 11;
        public const int LPMS_MAG_RAW_OUTPUT_ENABLED = 0x00000001 << 10;
        public const int LPMS_PRESSURE_OUTPUT_ENABLED = 0x00000001 << 9;
        const int LPMS_STREAM_FREQ_5HZ_ENABLED = 0x00000000;
        const int LPMS_STREAM_FREQ_10HZ_ENABLED = 0x00000001;
        const int LPMS_STREAM_FREQ_25HZ_ENABLED = 0x00000002;
        const int LPMS_STREAM_FREQ_50HZ_ENABLED = 0x00000003;
        const int LPMS_STREAM_FREQ_100HZ_ENABLED = 0x00000004;
        const int LPMS_STREAM_FREQ_200HZ_ENABLED = 0x00000005;
        const int LPMS_STREAM_FREQ_400HZ_ENABLED = 0x00000006;
        const int LPMS_STREAM_FREQ_MASK = 0x00000007;

        const int MAX_BUFFER = 2048;
        const int DATA_QUEUE_SIZE = 64;

        int imuId = 1;

        // Protocol parsing related
        int rxState = PACKET_END;
        byte[] rxBuffer = new byte[MAX_BUFFER];
        byte[] txBuffer = new byte[MAX_BUFFER];
        byte[] rawTxData = new byte[MAX_BUFFER];
        public byte[] rawRxBuffer = new byte[MAX_BUFFER];

        int currentAddress = 0;
        int currentFunction = 0;
        int currentLength = 0;
        int rxIndex = 0;
        byte b = 0;
        int lrcCheck = 0;
        public int nBytes = 0;
        public bool waitForAck = false;
        bool waitForData = false;
        byte[] inBytes = new byte[2];

        bool newDataFlag = false;
        Queue<LpmsData> dataQueue = new Queue<LpmsData>(DATA_QUEUE_SIZE);
        private Object lockObject = new Object();
        LpmsData mLpmsData = new LpmsData();
        int frameCounter = 0;

        Thread connectionThread;
        Thread dataReadThread;

        LpmsConnection conn;

        public class CommandParameter
        {

            public int commandRegisters;
            public int target;

            public CommandParameter(int command, int tar)
            {
                this.commandRegisters = command;
                this.target = tar;
            }
        }

        public LpmsB2(LpmsConnection conn)
        {
            this.conn = conn;
        }

        public void resetOrientationOffset()
        {
            lpbusSetNone(RESET_ORIENTATION_OFFSET);
        }

        public void setOrientationOffset(int offset)
        {
            if(offset == LPMS_OFFSET_MODE_ALIGNMENT ||
                    offset == LPMS_OFFSET_MODE_HEADING ||
                    offset == LPMS_OFFSET_MODE_OBJECT)
            {
                lpbusSetInt32(SET_ORIENTATION_OFFSET, offset);
            }
        }

        void lpbusSetNone(int command)
        {
            sendData(command, 0);
        }

        void lpbusSetData(int command, int length, byte[] dataBuffer)
        {
            for(int i = 0; i < length; ++i)
            {
                rawTxData[i] = dataBuffer[i];
            }
            sendData(command, length);
        }

        void lpbusSetInt32(int command, int v)
        {
            for(int i = 0; i < 4; ++i)
            {
                rawTxData[i] = (byte) (v & 0xff);
                v = v >> 8;
            }
            sendData(command, 4);
        }

        public void setCommandMode()
        {
            waitForAck = true;
            lpbusSetNone(GOTO_COMMAND_MODE);
        }

        public void setStreamingMode()
        {
            waitForAck = true;
            lpbusSetNone(GOTO_STREAM_MODE);
        }

        void sendData(int function, int length)
        {
            int txLrcCheck;

            txBuffer[0] = 0x3a;
            convertInt16ToTxbytes((Int16) imuId, 1, ref txBuffer);
            convertInt16ToTxbytes((Int16) function, 3, ref txBuffer);
            convertInt16ToTxbytes((Int16) length, 5, ref txBuffer);

            for(int i = 0; i < length; ++i)
            {
                txBuffer[7 + i] = rawTxData[i];
            }

            txLrcCheck = (imuId & 0xffff) + (function & 0xffff) + (length & 0xffff);

            for(int j = 0; j < length; j++)
            {
                txLrcCheck += (int) rawTxData[j] & 0xff;
            }

            convertInt16ToTxbytes((Int16) txLrcCheck, 7 + length, ref txBuffer);
            txBuffer[9 + length] = 0x0d;
            txBuffer[10 + length] = 0x0a;

            byte[] sendBuf = new byte[length + 11];
            Array.Copy(txBuffer, sendBuf, length + 11);
            conn.SendCommand(sendBuf);
        }

        public void parse()
        {
            int lrcReceived = 0;
            for(int i = 0; i < nBytes; i++)
            {
                b = rawRxBuffer[i];

                switch(rxState)
                {
                    case PACKET_END:
                        if(b == 0x3a)
                        {
                            rxState = PACKET_ADDRESS0;
                        }
                        break;

                    case PACKET_ADDRESS0:
                        inBytes[0] = b;
                        rxState = PACKET_ADDRESS1;
                        break;

                    case PACKET_ADDRESS1:
                        inBytes[1] = b;
                        currentAddress = convertRxbytesToInt16(0, inBytes);
                        rxState = PACKET_FUNCTION0;
                        break;

                    case PACKET_FUNCTION0:
                        inBytes[0] = b;
                        rxState = PACKET_FUNCTION1;
                        break;

                    case PACKET_FUNCTION1:
                        inBytes[1] = b;
                        currentFunction = convertRxbytesToInt16(0, inBytes);
                        rxState = PACKET_LENGTH0;
                        break;

                    case PACKET_LENGTH0:
                        inBytes[0] = b;
                        rxState = PACKET_LENGTH1;
                        break;

                    case PACKET_LENGTH1:
                        inBytes[1] = b;
                        currentLength = convertRxbytesToInt16(0, inBytes);
                        rxState = PACKET_RAW_DATA;
                        rxIndex = 0;
                        break;

                    case PACKET_RAW_DATA:
                        if(rxIndex == currentLength)
                        {
                            lrcCheck = (currentAddress & 0xffff) + (currentFunction & 0xffff) + (currentLength & 0xffff);
                            for(int j = 0; j < currentLength; j++)
                            {
                                if(j < MAX_BUFFER)
                                {
                                    lrcCheck += (int) rxBuffer[j] & 0xff;
                                }
                                else
                                    break;
                            }
                            inBytes[0] = b;
                            rxState = PACKET_LRC_CHECK1;
                        }
                        else
                        {
                            if(rxIndex < MAX_BUFFER)
                            {
                                rxBuffer[rxIndex] = b;
                                rxIndex++;
                            }
                            else
                                break;
                        }
                        break;

                    case PACKET_LRC_CHECK1:
                        inBytes[1] = b;

                        lrcReceived = convertRxbytesToInt16(0, inBytes);
                        lrcCheck = lrcCheck & 0xffff;

                        if(lrcReceived == lrcCheck)
                        {
                            parseSensorData();
                        }
                        rxState = PACKET_END;
                        break;

                    default:
                        rxState = PACKET_END;
                        break;
                }
            }
        }

        private void parseSensorData16Bit()
        {
            int o = 0;
            float r2d = 57.2958f;

            mLpmsData.imuId = imuId;
            mLpmsData.timestamp = (float) convertRxbytesToInt(0, rxBuffer) * 0.0025f;

            o += 4;
            mLpmsData.frameNumber = frameCounter;
            frameCounter++;

            for(int i = 0; i < 4; ++i)
            {
                mLpmsData.quat[i] = (float) ((short) (((rxBuffer[o + 1]) << 8) | (rxBuffer[o + 0] & 0xff))) / 10000.0f;
                o += 2;
            }


            lock(lockObject)
            {
                if(dataQueue.Count < DATA_QUEUE_SIZE)
                    dataQueue.Enqueue(new LpmsData(mLpmsData));
                else
                {
                    dataQueue.Dequeue();
                    dataQueue.Enqueue(new LpmsData(mLpmsData));
                }
            }

            newDataFlag = true;
        }

        private void parseSensorData()
        {
            int o = 0;
            float r2d = 57.2958f;

            mLpmsData.imuId = imuId;
            mLpmsData.timestamp = (float) convertRxbytesToInt(o, rxBuffer) * 0.0025f;
            o += 4;

            mLpmsData.quat[0] = convertRxbytesToFloat(o, rxBuffer);
            o += 4;
            mLpmsData.quat[1] = convertRxbytesToFloat(o, rxBuffer);
            o += 4;
            mLpmsData.quat[2] = convertRxbytesToFloat(o, rxBuffer);
            o += 4;
            mLpmsData.quat[3] = convertRxbytesToFloat(o, rxBuffer);
            o += 4;

            conn.CheckPacket(currentFunction, currentLength, mLpmsData.quat);
        }

        //////////////////////////////////////////
        /// BitConverter function
        //////////////////////////////////////////

        private float convertRxbytesToFloat(int offset, byte[] buffer)
        {
            return BitConverter.ToSingle(buffer, offset);
        }

        private int convertRxbytesToInt(int offset, byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, offset);
        }

        private int convertRxbytesToInt16(int offset, byte[] buffer)
        {
            return BitConverter.ToInt16(buffer, offset);
        }

        private String convertRxbytesToString(int length, byte[] buffer)
        {
            byte[] buffer2 = new byte[length];
            Buffer.BlockCopy(buffer, 0, buffer2, 0, length);
            return System.Text.Encoding.UTF8.GetString(buffer2);
        }

        private void convertInt16ToTxbytes(Int16 v, int offset, ref byte[] buffer)
        {
            byte[] b = BitConverter.GetBytes(v);
            for(int i = 0; i < 2; i++)
            {
                buffer[i + offset] = b[i];
            }
        }
    }
}