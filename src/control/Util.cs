using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MbitGate.control
{
    public class BauRate
    {
        public const string Rate300 = "300";
        public const string Rate600 = "600";
        public const string Rate1200 = "1200";
        public const string Rate2400 = "2400";
        public const string Rate4800 = "4800";
        public const string Rate9600 = "9600";
        public const string Rate19200 = "19200";
        public const string Rate38400 = "38400";
        public const string Rate57600 = "57600";
        public const string Rate74800 = "74800";
        public const string Rate115200 = "115200";
        public const string Rate230400 = "230400";
        public const string Rate460800 = "460800";
        public const string Rate512000 = "512000";
        public const string Rate600000 = "600000";
        public const string Rate750000 = "750000";
        public const string Rate921600 = "921600";
        public const string Rate1843200 = "1843200";
        public const string Rate3686400 = "3686400";

        public static List<string> GetReates()
        {
            return new List<string>() { Rate300, Rate600, Rate1200, Rate2400, Rate4800, Rate9600, Rate19200, Rate38400, Rate57600, Rate74800, Rate115200, Rate230400, Rate460800, Rate921600, Rate1843200, Rate3686400 };
        }
    }

    public class CANBauRate
    {
        public const string Rate500 = "500 Kbps";
        public const string Rate1000 = "1000 Kbps";
        public const string Rate800 = "800 Kbps";
        public const string Rate250 = "250 Kbps";
        public const string Rate125 = "125 Kbps";
        public const string Rate100 = "100 Kbps";
        public const string Rate50 = "50 Kbps";
        public const string Rate20 = "20 Kbps";
        public const string Rate10 = "10 Kbps";
        public const string Rate5 = "5 Kbps";

        public static List<string> GetRates()
        {
            return new List<string>() { Rate500, Rate1000 };
        }

        public static uint GetUINTRate(string key)
        {
            switch(key)
            {
                case Rate500:
                    return 0x060007;
                case Rate1000:
                    return 0x060003;
                case Rate800:
                    return 0x060004;
                case Rate250:
                    return 0x1C0008;
                case Rate125:
                    return 0x1C0011;
                case Rate100:
                    return 0x160023;
                case Rate50:
                    return 0x1C002C;
                case Rate20:
                    return 0x1600B3;
                case Rate10:
                    return 0x1C0010;
                case Rate5:
                    return 0x1C01C1;

            }
            return 0;
        }
    }

    public class SerialRadarCommands
    {
        public const string SoftReset = "softReset";
        public const string SensorStop = "sensorStop";
        public const string WriteCLI = "WriteCLI";
        public const string ReadCLI = "ReadCLI";
        public const string ResetCLI = "ResetCLI";
        public const string Update = "Update";
        public const string BootLoader = "BootLoader";
        public const string FlashErase = "flashErase";
        public const string T = "T";
        public const string CRC = "CRC";
        public const string Output = "clioutput";
    }

    public class ExtraSerialRadarCommands
    {
        public const string SoftInercludeReset = "SoftInercludeReset";
    }

    public class CANRadarCommands
    {
        public const string StopOutput = "CMD8400";
        public const string Synchronize = "CMD0100";
        public const string Version = "CMD0200";
        public const string CMDFlashErase = "CMD06";
        public const string CMDFILEPATHCRC = "FILEPATHCRC";
        public const string CMDFILETRANS = "FILETRANSFER";
        public const string CMDFILESUMCRC = "FILESUMCRC";
        public const string CMDFILEPREFIXCRC = "FILEPREFIXCRC";
    }
    public class SerialArguments
    {
        public const string CommandBoundRate = "commandBaudRate";
        public const string BoundRate921600 = "921600";
        public const string BoundRate1843200 = "1843200";
        public const string FilterParam = "setFilterPara";
        public const string FrameCfg = "frameCfg";
    }

    public class SerialRadarReply
    {
        public const string Done = "Done";
        public const string Error = "Error";
        public const string Success = "success";
        public const string Start = "RadarStart";
        public const string StudyEnd = "studyend";
    }
    public class ErrorString
    {
        public const string Error = "[Error]";
        public const string ParamError = "参数错误";
        public const string BinPathError = "固件路径错误";
        public const string SerialError = "串口错误";
        public const string SerialOpenError = "串口打开错误";
        public const string FileError = "文件错误";
        public const string CANOpenError = "CAN口打开错误";
        public const string RadarError = "雷达标号错误";
        public const string DisntacneError = "作用距离不能超过6米";
        public const string RangeError = "左右范围不能超过1米";
    }

    public class Tips
    {
        public const string Opening = "正在打开串口";
        public const string Initializing = "正在初始化设置";
        public const string Updating = "正在刷写固件";
        public const string Updated = "固件刷写成功";
        public const string UpdateFail = "固件刷写失败";
        public const string Flashing = "正在格式化Flash";
        public const string CRCing = "正在校验";
        public const string Rating = "正在修改波特率";
        public const string ConfigSuccess = "设置成功";
        public const string ConfigFail = "设置失败";
        public const string GetFail = "获取设置失败";
        public const string GetSuccess = "获取设置成功";
        public const string Studying = "正在自学习中";
        public const string StudySuccess = "自学习结束，请重启雷达";
    }

    public class GateType
    {
        public const string Straight = @"直杆";
        public const string AdvertisingFence = @"广告栅栏";

        public static List<string> GetAllTypes()
        {
            return new List<string>() { Straight, AdvertisingFence };
        }

        public static string GetType(string value)
        {
            if (value == "0")
                return Straight;
            else if (value == "1")
                return AdvertisingFence;
            else
                return value;
        }
        public static string GetValue(string type)
        {
            if (type == Straight)
                return "0";
            else if (type == AdvertisingFence)
                return "1";
            else
                return type;
        }
    }

    public class ThresholdType
    {
        public const string Low = @"低门限";
        public const string Middle = @"中门限";
        public const string High = @"高门限";
        public static List<string> GetAllTypes()
        {
            return new List<string> { Low, Middle, High };
        }

        public static string GetValue(string type)
        {
            switch(type)
            {
                case Low:
                    return "4500";
                case Middle:
                    return "3500";
                case High:
                    return "0";
                default:
                    return type;
            }
        }

        public static string GetType(string value)
        {
            switch(value)
            {
                case "4500":
                    return Low;
                case "3500":
                    return Middle;
                case "0":
                    return High;
                default:
                    return value;
            }
        }
    }
}
