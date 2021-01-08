using MbitGate.control;
using System;
using System.Collections.Concurrent;

namespace MbitGate.helper
{
    class CommHexProtocolDecoder
    {
        private const byte HEADER = 0x55;
        private const byte FOOT = 0xAA;
        public const byte SUCCESS = 0x00;

        private const byte MinLength = 0x0B;

        public const byte ADDRESS_TARGET_0 = 0x00;
        public const byte ADDRESS_PUBLIC = 0xFF;

        public const byte ERROR_COMMON_ADDRESS = 0x01;
        public const byte ERROR_COMMON_FUNCTION = 0x02;
        public const byte ERROR_COMMON_LENGTH = 0x03;
        public const byte ERROR_COMMON_CRC = 0x04;
        public const byte ERROR_COMMON_OVERTIME = 0x05;
        public const byte ERROR_COMMON_UNKNOW = 0x06;

        public const byte ERROR_COMMON_UPDATE_ERASE = 0x014;
        public const byte ERROR_COMMON_UPDATE_PACKET_COUNT = 0x015;
        public const byte ERROR_COMMON_UPDATE_PACKET_ORDER = 0x016;
        public const byte ERROR_COMMON_UPDATE_PACKET_NUMBER = 0x017; 
        public const byte ERROR_COMMON_UPDATE_DATA_LENGTH = 0x018;
        public const byte ERROR_COMMON_UPDATE_DATA_WRITE = 0x019;
        public const byte ERROR_COMMON_UPDATE_CRC = 0x01A;
        public const byte ERROR_COMMON_UPDATE_FRESH = 0x01B; 
        public const byte ERROR_COMMON_UPDATE_OTHER = 0x01C;
        public const byte ERROR_COMMON_UPDATE_UNKNOW = 0x01D;

        public const byte ERROR_IT_CRC = 0x01;
        public const byte ERROR_IT_LACK_PARAMS = 0x02;
        public const byte ERROR_IT_STUDY = 0x03;
        public const byte ERROR_IT_PARAMS = 0x04;
        public const byte ERROR_IT_FUNCTION = 0x05;
        public const byte ERROR_IT_ADDRESS = 0x06;
        public const byte ERROR_IT_PRECONDITION = 0x07;

        public const ushort FUNCTION_COMMON_HARD_VER = 0x0001;
        public const ushort FUNCTION_COMMON_SOFT_VER = 0x0002;
        public const ushort FUNCTION_COMMON_SENSOR_STOP = 0x0003;
        public const ushort FUNCTION_COMMON_SENSOR_START = 0x0004;
        public const ushort FUNCTION_COMMON_SOFTRESET = 0x0005;
        public const ushort FUNCTION_COMMON_READCLI = 0x0006;

        public const ushort FUNCTION_IT_WRITECLI = 0x0101;
        public const ushort FUNCTION_IT_FALSEALARMORDER = 0x0102;
        public const ushort FUNCTION_IT_CLIOUTPUT = 0x0103;
        public const ushort FUNCTION_IT_READCLI = 0x0100;
        public const ushort FUNCTION_IT_GET_SYSTEM_TIME = 0x010A;
        public const ushort FUNCTION_IT_SET_SYSTEM_TIME = 0x0109;
        public const ushort FUNCTION_IT_CLEAR_RECORD = 0x010B;
        public const ushort FUNCTION_IT_GET_ROD_DIRECTION = 0x010C;
        public const ushort FUNCTION_IT_SET_ROD_DIRECTION = 0x010D;
        public const ushort FUNCTION_IT_OUT_DATA_POINT = 0x010E;
        public const ushort FUNCTION_IT_SEARCH = 0x0104;
        public const ushort FUNCTION_IT_RESET = 0x0105;
        public const ushort FUNCTION_IT_SWITCH = 0x0113;
        public const ushort FUNCTION_IT_HEART = 0x0114;
        public const ushort FUNCTION_IT_POINTS_STATE = 0x0115;
        public const ushort FUNCTION_IT_POINTS_DATA = 0x0116;
        public const ushort FUNCTION_IT_RADAR_STATE = 0x0117;
        public const ushort FUNCTION_IT_POINTS_STRONGEST = 0x0118;
        public const ushort FUNCTION_IT_POINTS_STRONGEST_STATE = 0x0119;

        public const ushort FUNCTION_UPDATE_TEST = 0x00F0;
        public const ushort FUNCTION_UPDATE_ERASE = 0x00F1;
        public const ushort FUNCTION_UPDATE_SIZE = 0x00F2;
        public const ushort FUNCTION_UPDATE_POSITION_DATA = 0x00F3;
        public const ushort FUNCTION_UPDATE_CRC = 0x00F4;
        public const ushort FUNCTION_UPDATE_DATA = 0x00F5;
        public const ushort FUNCTION_UPDATE_READ_FLASH = 0x00F6;
        public const ushort FUNCTION_UPDATE_REFRESH = 0x00F8;
        public const ushort FUNCTION_UPDATE_RESTART = 0x00F9;
        public const ushort FUNCTION_UPDATE_BOOLOADER = 0x0008;
        
