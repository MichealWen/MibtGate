using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Windows.Navigation;

namespace MbitGate.helper
{
    class CommunicateHexProtocolDecoder
    {
        private const byte HEADER = 0x55;
        private const byte FOOT = 0xAA;
        public const byte SUCCESS = 0x00;

        private const byte MinLength = 0x0B;

        public const byte ADDRESS_PUBLIC = 0xFF;

        public const byte ERROVERTIME = 0x05;

        private const ushort FUNCTION_HARD_VER = 0x0001;
        private const ushort FUNCTION_SOFT_VER = 0x0002;

        public const ushort FUNCTION_UPDATE_TEST = 0x00F0;
        public const ushort FUNCTION_UPDATE_ERASE = 0x00F1;
        public const ushort FUNCTION_UPDATE_SIZE = 0x00F2;
        public const ushort FUNCTION_UPDATE_POSITION_DATA = 0x00F3;
        public const ushort FUNCTION_UPDATE_CRC = 0x00F4;
        public const ushort FUNCTION_UPDATE_DATA = 0x00F5;
        public const ushort FUNCTION_UPDATE_READ_FLASH = 0x00F6;
        public const ushort FUNCTION_UPDATE_REFRESH = 0x00F8;
        public const ushort FUNCTION_UPDATE_RESTART = 0x00F9;
        
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
        public Action<byte> NotifyReplayError { get; set; }

        public Action<Tuple<byte/*address*/, byte/*error*/, ushort/*function*/, byte[]/*data*/>> NotifyDecodeResult { get; set; }
        public Action<byte[]> NotifyFullResult { get; set; }
        internal void Decode(byte[] frame)
        {
            if(frame[0] == HEADER && frame[1] == HEADER && frame[frame.Length-1] == FOOT && frame[frame.Length-2] == FOOT)
            {
                byte[] data = new byte[frame.Length - 2 -2 -1];
                Array.Copy(frame, 2, data, 0, data.Length);
                if (CheckCRC(data , frame[frame.Length - 3]))
                {
                    if (data[1] != SUCCESS)
                    {
                        NotifyReplayError(data[1]);
                    }
                    else
                    {
                        byte[] values = new byte[data.Length - 6];
                        Array.Copy(data, 6, values, 0, values.Length);
                        NotifyDecodeResult?.Invoke(new Tuple<byte, byte, ushort, byte[]>(data[0], data[1], BitConverter.ToUInt16(data, 2), data));
                    }
                }
                else
                {
                    NotifyCRCError?.Invoke();
                }
            }
            else
            {
                throw new Exception("Frame Decode Error");
            }
        }

        private bool CheckCRC(byte[] data, byte val)
        {
            return (val==0xFF)?true:(Crc8(data) == val);
        }

        private int decodeCount = 0;
        public int MaxCantDecodeCount = 100;
        internal void Decode(ref ConcurrentQueue<byte> data)
        {
            lock(data)
            {
                byte headerCount = 0, index = 0, tmp = 0;
                int frameStart = 0;
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
                    bool errorFrame = false;
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
                            errorFrame = (val != FOOT);
                        }
                        else if(index == (frameEnd+1))
                        {
                            errorFrame = errorFrame && (val != FOOT);
                            break;
                        }
                    }
                    if(index == (frameEnd+1))
                    {
                        if (errorFrame)
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
                            if(framelength==0)
                            {
                                int stophere = 0;
                            }
                            System.Threading.Tasks.Task.Factory.StartNew(() => { Decode(frame); });
                            //Decode(frame);
                            decodeCount = 0;
                        }
                    }
                }

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
    }

    class CommunicateStringProtocolDecoder
    {
        private const string Done = "Done";
        private const string Error = "Error";

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
    }
}
