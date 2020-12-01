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

        public static List<string> GetUpdateRates()
        {
            return new List<string>() { Rate115200, Rate57600, Rate38400, Rate19200};
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
        public const string Test = "Test";
        public const string FlashErase = "flashErase";
        public const string T = "T";
        public const string CRC = "CRC";
        public const string Output = "clioutput";
        public const string Version = "ver.";
        public const string GetTIme = "ReadTime";
        public const string SetTime = "setTime";
        public const string ClearTime = "TimeErase";
        public const string SearchTime = "RelayFlipTime";
        public const string SearchInvert = "RelayTimeLog";
        public const string WriteBaudRate = "commandBaudRate ";
        public const string AlarmOrder = "falseAlarmOrder";
        public const string AlarmOrder0 = "falseAlarmOrder 0";
        public const string AlarmOrder1 = "falseAlarmOrder 1";
        public const string AlarmOrder2 = "falseAlarmOrder 2";
        public const string AlarmOrder3 = "falseAlarmOrder 3";
        public const string AlarmOrder4 = "falseAlarmOrder 4";
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
        public const string TriggerBox = "triggerBoxCfg";
        public const string Direction = "directionCfg";
        public const string CarNumber = "carNumTrigger";
        public const string ClassifyBox = "classifyBoxCfg";
        public const string TriggerThreshold = "triTher";
        public const string StayThreshold = "staTher";
        public const string RodDirection = "rodDirection";
        public const string RodArea = "setUpRodSubArea";
        public const string ThresholdParas = "setThresholdParas";
        public const string All = "all";
    }

    public class SerialRadarReply
    {
        public const string Done = "Done";
        public const string Error = "Error";
        public const string Success = "success";
        public const string Start = "RadarStart";
        public const string StudyEnd = "studyend";
        public const string NotRecognized = "not recognized";
        public const string NewLine = "\n";
    }
    public class ErrorString
    {
        public const string Error = "[Error]";
        public static string Warning = Application.Current.Resources["Warnning"].ToString();
        public static string ParamError = Application.Current.Resources["ParamsError"].ToString();
        public static string BinPathError = Application.Current.Resources["BinPathError"].ToString();
        public static string SerialError = Application.Current.Resources["SerialError"].ToString();
        public static string SerialOpenError = Application.Current.Resources["SerialOpenError"].ToString();
        public static string FileError = Application.Current.Resources["FileError"].ToString();
        public static string CANOpenError = Application.Current.Resources["CANOpenError"].ToString();
        public static string RadarError = Application.Current.Resources["RadarError"].ToString();
        public static string DistacneMaxError = Application.Current.Resources["DistacneMaxError"].ToString();
        public static string DistacneMinError = Application.Current.Resources["DistacneMinError"].ToString();
        public static string RangeError1 = Application.Current.Resources["RangeError1"].ToString();
        public static string RangeError2 = Application.Current.Resources["RangeError2"].ToString();
        public static string SmallVersion = Application.Current.Resources["SmallVersion"].ToString();
        public static string OverTime = Application.Current.Resources["OverTime"].ToString();
        public static string DelayError = Application.Current.Resources["DelayError"].ToString();
        internal static string ConnectError = Application.Current.Resources["ConnectError"].ToString();

        public const string StudyError = "Study";
        public const string FunctionError = "Function Code";
        public const string AddressError = "Address Error";
        public const string PreCondition = "Lack of PreCondition";
        public const string FrameError = "Data Length Or Frame End Byte";
        public const string UnknowError = "Unknow";
        public const string EraseError = "Flase Erase";
        public const string FrameCountError = "Frame Count";
        public const string FrameNumberError = "Frame Number";
        public const string FrameLengthError = "Frame Length";
        public const string DataWriteError = "Write Data";
        public const string BinCRCError = "Bin File CRC";
        public const string BinFreshError = "Bin Data Copy";

        public const string CommonAddress = "Address Error";
        public const string CommonFunction = "Function Error";
        public const string CommonLength = "Length Error";
        public const string CommonCRC = "CRC Error";
        public const string CommonOverTime = "OverTime Error";
        public const string CommonUnknow = "Unknow Error";
        public const string CommonUpdateErase = "Erase Error";
        public const string CommonUpdatePacketCount = "PacketCount Error";
        public const string CommonUpdatePacketOrder = "PacketOrder Error";
        public const string CommonUpdatePacketNumber = "Packet Number Error";
        public const string CommonUpdateDataLength = "Data Length Error";
        public const string CommonUpdateDataWrite = "Data Write Error";
        public const string CommonUpdateCRC = "CRC Error";
        public const string CommonUpdateFresh = "Data Fresh Error";
        public const string CommonUpdateOther = "Other Error";
        public const string CommonUpdateUnknow = "Unknow Error";

        public const string ITCRC = "CRC Error";
        public const string ITLackParams = "Lack Params Error";
        public const string ITStudy = "Study Error";
        public const string ITParams = "Params Error";
        public const string ITFunction = "Function Error";
        public const string ITAddress= "Address Error";
        public const string ITPreCondition = "PreCondition Error";
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
        public static string GetTimeFail = Application.Current.Resources["GetTimeFail"].ToString();
        public static string GetTimeSuccess = Application.Current.Resources["GetTimeSuccess"].ToString();
        public static string SetTimeFail = Application.Current.Resources["SetTimeFail"].ToString();
        public static string SetTimeSuccess = Application.Current.Resources["SetTimeSuccess"].ToString();
        public static string ClearTiming = Application.Current.Resources["ClearTiming"].ToString();
        public static string ClearTimeFail = Application.Current.Resources["ClearTimeFail"].ToString();
        public static string ClearTimeSuccess = Application.Current.Resources["ClearTimeSuccess"].ToString();
        public static string SearchTimeFail = Application.Current.Resources["SearchTimeFail"].ToString();
        public static string SearchTimeSuccess = Application.Current.Resources["SearchTimeSuccess"].ToString();
        public static string SearchTimeGetNone = Application.Current.Resources["SearchTimeGetNone"].ToString();
        public static string Searching = Application.Current.Resources["Searching"].ToString();
        public static string SearchTimeEnd = Application.Current.Resources["SearchTimeEnd"].ToString();
        public static string GetVersion = Application.Current.Resources["GetVersion"].ToString();
        public static string CorrelationLow = Application.Current.Resources["CorrelationLow"].ToString();
        public static string CorrelationHigh = Application.Current.Resources["CorrelationHigh"].ToString();
        public static string CorrelationError = Application.Current.Resources["CorrelationError"].ToString();
        public static string CorrelationNoDataError = Application.Current.Resources["CorrelationNoDataError"].ToString();
        public static string ToSaveCorrelationData = Application.Current.Resources["ToSaveCorrelationData"].ToString();
        public static string ToSaveCorrelationData2 = Application.Current.Resources["ToSaveCorrelationData2"].ToString();
        public static string ToSaveCorrelationData3 = Application.Current.Resources["ToSaveCorrelationData3"].ToString();
        public static string RangeProfileError = Application.Current.Resources["RangeProfileError"].ToString();
        public static string PointDataError = Application.Current.Resources["PointDataError"].ToString();
        public static string RangeError = Application.Current.Resources["RangeError"].ToString();
        public static string ReasonAnomalousSearching = Application.Current.Resources["ReasonAnomalousSearching"].ToString();
        public static string ResetSuccess = Application.Current.Resources["TipResetSuccess"].ToString();
        public static string ResetFail = Application.Current.Resources["TipResetFail"].ToString();
        public static string ToReset = Application.Current.Resources["TipToReset"].ToString();
        public static string Reseting = Application.Current.Resources["TipReseting"].ToString();
        public static string KeepLifting = Application.Current.Resources["KeepLifting"].ToString();
        public static string Connect = Application.Current.Resources["Connect"].ToString();
        public static string Disconnect = Application.Current.Resources["Disconnect"].ToString();
        public static string StopShowPointsFirst = Application.Current.Resources["StopShowPointsFirst"].ToString();
        public static string NoTargets = Application.Current.Resources["NoTargets"].ToString();
        //public static string StayThreshold = Application.Current.Resources["StayThreshold"].ToString();
        public static string Threshold = Application.Current.Resources["Threshold"].ToString();
        //public static string TriggerBox = Application.Current.Resources["TriggerBox"].ToString();
        //public static string ClassifyBox = Application.Current.Resources["ClassifyBox"].ToString();
        //public static string Direction = Application.Current.Resources["Direction"].ToString();
        //public static string Count = Application.Current.Resources["Count"].ToString();
    }

    public class GateType
    {
        public static string Straight = Application.Current.Resources["Straight"].ToString();
        public static string FenceLeft = Application.Current.Resources["FenceLeft"].ToString();
        public static string FenceRight = Application.Current.Resources["FenceRight"].ToString();
        public static string AdvertisingLeft = Application.Current.Resources["AdvertisingLeft"].ToString();
        public static string AdvertisingRight = Application.Current.Resources["AdvertisingRight"].ToString();
        public static string Advertising = Application.Current.Resources["Advertising"].ToString();
        public static string Fence = Application.Current.Resources["Fence"].ToString();

        public static List<string> GetAllTypes()
        {
            return new List<string>() { Straight, AdvertisingLeft, AdvertisingRight, FenceLeft, FenceRight};
        }
        public static List<string> getAllTypesWithoutFence()
        {
            return new List<string>() { Straight, Fence };
        }
        public static string GetType(string value)
        {
            if (value == "0")
                return Straight;
            else if (value == "2")
                return Advertising;
            else if (value == "1")
                return Fence;
            else
                return value;
        }
        public static string GetTypeByPosition(string gate, string position)
        {
            if(position == "0")
            {
                if (gate == "2" || gate == Advertising)
                    return AdvertisingLeft;
                else if (gate == "1" || gate == Fence)
                    return FenceLeft;
            }
            else if(position == "1")
            {
                if (gate == "2" || gate == Advertising)
                    return AdvertisingRight;
                else if (gate == "1" || gate == Fence)
                    return FenceRight;
            }
            return gate + "|" + position;
        }
        public static string GetValue(string type)
        {
            if (type == Straight)
                return "0";
            else if(type == Advertising || type == Fence)
                return "1";
            else if (type == AdvertisingLeft || type == AdvertisingRight)
                return "2";
            else if (type == FenceLeft || type == FenceRight)
                return "1";
            else
                return type;
        }
    }

    public class RecordKind
    {
        public static string Persistence = Application.Current.Resources["Persistence"].ToString();
        public static string Ignore = Application.Current.Resources["Ignore"].ToString();

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
        public static string Up = Application.Current.Resources["Up"].ToString();
        public static string Down = Application.Current.Resources["Down"].ToString();

        public const string UpValue = @"Relay:1";
        public const string DownValue = @"Relay:0";
        public const string Value = @"Relay:";
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
                return "3150";
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
            try
            {
                int val = int.Parse(value);
                if (val < 3150)
                    return High;
                else if (val < 4500)
                    return Middle;
                else
                    return Low;
            }
            catch (Exception)
            {
                return value;
            }
        }
    }

    public class FencePositionType
    {
        public static string Left = Application.Current.Resources["PositionLeft"].ToString();
        public static string Right = Application.Current.Resources["PositionRight"].ToString();

        public static List<string> GetAllTypes()
        {
            return new List<string>() { Left, Right };
        }

        public static string GetType(string value)
        {
            if (value == "0")
                return Left;
            else if (value == "1")
                return Right;
            else
                return value;
        }
        public static string GetValue(string type)
        {
            if (type == Left || type == GateType.AdvertisingLeft || type == GateType.FenceLeft)
                return "0";
            else if (type == Right || type == GateType.AdvertisingRight || type == GateType.FenceRight)
                return "1";
            else
                return type;
        }
    }
    public class DirectionType
    {
        public static string Left = Application.Current.Resources["DirectionLeft"].ToString();
        public static string Right = Application.Current.Resources["DirectionRight"].ToString();

        public static List<string> GetAllTypes()
        {
            return new List<string>() { Left, Right };
        }

        public static string GetType(string value)
        {
            if (value.Contains('2'))
                return Left;
            else if (value.Contains('1'))
                return Right;
            else
                return value;
        }
        public static string GetValue(string type)
        {
            if (type == Left)
                return "2";
            else if (type == Right)
                return "1";
            else
                return type;
        }
    }

    public class CountType
    {
        public static string Start = Application.Current.Resources["CountStart"].ToString();
        public static string Stop = Application.Current.Resources["CountStop"].ToString();

        public static List<string> GetAllTypes()
        {
            return new List<string>() { Start, Stop };
        }

        public static string GetType(string value)
        {
            if (value == "0")
                return Stop;
            else if (value == "1")
                return Start;
            else
                return value;
        }
        public static string GetValue(string type)
        {
            if (type == Stop)
                return "0";
            else if (type == Start)
                return "1";
            else
                return type;
        }
    }
}