        static byte[] Crc8_table = {
                0x93,0x98,0xE4,0x46,0xEB,0xBA,0x04,0x4C,
                0xFA,0x40,0xB8,0x96,0x0E,0xB2,0xB7,0xC0,
                0x0C,0x32,0x9B,0x80,0xFF,0x30,0x7F,0x9D,
                0xB3,0x81,0x58,0xE7,0xF1,0x19,0x7E,0xB6,
                0xCD,0xF7,0xB4,0xCB,0xBC,0x5C,0xD6,0x09,
                0x20,0x0A,0xE0,0x37,0x51,0x67,0x24,0x95,
                0xE1,0x62,0xF8,0x5E,0x38,0x15,0x54,0x77,
                0x63,0x57,0x6D,0xE9,0x89,0x76,0xBE,0x41,
                0x5D,0xF9,0xB1,0x4D,0x6C,0x53,0x9C,0xA2,
                0x23,0xC4,0x8E,0xC8,0x05,0x42,0x61,0x71,
                0xC5,0x00,0x18,0x6F,0x5F,0xFB,0x7B,0x11,
                0x65,0x2D,0x8C,0xED,0x14,0xAB,0x88,0xD5,
                0xD9,0xC2,0x36,0x34,0x7C,0x5B,0x3C,0xF6,
                0x48,0x0B,0xEE,0x02,0x83,0x79,0x17,0xE6,
                0xA8,0x78,0xF5,0xD3,0x4E,0x50,0x52,0x91,
                0xD8,0xC6,0x22,0xEC,0x3B,0xE5,0x3F,0x86,
                0x06,0xCF,0x2B,0x2F,0x3D,0x59,0x1C,0x87,
                0xEF,0x4F,0x10,0xD2,0x7D,0xDA,0x72,0xA0,
                0x9F,0xDE,0x6B,0x75,0x56,0xBD,0xC7,0xC1,
                0x70,0x1D,0x25,0x92,0xA5,0x31,0xE2,0xD7,
                0xD0,0x9A,0xAF,0xA9,0xC9,0x97,0x08,0x33,
                0x5A,0x99,0xC3,0x16,0x84,0x82,0x8A,0xF3,
                0x4A,0xCE,0xDB,0x29,0x0F,0xAE,0x6E,0xE3,
                0x8B,0x07,0x3A,0x74,0x47,0xB0,0xBB,0xB5,
                0x7A,0xA9,0x2C,0xD4,0x03,0x3E,0x1A,0xA7,
                0x27,0x64,0x06,0xBF,0x56,0x73,0x1E,0xFE,
                0x49,0x01,0x39,0x28,0xF4,0x26,0xDF,0xDD,
                0x44,0x0D,0x21,0xF2,0x85,0xB9,0xEA,0x4B,
                0xDC,0x6A,0xCA,0xAC,0x12,0xFC,0x2E,0x2A,
                0xA3,0xF0,0x66,0xE8,0x60,0x45,0xA1,0x8D,
                0x68,0x35,0xFD,0x8F,0x9E,0x1F,0x13,0xD1,
                0xAD,0x69,0xCC,0xA4,0x94,0x90,0x1B,0x43,
        };
        static byte Crc8(byte[] data)
        {
            byte crc = 0;
            foreach (byte val in data)
            {
                crc = Crc8_table[crc ^ val];
            }
            return crc;
        }
        internal static byte[] Code(byte address, byte error, ushort function, byte[] data)
        {
            byte[] message = new byte[2 + 1 + 1 + 2 + 2 + data.Length + 1 + 2];
            message[0] = HEADER;
            message[1] = HEADER;
            message[2] = address;
            message[3] = error;

            byte[] MsgData = new byte[1 + 1 + 2 + 2 + data.Length];
            MsgData[0] = address;
            MsgData[1] = error;
            Array.Copy(BitConverter.GetBytes(function), 0, MsgData, 2, 2);
            Array.Copy(BitConverter.GetBytes((ushort)data.Length), 0, MsgData, 4, 2);
            Array.Copy(data, 0, MsgData, 6, data.Length);

            Array.Copy(MsgData, 0, message, 2, MsgData.Length);
            message[message.Length - 1] = 0xAA;
            message[message.Length - 2] = 0xAA;
            message[message.Length - 3] = Crc8(MsgData);
            return message;
        }

        public Action NotifyCRCError { get; set; }
        public Action<ushort, byte> NotifyReplayError { get; set; }

        public Action<Tuple<byte/*address*/, byte/*error*/, ushort/*function*/, byte[]/*data*/>> NotifyDecodeResult { get; set; }
        public Action<byte[]> NotifyFullResult { get; set; }
        private void Decode(byte[] frame)
        {
            if(frame[0] == HEADER && frame[1] == HEADER && frame[frame.Length-1] == FOOT && frame[frame.Length-2] == FOOT)
            {
                byte[] data = new byte[frame.Length - 2 -2 -1];
                Array.Copy(frame, 2, data, 0, data.Length);
                if (CheckCRC(data , frame[frame.Length - 3]))
                {
                    if (data[1] != SUCCESS)
                    {
                        NotifyReplayError(BitConverter.ToUInt16(data, 2), data[1]);
                    }
                    else
                    {
                        byte[] values = new byte[data.Length - 6];
                        Array.Copy(data, 6, values, 0, values.Length);
                        NotifyDecodeResult?.Invoke(new Tuple<byte, byte, ushort, byte[]>(data[0], data[1], BitConverter.ToUInt16(data, 2), values));
                    }
                }
                else
                {
                    NotifyCRCError?.Invoke();
                }
            }
            else
            {
#if DEBUG
                throw new Exception("Frame Decode Error");
#else
                System.Console.Write("Frame Decode Error");
#endif
            }
        }

        private static bool CheckCRC(byte[] data, byte val)
        {
            return (val==0xFF)?true:(Crc8(data) == val);
        }

