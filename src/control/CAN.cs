using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace MbitGate.control
{
    class CANManager
    {
        private Timer _dataPicker;
        const UInt32 STATUS_OK = 1;
        const UInt32 STATUS_FAIL = 0;
        private uint _device, _ind, _id, _baud;

        public Action<VCI_CAN_OBJ[]> DataReceivedHandler { get; set; }
        public CANManager()
        {
            _dataPicker = new Timer();
            _dataPicker.Interval = 200;
            _dataPicker.Elapsed += DataPicker_Elapsed;
            RecvSize = 50;
        }

        public bool IsStart { get; set; }

        public CANManager(uint type, uint ind, uint id, uint baud)
        {
            _dataPicker = new Timer();
            _dataPicker.Interval = 200;
            _dataPicker.Elapsed += DataPicker_Elapsed;
            RecvSize = 100;
            IsStart = Init(type, ind, id, baud);
        }

        private void DataPicker_Elapsed(object sender, ElapsedEventArgs e)
        {
            UInt32 res = CANDLL.CXCAN.VCI_GetReceiveNum(_device, _ind, _id);
            if (res == 0)
                return;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)RecvSize);
            res = CANDLL.CXCAN.VCI_Receive(_device, _ind, _id, ptr, RecvSize, 100);
            if (res > 0 && res < 0xFFFF)
            {
                VCI_CAN_OBJ[] data = new VCI_CAN_OBJ[res];
                for (uint i = 0; i < res; i++)
                {
                    data[i] = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)ptr + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                }
                if (DataReceivedHandler != null)
                    DataReceivedHandler(data);
            }
            Marshal.FreeHGlobal(ptr);
            RecvSize = 50;
            //_dataPicker.Stop();
        }

        public uint RecvSize { get; set; }
        public string LastError { get; internal set; }

        public unsafe bool Init(uint type, uint ind, uint canid, uint baud)
        {
            try
            {
                if (CANDLL.CXCAN.VCI_OpenDevice(type, ind, canid) != STATUS_OK)
                {
                    IsStart = false;
                    return false;
                }
                if (CANDLL.CXCAN.VCI_SetReference(type, ind, canid, 0, (byte*)&baud) != STATUS_OK)
                {
                    IsStart = false;
                    return false;
                }
                int nTimeOut = 3000;
                CANDLL.CXCAN.VCI_SetReference(type, ind, canid, 4, (byte*)&nTimeOut);
                VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                config.AccCode = Convert.ToUInt32(0x00000000);
                config.AccMask = Convert.ToUInt32(0xFFFFFFFF);
                config.Filter = 0x01;
                config.Mode = 0x00;
                if(CANDLL.CXCAN.VCI_InitCAN(type, ind, canid, ref config) != STATUS_OK)
                {
                    IsStart = false;
                    return false;
                }
                _device = type;
                _ind = ind;
                _id = canid;
                _baud = baud;
                if (IsStart = StartCAN())
                {
                    _dataPicker.Start();
                    return true;
                }
            }
            catch(Exception e)
            {
                IsStart = false;
                return false;
            }
            return false;
        }

        internal bool StopCAN()
        {
            if(CANDLL.CXCAN.VCI_ResetCAN(_device, _ind, _id) == STATUS_OK)
            {
                return true;
            }else
            {
                IsStart = false;
                return false;
            }
        }

        internal bool StartCAN()
        {
            if (CANDLL.CXCAN.VCI_StartCAN(_device, _ind, _id) == STATUS_OK)
            {
                return true;
            }else
            {
                IsStart = false;
                return false;
            }
        }

        public unsafe bool Send(uint target, byte[] data, byte externFlag=0x00)
        {
            VCI_CAN_OBJ msg = new VCI_CAN_OBJ();
            msg.SendType = 0x00;
            msg.RemoteFlag = 0x00;
            msg.ExternFlag = externFlag;
            msg.DataLen = (byte)data.Length;
            msg.ID = target;
            for (int i = 0; i < data.Length; i++)
            {
                msg.Data[i] = data[i];
            }
            return CANDLL.CXCAN.VCI_Transmit(_device, _ind, _id, ref msg, 1) == STATUS_OK;
        }

        public bool Close()
        {
            CANDLL.CXCAN.VCI_ClearBuffer(_device, _ind, _id);
            _dataPicker.Stop();
            IsStart = false;
            return CANDLL.CXCAN.VCI_CloseDevice(_device, _ind)==1;
        }

        internal void ClearCAN()
        {
            CANDLL.CXCAN.VCI_ClearBuffer(_device, _ind, _id);
        }
    }

    unsafe public struct VCI_CAN_OBJ  //使用不安全代码
    {
        public uint ID;
        public uint TimeStamp;
        public byte TimeFlag;
        public byte SendType;
        public byte RemoteFlag;//是否是远程帧
        public byte ExternFlag;//是否是扩展帧
        public byte DataLen;
        public fixed byte Data[8];
        public fixed byte Reserved[3];
    }

    public struct VCI_INIT_CONFIG
    {
        public UInt32 AccCode;
        public UInt32 AccMask;
        public UInt32 Reserved;
        public byte Filter;
        public byte Timing0;
        public byte Timing1;
        public byte Mode;
    }

    public struct VCI_FILTER_RECORD
    {
        public UInt32 ExtFrame;
        public UInt32 Start;
        public UInt32 End;
    }

    public struct CHGDESIPANDPORT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] szpwd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] szdesip;
        public Int32 desport;

        public void Init()
        {
            szpwd = new byte[10];
            szdesip = new byte[20];
        }
    }

    public struct VCI_ERR_INFO
    {
        public UInt32 ErrCode;
        public byte Passive_ErrData1;
        public byte Passive_ErrData2;
        public byte Passive_ErrData3;
        public byte ArLost_ErrData;
    }

    public struct VCI_CAN_STATUS
    {
        public byte ErrInterrupt;
        public byte regMode;
        public byte regStatus;
        public byte regALCapture;
        public byte regECCapture;
        public byte regEWLimit;
        public byte regRECounter;
        public byte regTECounter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
    }

    public struct VCI_BOARD_INFO
    {
        public UInt16 hw_Version;
        public UInt16 fw_Version;
        public UInt16 dr_Version;
        public UInt16 in_Version;
        public UInt16 irq_Num;
        public byte can_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] public byte[] str_Serial_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] str_hw_Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved;
    }

    public class Devices
    {
        public const int VCI_PCI5121 = 1;
        public const int VCI_PCI9810 = 2;
        public const int VCI_USBCAN1 = 3;
        public const int VCI_USBCAN2 = 4;
        public const int VCI_USBCAN2A = 4;
        public const int VCI_PCI9820 = 5;
        public const int VCI_CAN232 = 6;
        public const int VCI_PCI5110 = 7;
        public const int VCI_CANLITE = 8;
        public const int VCI_ISA9620 = 9;
        public const int VCI_ISA5420 = 10;
        public const int VCI_PC104CAN = 11;
        public const int VCI_CANETUDP = 12;
        public const int VCI_CANETE = 12;
        public const int VCI_DNP9810 = 13;
        public const int VCI_PCI9840 = 14;
        public const int VCI_PC104CAN2 = 15;
        public const int VCI_PCI9820I = 16;
        public const int VCI_CANETTCP = 17;
        public const int VCI_PEC9920 = 18;
        public const int VCI_PCI5010U = 19;
        public const int VCI_USBCAN_E_U = 20;
        public const int VCI_USBCAN_2E_U = 21;
        public const int VCI_PCI5020U = 22;
        public const int VCI_EG20T_CAN = 23;
    }
    class CANDLL
    {
        public static class CXCAN
        {
            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);
            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);

            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
            [DllImport("controlcan.dll")]
            //static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
            internal unsafe static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, byte* pData);

            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

            [DllImport("controlcan.dll")]
            internal static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

            //[DllImport("controlcan.dll")]
            //static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);
            [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
            internal static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);
        }
    }
}
