using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

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
        public const string SensorStart = "sensorStart";
        public const string WriteCLI = "WriteCLI";
        public const string ReadCLI = "ReadCLI";
        public const string ResetCLI = "ResetCLI";
        public const string Update = "Update";
        public const string BootLoader = "BootLoader";
        public const string FlashErase = "flashErase";
        public const string T = "T";
        public const string CRC = "CRC";
        public const string Output = "clioutput";
        public const string Version = "ver.";
        public const string GetTIme = "ReadTime";
        public const string SetTime = "setTime";
        public const string ClearTime = "TimeErase";
        public const string SearchTime = "DalayFilpTime";
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
        public const string BootLoaderFlag = "bootLoaderFlag";
        public const string DelayTimeParam = "setDelayTimeParas";
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
        public static string ParamError = Application.Current.Resources["ParamsError"].ToString();
        public static string BinPathError = Application.Current.Resources["ParamsError"].ToString();
        public static string SerialError = Application.Current.Resources["SerialError"].ToString();
        public static string SerialOpenError = Application.Current.Resources["SerialOpenError"].ToString();
        public static string FileError = Application.Current.Resources["FileError"].ToString();
        public static string CANOpenError = Application.Current.Resources["CANOpenError"].ToString();
        public static string RadarError = Application.Current.Resources["RadarError"].ToString();
        public static string DisntacneError = Application.Current.Resources["DisntacneError"].ToString();
        public static string RangeError1 = Application.Current.Resources["RangeError1"].ToString();
        public static string RangeError2 = Application.Current.Resources["RangeError2"].ToString();
        public static string SmallVersion = Application.Current.Resources["SmallVersion"].ToString();
        public static string OverTime = Application.Current.Resources["OverTime"].ToString();
        public static string DelayError = Application.Current.Resources["DelayError"].ToString();
    }

    public class Tips
    {
        public static string Opening = Application.Current.Resources["Opening"].ToString();
        public static string Initializing = Application.Current.Resources["Initializing"].ToString();
        public static string Updating = Application.Current.Resources["Updating"].ToString();
        public static string Updated = Application.Current.Resources["Updated"].ToString();
        public static string UpdateFail = Application.Current.Resources["UpdateFail"].ToString();
        public static string Flashing = Application.Current.Resources["Flashing"].ToString();
        public static string CRCing = Application.Current.Resources["CRCing"].ToString();
        public static string Rating = Application.Current.Resources["Rating"].ToString();
        public static string ConfigSuccess = Application.Current.Resources["ConfigSuccess"].ToString();
        public static string ConfigFail = Application.Current.Resources["ConfigFail"].ToString();
        public static string GetFail = Application.Current.Resources["GetFail"].ToString();
        public static string GetSuccess = Application.Current.Resources["GetSuccess"].ToString();
        public static string Studying = Application.Current.Resources["Studying"].ToString();
        public static string StudySuccess = Application.Current.Resources["StudySuccess"].ToString();
        public static string StudyEnd = Application.Current.Resources["StudyEnd"].ToString();
        public static string WaitForOpen = Application.Current.Resources["WaitForOpen"].ToString();
        public static string ManualReboot = Application.Current.Resources["ManualReboot"].ToString();
        public static string RebootSuccess = Application.Current.Resources["RebootSuccess"].ToString();
        public const string GetTimeFail = "获取雷达时间失败";
        public const string GetTimeSuccess = "获取雷达时间成功";
        public const string SetTimeFail = "设置雷达时间失败";
        public const string SetTimeSuccess = "设置雷达时间成功";
        public const string ClearTiming = "正在清除雷达时间";
        public const string ClearTimeFail = "清除雷达时间失败";
        public const string ClearTimeSuccess = "清除雷达时间成功";
        public const string SearchTimeFail = "查询数据失败";
        public const string SearchTimeSuccess = "查询数据成功";
        public const string SearchTimeGetNone = "查询数据为空";
        public const string Searching = "正在查询数据";
        public const string SearchTimeEnd = "查询数据结束";
    }

    public class GateType
    {
        public static string Straight = Application.Current.Resources["Straight"].ToString();
        public static string AdvertisingFence = Application.Current.Resources["AdvertisingFence"].ToString();

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

    public class RecordKind
    {
        public const string Persistence = @"记录(雷达时间戳)";
        public const string Ignore = @"不记录(雷达时间戳)";

        public static List<string> GetAllTypes()
        {
            return new List<string>() { Persistence, Ignore };
        }

        public static string GetType(string value)
        {
            if (value == "0")
                return Ignore;
            else if (value == "1")
                return Persistence;
            else
                return value;
        }

        public static string GetValue(string type)
        {
            if (type == Ignore)
                return "0";
            else if (type == Persistence)
                return "1";
            else
                return type;
        }
    }

    public class OperationType
    {
        public const string Up = "\t抬杆";
        public const string Down = "\t降杆";

        public const string UpValue = @"Dalay:1";
        public const string DownValue = @"Dalay:0";
    }

    public class ThresholdType
    {
        public static string Low = Application.Current.Resources["LowSensibility"].ToString();
        public static string Middle = Application.Current.Resources["MiddleSensibility"].ToString();
        public static string High = Application.Current.Resources["HighSensibility"].ToString();
        public static List<string> GetAllTypes()
        {
            return new List<string> { Low, Middle, High };
        }

        public static string GetValue(string type)
        {
            if (type == Low)
            {
                return "4500";
            }
            else if (type == Middle)
            {
                return "3500";
            }
            else if (type == High)
            {
                return "0";
            }
            else
                return type;
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