        private int decodeCount = 0;
        public int MaxCantDecodeCount = 100;
        internal void Decode(ref ConcurrentQueue<byte> data)
        {
            lock(data)
            {
                byte headerCount = 0, tmp = 0;
                int frameStart = 0, index = 0 ;
                if (data.Count >= GenerateLength(0))
                {
                    foreach (byte val in data)
                    {
                        index++;
                        if (val == HEADER)
                        {
                            if (headerCount == 0)
                                frameStart = index;
                            headerCount++;
                        }
                        if (headerCount == 0)
                        {
                            frameStart++;
                        }
                        else if (headerCount == 1)
                        {
                            if (index > (frameStart + 1))
                            {
                                headerCount = 0;
                            }
                        }
                        else if (headerCount == 2)
                        {
                            //找到HEADER后退出 认为可以进行帧解析
                            break;
                        }
                    }
                    //discard no use data in front
                    for (int i = 0; i < frameStart - 1; i++)
                    {
                        data.TryDequeue(out tmp);
                    }
                    index = 0;
                    byte lengthLow = 0x00, lengthHigh = 0x00;
                    int framelength = 0;
                    bool errorFrameFoot = false;
                    int frameEnd = -1;
                    foreach (byte val in data)
                    {
                        index++;
                        if (index == 7)
                            lengthLow = val;
                        else if (index == 8)
                        {
                            lengthHigh = val;
                        }
                        else if(index == 9)
                        {
                            framelength = GenerateLength((lengthHigh << 8) + lengthLow);
                            frameEnd = 2+1+1+2+2 + (lengthHigh << 8) + lengthLow + 1 + 1;
                        }
                        else if(index == frameEnd)
                        {
                            errorFrameFoot = (val != FOOT);
                        }
                        else if(index == (frameEnd+1))
                        {
                            errorFrameFoot = errorFrameFoot && (val != FOOT);
                            break;
                        }
                    }
                    if(index == (frameEnd+1))
                    {
                        if (errorFrameFoot)
                        {
                            data.TryDequeue(out tmp);
                        }
                        else
                        {
                            byte[] frame = new byte[framelength];
                            for (int i = 0; i < framelength; i++)
                            {
                                data.TryDequeue(out frame[i]);
                            }
                            System.Threading.Tasks.Task.Factory.StartNew(() => { Decode(frame); });
                            //Decode(frame);
                            decodeCount = 0;
                        }
                    }
                }

                //MaxCantDecodeCount次无法解析帧会丢弃掉当前数据
                decodeCount++;
                if (decodeCount > MaxCantDecodeCount)
                {
                    decodeCount = 0;
                    for (int i = 0; i < data.Count; i++)
                    {
                        data.TryDequeue(out tmp);
                    }
                }
            }
        }

        private int GenerateLength(int dataLen)
        {
            return 2/*header*/+ 1/*address*/+ 1/*error*/+ 2/*function*/+ 2/*length*/+ dataLen + 1/*crc*/+ 2/*foot*/;
        }

        internal static Tuple<byte, byte, ushort, byte[]> DecodeFrame(byte[] frame)
        {
            if (frame[0] == HEADER && frame[1] == HEADER && frame[frame.Length - 1] == FOOT && frame[frame.Length - 2] == FOOT)
            {
                byte[] data = new byte[frame.Length - 2 - 2 - 1];
                Array.Copy(frame, 2, data, 0, data.Length);
                if (CheckCRC(data, frame[frame.Length - 3]))
                {
                    if (data[1] != SUCCESS)
                    {
                        return new Tuple<byte, byte, ushort, byte[]>(data[0], data[1], BitConverter.ToUInt16(data, 2), new byte[] { });
                    }
                    else
                    {
                        byte[] values = new byte[data.Length - 6];
                        Array.Copy(data, 6, values, 0, values.Length);
                        return new Tuple<byte, byte, ushort, byte[]>(data[0], data[1], BitConverter.ToUInt16(data, 2), values);
                    }
                }
                else
                {
                    return new Tuple<byte, byte, ushort, byte[]>(data[0], ERROR_COMMON_CRC, BitConverter.ToUInt16(data, 2), new byte[] { });
                }
            }
            else
            {
#if DEBUG
                throw new Exception("Frame Decode Error");
#else
                System.Console.WriteLine("Frame Decode Error");
                return null;
#endif
            }
        }

        internal static bool CheckFunctionCode(ref byte[] command, ushort func)
        {
            return BitConverter.ToUInt16(command, 4) == func;
        }
    }

    class CommStringProtocolDecoder
    {
        internal const string Done = "Done";
        internal const string Error = "Error";

        string customFoot = Done;
        public string CustomFoot { 
            get=>customFoot; set=>customFoot = value; 
        }
        string ReplayContentBuffer { get; set; }
        public Action<string> NotifyDecodeResult { get; set; }

        public bool CompareEndString { get; set; }

        internal void Decode(ref string data)
        {
            lock(data)
            {
                ReplayContentBuffer += data;
                if(CompareEndString)
                {
                    if (ReplayContentBuffer.Contains(CustomFoot) || ReplayContentBuffer.Contains(Error))
                    {
                        NotifyDecodeResult?.Invoke(ReplayContentBuffer);
                        ReplayContentBuffer = string.Empty;
                    }
                }
                else
                {
                    NotifyDecodeResult?.Invoke(ReplayContentBuffer);
                    ReplayContentBuffer = string.Empty;
                }
            }
        }

        internal void Clear()
        {
            ReplayContentBuffer = string.Empty;
        }
    }
    class Translater
    {
        internal byte[] Translate(string command) 
        {
            try
            {
                string[] cmds = command.Split(' ');
                switch (cmds[0])
                {
                    case SerialRadarCommands.SoftReset:
                        return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_COMMON_SOFTRESET, new byte[] { });
                    case SerialRadarCommands.SensorStart:
                        return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_COMMON_SENSOR_START, new byte[] { });
                    case SerialRadarCommands.SensorStop:
                        return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_COMMON_SENSOR_STOP, new byte[] { });
                    case SerialRadarCommands.WriteCLI:
                        {
                            byte[] data = null;
                            switch (cmds[1])
                            {
                                case SerialArguments.BootLoaderFlag:
                                    {
                                        data = new byte[] { 0x00};
                                        data[0] = byte.Parse(cmds[2]);
                                        return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_UPDATE_BOOLOADER, data);
                                    }
                                case SerialArguments.CommandBoundRate:
                                    {
                                        data = new byte[6] { 0x05, 0x00, 0x00, 0x00, 0x00, 0x00 };
                                        Array.Copy(BitConverter.GetBytes(int.Parse(cmds[2])), 0, data, 2, 4);
                                    }
                                    break;
                                case SerialArguments.DelayTimeParam:
                                    {
                                        data = new byte[4] { 0x02, 0x00, 0x00, 0x00 };
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[2])), 0, data, 2, 2);
                                    }
                                    break;
                                case SerialArguments.FilterParam:
                                    {
                                        data = new byte[14];
                                        data[0] = 0x01;
                                        data[1] = 0x00;
                                        data[2] = byte.Parse(cmds[2]);
                                        data[3] = byte.Parse(cmds[3]);
                                        data[4] = (byte)(float.Parse(cmds[4]));
                                        data[5] = (byte)(float.Parse(cmds[5]));
                                        data[6] = byte.Parse(cmds[6]);
                                        data[7] = (byte)(float.Parse(cmds[7]));
                                        data[8] = (byte)(float.Parse(cmds[8]));
                                        data[9] = byte.Parse(cmds[9]);
                                        data[10] = (byte)(float.Parse(cmds[10])*10);
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[11])), 0, data, 11, 2);
                                        data[13] = (cmds[12] == "0") ? (byte)0x00 : (byte)0x0A;
                                    }
                                    break;
                                case SerialArguments.RodDirection:
                                    {
                                        data = new byte[1] { 0x00 };
                                        data[0] = byte.Parse(cmds[2]);
                                        return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_SET_ROD_DIRECTION, data);
                                    }
                                case SerialArguments.RodArea:
                                    {
                                        data = new byte[8] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[2])*100)), 0, data, 2, 2);
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[3]) * 100)), 0, data, 2*2, 2);
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[4]))), 0, data, 2*3, 2);
                                    }
                                    break;
                                case SerialArguments.TriggerBox:
                                    {
                                        data = new byte[] { 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[2])*100)), 0, data, 2, 2);
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[3])*100)), 0, data, 2*2, 2);
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[4])*100)), 0, data, 2*3, 2);
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[5])*100)), 0, data, 2*4, 2);
                                    }
                                    break;
                                case SerialArguments.Direction:
                                    {
                                        data = new byte[] { 0x12, 0x00, 0x00, 0x00};
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[2]) ), 0, data, 2, 2);
                                    }
                                    break;
                                case SerialArguments.ClassifyBox:
                                    {
                                        data = new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[2]) * 100)), 0, data, 2*1, 2);
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[3]) * 100)), 0, data, 2*2, 2);
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[4]) * 100)), 0, data, 2*3, 2);
                                        Array.Copy(BitConverter.GetBytes((short)(float.Parse(cmds[5]) * 100)), 0, data, 2*4, 2);
                                    }
                                    break;
                                case SerialArguments.CarNumber:
                                    {
                                        data = new byte[] { 0x13, 0x00, 0x00, 0x00 };
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[2])), 0, data, 2, 2);
                                    }
                                    break;
                                case SerialArguments.TriggerThreshold:
                                    {
                                        data = new byte[] { 0x14, 0x00, 0x00, 0x00 };
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[2])), 0, data, 2, 2);
                                    }
                                    break;
                                case SerialArguments.StayThreshold:
                                    {
                                        data = new byte[] { 0x15, 0x00, 0x00, 0x00 };
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[2])), 0, data, 2, 2);
                                    }
                                    break;
                                case SerialArguments.ThresholdParas:
                                    {
                                        data = new byte[] {0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[2])), 0, data, 2, 2);
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[3])*100), 0, data, 4, 2);
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[4])*100), 0, data, 6, 2);
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[5])), 0, data, 8, 2);
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[6])), 0, data, 10, 2);
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[7])), 0, data, 12, 2);
                                        Array.Copy(BitConverter.GetBytes(short.Parse(cmds[8])*100), 0, data, 14, 2);
                                    }
                                    break;
                            }
                            return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_WRITECLI, data);
                        }
                    case SerialRadarCommands.ReadCLI:
                        {
                            byte[] data = null;
                            switch (cmds[1])
                            {
                                case SerialArguments.BootLoaderFlag:
                                    {
                                        data = new byte[2] { 0x06, 0x00};
                                    }
                                    break;
                                case SerialArguments.CommandBoundRate:
                                    {
                                        data = new byte[2] { 0x05, 0x00};
                                    }
                                    break;
                                case SerialArguments.DelayTimeParam:
                                    {
                                        data = new byte[2] { 0x02, 0x00};
                                    }
                                    break;
                                case SerialArguments.FilterParam:
                                    {
                                        data = new byte[2] { 0x01, 0x00};
                                    }
                                    break;
                                case SerialArguments.RodDirection:
                                    {
                                        return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_GET_ROD_DIRECTION, new byte[] { });
                                    }
                                case SerialArguments.RodArea:
                                    {
                                        data = new byte[2] { 0x03, 0x00};
                                    }
                                    break;
                                case SerialArguments.TriggerBox:
                                    {
                                        data = new byte[2] { 0x11, 0x00 };
                                    }
                                    break;
                                case SerialArguments.Direction:
                                    {
                                        data = new byte[2] { 0x12, 0x00 };
                                    }
                                    break;
                                case SerialArguments.ClassifyBox:
                                    {
                                        data = new byte[2] { 0x10, 0x00 };
                                    }
                                    break;
                                case SerialArguments.CarNumber:
                                    {
                                        data = new byte[2] { 0x13, 0x00 };
                                    }
                                    break;
                                case SerialArguments.TriggerThreshold:
                                    {
                                        data = new byte[2] { 0x14, 0x00 };
                                    }
                                    break;
                                case SerialArguments.StayThreshold:
                                    {
                                        data = new byte[2] { 0x15, 0x00 };
                                    }
                                    break;
                                case SerialArguments.ThresholdParas:
                                    {
                                        data = new byte[2] {0x04, 0x00};
                                    }
                                    break;
                                case SerialArguments.All:
                                    {
                                        data = new byte[2] { 0x00, 0x00 };
                                    }
                                    break;
                            }
                            SUB_FUNCTION_CODE = data[0];
                            return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_COMMON_READCLI, data);
                        }
                    case SerialRadarCommands.FlashErase:
                        return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_UPDATE_ERASE, new byte[] { });
                    case SerialRadarCommands.CRC:
                        break;
                    case SerialRadarCommands.Output:
                        {
                            byte[] data = new byte[2] { 0x00, 0x00};
                            data[0] = byte.Parse(cmds[1]);
                            SUB_FUNCTION_CODE = data[0];
                            if (data[0] == 0x0D)
                                return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_RESET, data);
                            if (data[0] == 11)
                            {
                                data[1] = byte.Parse(cmds[2]);
                            }
                            return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_CLIOUTPUT, data);
                        }
                    case SerialRadarCommands.Version:
                        return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_COMMON_SOFT_VER, new byte[] { });
                    case SerialRadarCommands.SetTime:
                        {
                            byte[] data = new byte[7];
                            Array.Copy(BitConverter.GetBytes(short.Parse(cmds[1])), 0, data, 0, 2); /* — 年*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[2])), 0, data, 2, 1);  /* — 月*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[3])), 0, data, 3, 1);  /* — 日*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[4])), 0, data, 4, 1);  /* — 时*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[5])), 0, data, 6, 1);  /* — 分*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[6])), 0, data, 6, 1);  /* — 秒*/

                            return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_SET_SYSTEM_TIME, data);
                        }
                    case SerialRadarCommands.GetTIme:
                        return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_GET_SYSTEM_TIME, new byte[] { });
                    case SerialRadarCommands.ClearTime:
                        return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_CLEAR_RECORD, new byte[] { });
                    case SerialRadarCommands.SearchTime:
                        {
                            byte[] data = new byte[15];
                            data[0] = 0x00;
                            Array.Copy(BitConverter.GetBytes(short.Parse(cmds[1])), 0, data, 1, 2); /*起始时间 — 年*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[2])), 0, data, 3, 1);  /*起始时间 — 月*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[3])), 0, data, 4, 1);  /*起始时间 — 日*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[4])), 0, data, 5, 1);  /*起始时间 — 时*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[5])), 0, data, 6, 1);  /*起始时间 — 分*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[6])), 0, data, 7, 1);  /*起始时间 — 秒*/

                            Array.Copy(BitConverter.GetBytes(short.Parse(cmds[7])), 0, data, 8, 2); /*结束时间 — 年*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[8])), 0, data, 10, 1);  /*结束时间 — 月*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[9])), 0, data, 11, 1);  /*结束时间 — 日*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[10])), 0, data, 12, 1);  /*结束时间 — 时*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[11])), 0, data, 13, 1);  /*结束时间 — 分*/
                            Array.Copy(BitConverter.GetBytes(byte.Parse(cmds[12])), 0, data, 14, 1);  /*结束时间 — 秒*/

                            return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_SEARCH, data);
                        }
                    case SerialRadarCommands.SearchInvert:
                        {
                            SUB_FUNCTION_CODE = 0x01;
                            byte[] data = new byte[2] {0x00, 0x00 };
                            Array.Copy(BitConverter.GetBytes(short.Parse(cmds[1])), 0, data, 0, 2);

                            return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_SEARCH, data);
                        }
                    case SerialRadarCommands.AlarmOrder:
                        {
                            byte[] data = new byte[1];
                            data[0] = byte.Parse(cmds[1]);
                            SUB_FUNCTION_CODE = data[0];
                            return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_FALSEALARMORDER, data);
                        }
                    case SerialRadarCommands.Switch:
                        {
                            return CommHexProtocolDecoder.Code(CommHexProtocolDecoder.ADDRESS_TARGET_0, CommHexProtocolDecoder.SUCCESS, CommHexProtocolDecoder.FUNCTION_IT_SWITCH, new byte[] { 0x01 });
                        }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private byte SUB_FUNCTION_CODE { get; set; }
        internal string Translate(Tuple<byte/*address*/, byte/*error*/, ushort/*function*/, byte[]/*data*/> data)
        {
            try
            {
                string result = string.Empty;
                if (data.Item2 == CommHexProtocolDecoder.SUCCESS)
                {
                    switch (data.Item3)
                    {
                        case CommHexProtocolDecoder.FUNCTION_IT_FALSEALARMORDER:
                            switch(SUB_FUNCTION_CODE)
                            {
                                case 0x00:
                                    return "X:" + (BitConverter.ToInt16(data.Item4, 0) * 0.01f).ToString("F2") + " Y:" + (BitConverter.ToInt16(data.Item4, 2) * 0.01f).ToString("F2");
                                case 0x01:
                                    break;
                                case 0x02:
                                    result = string.Empty;
                                    for (int i = 0; i < data.Item4.Length; i += 4)
                                    {
                                        result += "X:" + (BitConverter.ToInt16(data.Item4, i) * 0.01f).ToString("F2") + " Y:" + (BitConverter.ToInt16(data.Item4, i + 2) * 0.01f).ToString("F2");
                                    }
                                    break;
                                case 0x03:
                                    SUB_FUNCTION_CODE = 0x00;
                                    break;
                                case 0x04:
                                    break;
                                default:
                                    switch (data.Item4[0])
                                    {
                                        case 0x00:
                                            return "X:" + (BitConverter.ToInt16(data.Item4, 1) * 0.01f).ToString("F2") + " Y:" + (BitConverter.ToInt16(data.Item4, 3) * 0.01f).ToString("F2");
                                        case 0x02:
                                            result = string.Empty;
                                            for (int i = 1; i < data.Item4.Length; i += 4)
                                            {
                                                result += "X:" + (BitConverter.ToInt16(data.Item4, i) * 0.01f).ToString("F2") + " Y:" + (BitConverter.ToInt16(data.Item4, i + 2) * 0.01f).ToString("F2");
                                            }
                                            return result;
                                    }
                                    break;
                            }
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_SEARCH:
                            result = string.Empty;
                            for (int i = 0; i < data.Item4.Length; i += 8)
                            {
                                result += BitConverter.ToInt16(data.Item4, i) + "-" + data.Item4[i + 2] + "-" + data.Item4[i + 3] + "   " + data.Item4[i + 4] + ":" + data.Item4[i + 5] + ":" + data.Item4[i + 6] + "\tRelay:" + data.Item4[i + 7] + "\n";
                            }
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_CLEAR_RECORD:
                            result = data.Item4[0] == 0x00 ? "Success" : "Fail";
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_GET_SYSTEM_TIME:
                            result = "Time(GMT)" + " " + BitConverter.ToInt16(data.Item4, 0) + " " + data.Item4[2] + " " + data.Item4[3] + " "  + data.Item4[4] + ":" + data.Item4[5] + ":" + data.Item4[6];
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_SET_SYSTEM_TIME:
                            result = data.Item4[0] == 0x00 ? "Success" : "Fail";
                            break;
                        case CommHexProtocolDecoder.FUNCTION_COMMON_SOFT_VER:
                            result = System.Text.Encoding.Default.GetString(data.Item4);
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_CLIOUTPUT:
                            switch (SUB_FUNCTION_CODE)
                            {
                                case 0x0C:
                                    result = string.Empty;
                                    for (int i = 0; i < data.Item4.Length; i += 4)
                                    {
                                        result += "X:" + (BitConverter.ToInt16(data.Item4, i) * 0.01f).ToString("F2") + "Y:" + (BitConverter.ToInt16(data.Item4, i + 2) * 0.01f).ToString("F2") + "    ";
                                    }
                                    break;
                                case 0x04:
                                    result = SerialRadarReply.StudyEnd;
                                    break;
                                case 0x00:
                                    break;
                                case 0x0D:
                                    break;
                                case 0x15:
                                    break;
                                case 0x16:
                                    break;
                                default:
                                    switch (data.Item4[0])
                                    {
                                        case 0x01:
                                            result = "X:" + BitConverter.ToInt16(data.Item4, 2) * 0.01f + " Y:" + BitConverter.ToInt16(data.Item4, 4) * 0.01f + " P:" + BitConverter.ToInt16(data.Item4, 6) + " DL:" + BitConverter.ToInt16(data.Item4, 8) + " THS:" + BitConverter.ToInt16(data.Item4, 10) + " did:" + BitConverter.ToInt16(data.Item4, 12);
                                            break;
                                        case 0x02:
                                            result = "MC:" + BitConverter.ToInt16(data.Item4, 2) * 0.01f + " Var:" + BitConverter.ToInt16(data.Item4, 4);
                                            break;
                                        case 0x03:
                                            result = "X:" + BitConverter.ToInt16(data.Item4, 2) * 0.01f + " Y:" + BitConverter.ToInt16(data.Item4, 4) * 0.01f;
                                            break;
                                        case 0x06:
                                            result = data.Item4[0] == 0x00 ? "up" : "down";
                                            break;
                                        case 0x0B:
                                            result = string.Empty;
                                            for (int i = 1; i < data.Item4.Length; i += 4)
                                            {
                                                result += "X:" + (BitConverter.ToInt16(data.Item4, i) * 0.01f).ToString("F2") + "Y:" + (BitConverter.ToInt16(data.Item4, i + 2) * 0.01f).ToString("F2") + "    ";
                                            }
                                            break;
                                        case 0x0C:
                                            result = string.Empty;
                                            for (int i = 1; i < data.Item4.Length; i += 4)
                                            {
                                                result += "X:" + (BitConverter.ToInt16(data.Item4, i) * 0.01f).ToString("F2") + "Y:" + (BitConverter.ToInt16(data.Item4, i + 2) * 0.01f).ToString("F2") + "    ";
                                            }
                                            break;
                                        case 0x0D:
                                            result = BitConverter.ToString(data.Item4);
                                            break;
                                        case 0x0A:
                                            System.Console.WriteLine("DATA:" + BitConverter.ToString(data.Item4));
                                            result = "X:" + (BitConverter.ToInt16(data.Item4, 2) * 0.01f).ToString("F2") + "  Y:" + (BitConverter.ToInt16(data.Item4, 4) * 0.01f).ToString("F2") + "  Num:" + BitConverter.ToInt16(data.Item4, 6);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case CommHexProtocolDecoder.FUNCTION_UPDATE_ERASE:
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_READCLI:
                        case CommHexProtocolDecoder.FUNCTION_COMMON_READCLI:
                            switch (SUB_FUNCTION_CODE)
                            {
                                case 0x00:
                                    result = SerialArguments.FilterParam + ": " + (BitConverter.ToInt32(data.Item4, 0) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 4) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 8) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 12) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 16) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 20) * 0.1f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 24) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 28) * 0.1f).ToString("F0") + " " + (BitConverter.ToInt32(data.Item4, 32) * 1.0f).ToString("F0") + " " + (BitConverter.ToInt32(data.Item4, 36) * 1.0f).ToString("F0") + " " + (BitConverter.ToInt32(data.Item4, 40) * 0.1f).ToString("F0") + "\n"
                                                    + SerialArguments.ThresholdParas + ": " + BitConverter.ToInt32(data.Item4, 44) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 48) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 52) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 56) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 60) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 64) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 68) * 0.01f + "\n"
                                                    + SerialArguments.DelayTimeParam + ": " + BitConverter.ToInt32(data.Item4, 72);
                                    break;
                                case 0x04:
                                    result = SerialArguments.ThresholdParas + ":" + BitConverter.ToInt32(data.Item4, 0) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 4) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 8) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 12) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 16) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 20) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 24) * 0.01f;
                                    break;
                                case 0x01:
                                    result = SerialArguments.FilterParam + " " + (BitConverter.ToInt32(data.Item4, 0) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 4) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 8) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 12) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 16) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 20) * 0.1f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 24) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 28) * 1.0f).ToString("F2") + " " + (BitConverter.ToInt32(data.Item4, 32) * 1.0f).ToString("F0") + " " + (BitConverter.ToInt32(data.Item4, 36) * 1.0f).ToString("F0") + " " + (BitConverter.ToInt32(data.Item4, 40) * 0.1f).ToString("F0");
                                    break;
                                case 0x02:
                                    result = SerialArguments.DelayTimeParam + " " + BitConverter.ToInt32(data.Item4, 0);
                                    break;
                                case 0x03:
                                    result = SerialArguments.RodArea + " " + BitConverter.ToInt32(data.Item4, 0) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 4) * 0.01f + " " + BitConverter.ToInt32(data.Item4, 8);
                                    break;
                                default:
                                    switch (data.Item4[0])
                                    {
                                        case 0x01:
                                            result = SerialArguments.FilterParam + " " + data.Item4[2] * 0.1f + data.Item4[3] * 0.1f + data.Item4[4] * 0.1f + data.Item4[5] * 0.1f + data.Item4[6] * 0.1f + data.Item4[7] * 0.1f + data.Item4[8] * 0.1f + data.Item4[9] * 0.1f + data.Item4[10] * 0.1f + BitConverter.ToInt16(data.Item4, 11) * 0.1f + data.Item4[13] * 0.1f;
                                            break;
                                        case 0x02:
                                            result = SerialArguments.DelayTimeParam + " " + BitConverter.ToInt16(data.Item4, 2);
                                            break;
                                        case 0x03:
                                            result = SerialArguments.RodArea + " " + BitConverter.ToInt16(data.Item4, 2) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 4) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 6);
                                            break;
                                        case 0x04:
                                            result = SerialArguments.ThresholdParas + " " + BitConverter.ToInt16(data.Item4, 2) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 4) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 6) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 8) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 10) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 12) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 14) * 0.01f;
                                            break;
                                        case 0x10:
                                            result = SerialArguments.ClassifyBox + " " + BitConverter.ToInt16(data.Item4, 2) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 4) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 6) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 8) * 0.01f;
                                            break;
                                        case 0x11:
                                            result = SerialArguments.TriggerBox + " " + BitConverter.ToInt16(data.Item4, 2) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 4) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 6) * 0.01f + " " + BitConverter.ToInt16(data.Item4, 8) * 0.01f;
                                            break;
                                        case 0x12:
                                            result = SerialArguments.Direction + " " + BitConverter.ToInt16(data.Item4, 2);
                                            break;
                                        case 0x13:
                                            result = SerialArguments.CarNumber + " " + BitConverter.ToInt16(data.Item4, 2);
                                            break;
                                        case 0x14:
                                            result = SerialArguments.TriggerThreshold + " " + BitConverter.ToInt16(data.Item4, 2);
                                            break;
                                        case 0x15:
                                            result = SerialArguments.StayThreshold + " " + BitConverter.ToInt16(data.Item4, 2);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_WRITECLI:
                            break;
                        case CommHexProtocolDecoder.FUNCTION_COMMON_SENSOR_STOP:
                            break;
                        case CommHexProtocolDecoder.FUNCTION_COMMON_SENSOR_START:
                            break;
                        case CommHexProtocolDecoder.FUNCTION_COMMON_SOFTRESET:
                            result = SerialRadarReply.Start;
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_RESET:
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_POINTS_STRONGEST_STATE:
                            result = SerialRadarCommands.StrongestPointStatus + " PeakAverage:" + BitConverter.ToInt16(data.Item4, 0) + "  Position:" + data.Item4[2];
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_RADAR_STATE:
                            result = SerialRadarCommands.RadarStatus + " PointFlag:" + (data.Item4[0] & 0x01) + " TargetFlag:" + (data.Item4[0] & 0x02) + " PoleFlag:" + (data.Item4[0] & 0x04) + " PoleDirectionTarget:" + (data.Item4[0] & 0x08) + " CarFlag:" + (data.Item4[0] & 0x10) + " Maintain:" + (data.Item4[0] & 0x20) + " TargetRelevance:" + (data.Item4[0] & 0x40) + " RelayState:" + (data.Item4[0] & 0x80) + " UpCount:" + data.Item4[1] + " MissCount:" + data.Item4[2] + " DplMapVariance:" + BitConverter.ToInt16(data.Item4, 4) + " MaxFenceRelevance:" + BitConverter.ToInt16(data.Item4, 6);
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_POINTS_DATA:
                            result = SerialRadarCommands.AllPoints + " ID:" + data.Item4[0] + " X:" + (((((data.Item4[3] & 0x03) << 8) + data.Item4[1])*1.0f / 4.0f - 128.0f) / 20.0f).ToString("F2") + " Y:" + (((((data.Item4[3] & 0x30) << 8) + data.Item4[2])*1.0f / 4.0f) / 20.0f).ToString("F2") + " V:" + ((data.Item4[4] - 128) * 0.4).ToString("F2") + " Peak:" + BitConverter.ToInt16(data.Item4, 5) + " TriggerRange:" + ((data.Item4[7] & 0x80)>>7) + " TriggerThreshold:" + ((data.Item4[7] & 0x40)>>6) + " TriggerStrongest:" + ((data.Item4[7] & 0x02) >> 1) + " TriggerAction:" + (data.Item4[7] & 0x01);
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_POINTS_STRONGEST:
                            result = SerialRadarCommands.Strongest + " ID:" + data.Item4[0] + " X:" + (((((data.Item4[3] & 0x03) << 8) + data.Item4[1])*1.0f/ 4.0f - 128.0f) / 20.0f).ToString("F2") + " Y:" + (((((data.Item4[3] & 0x30) << 8) + data.Item4[2])*1.0f / 4.0f) / 20.0f).ToString("F2") + " V:" + ((data.Item4[4] - 128) * 0.4).ToString("F2") + " Peak:" + BitConverter.ToInt16(data.Item4, 5) + " TriggerRange:" + ((data.Item4[7] & 0x80) >> 7) + " TriggerThreshold:" + ((data.Item4[7] & 0x40) >> 6) + " TriggerStrongest:" + ((data.Item4[7] & 0x02) >> 1) + " TriggerAction:" + (data.Item4[7] & 0x01);
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_POINTS_STATE:
                            result = SerialRadarCommands.PointStatus + " NO.:" + data.Item4[0] + " Number:" + data.Item4[1] + " Relay:" + data.Item4[7];
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_HEART:
                            result =  SerialRadarCommands.Heart + " No.:" + data.Item4[0] + " Registered:" + ((data.Item4[1] & 0x80)>>7) + " Adjusted:" + ((data.Item4[1] & 0x40)>>6) + " Studyed:" + ((data.Item4[1] & 0x20)>>5) + " Relay:" + (data.Item4[1] & 0x01) + " GateType:" + (data.Item4[2]&0x80) + " Direction:" + (data.Item4[2]&0x20);
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_SWITCH:
                            break;
                        case CommHexProtocolDecoder.FUNCTION_IT_GET_ROD_DIRECTION:
                            result = SerialArguments.RodDirection + " " + data.Item4[0];
                            break;
                    }
                }
                else
                {
                    result = TranslateErrorCode(data.Item3, data.Item2);
                }
                return result + " \nDone";
            }
            catch (Exception)
            {
#if DEBUG
                throw new Exception("Translate Data Decode Error");
#else
                System.Console.WriteLine("Translate Data Decode Error");
                return ErrorString.Error + ErrorString.TranslateError;
#endif
            }
        }
        internal string TranslateErrorCode(ushort func, byte err)
        {
            string result = string.Empty;

            if (func > 0x00FF)
            {
                //道闸项目错误码
                switch (err)
                {
                    case CommHexProtocolDecoder.ERROR_IT_CRC:
                        result += ErrorString.ITCRC;
                        break;
                    case CommHexProtocolDecoder.ERROR_IT_LACK_PARAMS:
                        result += ErrorString.ITLackParams;
                        break;
                    case CommHexProtocolDecoder.ERROR_IT_STUDY:
                        result += ErrorString.ITStudy;
                        break;
                    case CommHexProtocolDecoder.ERROR_IT_PARAMS:
                        result += ErrorString.ITParams;
                        break;
                    case CommHexProtocolDecoder.ERROR_IT_FUNCTION:
                        result += ErrorString.ITFunction;
                        break;
                    case CommHexProtocolDecoder.ERROR_IT_ADDRESS:
                        result += ErrorString.ITAddress;
                        break;
                    case CommHexProtocolDecoder.ERROR_IT_PRECONDITION:
                        result += ErrorString.ITPreCondition;
                        break;
                }
            }
            else
            {
                //通用错误码
                switch (err)
                {
                    case CommHexProtocolDecoder.ERROR_COMMON_ADDRESS:
                        result += ErrorString.CommonAddress;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_FUNCTION:
                        result += ErrorString.CommonFunction;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_LENGTH:
                        result += ErrorString.CommonLength;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_CRC:
                        result += ErrorString.CommonCRC;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_OVERTIME:
                        result += ErrorString.CommonOverTime;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UNKNOW:
                        result += ErrorString.CommonUnknow;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UPDATE_ERASE:
                        result += ErrorString.CommonUpdateErase;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UPDATE_PACKET_COUNT:
                        result += ErrorString.CommonUpdatePacketCount;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UPDATE_PACKET_ORDER:
                        result += ErrorString.CommonUpdatePacketOrder;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UPDATE_PACKET_NUMBER:
                        result += ErrorString.CommonUpdatePacketNumber;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UPDATE_DATA_LENGTH:
                        result += ErrorString.CommonUpdateDataLength;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UPDATE_DATA_WRITE:
                        result += ErrorString.DataWriteError;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UPDATE_CRC:
                        result += ErrorString.CommonUpdateCRC;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UPDATE_FRESH:
                        result += ErrorString.CommonUpdateFresh;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UPDATE_OTHER:
                        result += ErrorString.CommonUpdateOther;
                        break;
                    case CommHexProtocolDecoder.ERROR_COMMON_UPDATE_UNKNOW:
                        result += ErrorString.CommonUpdateUnknow;
                        break;
                }
            }
            return result;
        }
    }
}
