using MbitGate.helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Controls;
using HexProtocol = MbitGate.helper.CommHexProtocolDecoder;

namespace MbitGate.control
{
    public class FileIOManager
    {
        private FileStream _stream;
        private string _path;
        private FileMode _mode;
        private FileAccess _access;
        private FileShare _share;

        public long Length { get; set; }
        public FileIOManager()
        {
        }
        public FileIOManager(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.ReadWrite)
        {
            Length = -1;
            _mode = mode;
            _access = access;
            _share = share;
            Path = path;
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (value == null)
                {
                    Length = -1;
                    _stream = null;
                    return;
                }
                if (_path != value)
                {
                    if (_stream != null)
                        _stream.Close();
                }
                _path = value;
                string dir = value.Substring(0, value.LastIndexOf('\\'));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                try
                {
                    _stream = new FileStream(value, _mode, _access, _share);
                    _stream.Seek(0, SeekOrigin.End);
                    Length = _stream.Length;
                    _stream.Seek(0, SeekOrigin.Begin);
                }
                catch(Exception e)
                {
                    Length = -1;
                    _stream = null;
                }
                
            }
        }

        public bool IsOpend()
        {
            return !(Length == -1);
        }

        public void Write(string content)
        {
            _stream.Write(System.Text.Encoding.Default.GetBytes(content), 0, content.Length);
            _stream.Flush();
        }

        public void WriteLine(string content)
        {
            _stream.Write(System.Text.Encoding.Default.GetBytes(content + "\r\n"), 0, content.Length);
            _stream.Flush();
        }

        public void ReWrite(string content)
        {
            _stream.Close();
            _stream = new FileStream(_path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            Write(content);
            _stream.Close();
            _stream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            _stream.Seek(0, SeekOrigin.End);
        }

        public string Read(long size = -1)
        {
            StringBuilder _strBuilder = new StringBuilder();
            byte[] content = new byte[1024 * 1024];
            if (size == -1) size = Length;
            int loopCount = (int)size / (1024 * 1024);
            int i = 1;
            for (; i < loopCount+1; i++)
            {
                _stream.Read(content, (i-1)*1024*1024, 1024 * 1024);
                _strBuilder.Append(System.Text.Encoding.Default.GetString(content));
                Array.Clear(content, 0, 1024 * 1024);
            }
            loopCount = (int)size % (1024 * 1024);
            content = new byte[loopCount];
            _stream.Read(content, (i-1)*1024*1024, loopCount);
            _strBuilder.Append(System.Text.Encoding.Default.GetString(content));

            return _strBuilder.ToString();
        }

        public byte[] ReadBytes(long size = -1)
        {
            if (size == -1) size = Length;
            byte[] content = new byte[size];
            int loopCount = (int)size / (1024 * 1024);
            int i = 1;
            for (; i < loopCount + 1; i++)
            {
                _stream.Read(content, (i-1) * 1024 * 1024, 1024 * 1024);
            }
            loopCount = (int)size % (1024 * 1024);
            content = new byte[loopCount];
            _stream.Read(content, (i - 1) * 1024 * 1024, loopCount);
            return content;
        }

        public string ReadLine(int pos = 0)
        {
            StreamReader reader = new StreamReader(_stream);
            int loop = 0;
            while (loop < pos)
            {
                reader.ReadLine();
            }
            string result = reader.ReadLine();
            reader.Close();
            return result;
        }

        public void Seek(int position)
        {
            _stream.Seek(position, SeekOrigin.Begin);
        }

        public void Close()
        {
            _stream.Close();
        }
    }
    public enum SerialReceiveType
    {
        Bytes,
        Chars
    }
    public class SerialManager
    {
        private System.IO.Ports.SerialPort _serial;
        CommHexProtocolDecoder hexDecoder;
        CommStringProtocolDecoder stringDecoder;
        Translater translater;
        Timer overTimer = null;
        ConcurrentQueue<byte> ReplayBytesQueue;
        string preStrCommand;
        byte[] preHexCommand;
        int preWaitMilliseconds;
        byte repeat;
        const byte MaxRepeatCount = 4;
        //为了兼容以前的字符串命令，设置一个标志位，表示是否需要将字符串命令翻译成十六进制命令
        private bool _translate = false;
        internal bool ToTranslate { get => _translate;
            set
            {
                _translate = value;
                if(value)
                    Type = SerialReceiveType.Bytes;
            } }
        internal bool CheckFuncCode { get; set; }
        public SerialManager(SerialReceiveType type = SerialReceiveType.Chars)
        {
            _serial = new System.IO.Ports.SerialPort();
            _serial.BaudRate = 921600;
            _serial.DataBits = 8;
            _serial.StopBits = System.IO.Ports.StopBits.One;
            _serial.Parity = System.IO.Ports.Parity.None;
            _serial.DtrEnable = true;
            _serial.RtsEnable = false;
            _serial.ReceivedBytesThreshold = 2;
            _serial.ReadTimeout = 1000;
            _serial.DataReceived += OnSerialDataReceived;
            stringDecoder = new CommStringProtocolDecoder();
            hexDecoder = new CommHexProtocolDecoder();
            translater = new Translater();
            stringDecoder.NotifyDecodeResult = OnStringResultReach;
            hexDecoder.NotifyCRCError = OnHexResultCRCFail;
            hexDecoder.NotifyReplayError = OnHexResultError;
            hexDecoder.NotifyFullResult = OnHexFrameReach;
            hexDecoder.NotifyDecodeResult = OnHexResultReach;

            Type = type;
            EndStr = "Done";
            EndBytes = new byte[] { };
            CompareEndString = true;

            overTimer = new Timer(300);
            overTimer.Elapsed += OverTimer_Elapsed;

            ReplayBytesQueue = new ConcurrentQueue<byte>();
            preStrCommand = null;
            preHexCommand = null;
            repeat = 0;

            System.Threading.Tasks.Task.Factory.StartNew(() => { DecodeLoop(4); });

            hexDecoder.MaxCantDecodeCount = ((300 / 4) + 10) < 0 ? 1 : ((300/4) + 10);  /*300 = overTimer.Interval   4=consumerTimer.Interval*/
        }

        //十六进制与字符串统一回复出口
        public Action<string> ReplayReachHandler { get; set; }
        public Action<string> StringDataReceivedHandler { get; set; }
        public Action<byte[]> BytesFrameDataReceivedHandler { get; set; }
        public Action<Tuple<byte/*address*/, byte/*error*/, ushort/*function*/, byte[]/*data*/>> BytesDecodedDataReceivedHandler { get; set; }
        public Action<byte> ErrorReceivedHandler { get; set; }

        public SerialManager(string name, SerialReceiveType type = SerialReceiveType.Chars)
        {
            _serial = new System.IO.Ports.SerialPort();
            _serial.BaudRate = 921600;
            _serial.DataBits = 8;
            _serial.StopBits = System.IO.Ports.StopBits.One;
            _serial.Parity = System.IO.Ports.Parity.None;
            _serial.DtrEnable = true;
            _serial.RtsEnable = false;
            //_serial.ReceivedBytesThreshold = 2;
            _serial.ReadTimeout = 1000;
            PortName = name;
            _serial.DataReceived += OnSerialDataReceived;
            stringDecoder = new CommStringProtocolDecoder();
            hexDecoder = new CommHexProtocolDecoder();
            translater = new Translater();
            stringDecoder.NotifyDecodeResult = OnStringResultReach;
            hexDecoder.NotifyCRCError = OnHexResultCRCFail;
            hexDecoder.NotifyReplayError = OnHexResultError;
            hexDecoder.NotifyFullResult = OnHexFrameReach;
            hexDecoder.NotifyDecodeResult = OnHexResultReach;

            Type = type;
            EndStr = "Done";
            EndBytes = new byte[] { };
            CompareEndString = true;

            overTimer = new Timer(300);
            overTimer.AutoReset = true;
            overTimer.Elapsed += OverTimer_Elapsed;

            ReplayBytesQueue = new ConcurrentQueue<byte>();
            preStrCommand = null;
            preHexCommand = null;
            repeat = 0;

            System.Threading.Tasks.Task.Factory.StartNew(() => { DecodeLoop(4); });
            hexDecoder.MaxCantDecodeCount = ((300 / 4) + 10) < 0 ? 1 : ((300 / 4) + 10);   /*300 = overTimer.Interval   4=consumerTimer.Interval*/
        }

        public string EndStr { 
            get=>stringDecoder.CustomFoot; set=>stringDecoder.CustomFoot=value; 
        }
        public byte[] EndBytes { get; set; }

        public bool CompareEndString { 
            get=>stringDecoder.CompareEndString; set=>stringDecoder.CompareEndString=value; 
        }

        public bool DecodeFrame { get; set; }
        public int CompareEndBytesCount { 
            get=>_serial.ReceivedBytesThreshold; set=>_serial.ReceivedBytesThreshold = value; }
        private void OnSerialDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                switch (Type)
                {
                    case SerialReceiveType.Bytes:
                        byte[] data = new byte[_serial.BytesToRead];
                        _serial.Read(data, 0, data.Length);
                        //System.Console.WriteLine("====================" + BitConverter.ToString(data));
                        if(DecodeFrame || ToTranslate)
                        {
                            foreach (byte val in data)
                            {
                                ReplayBytesQueue.Enqueue(val);
                            }
                        }
                        else
                        {
                            overTimer.Stop();
                            BytesFrameDataReceivedHandler?.Invoke(data);
                        }
                        break;
                    case SerialReceiveType.Chars:
                        string content = _serial.ReadExisting();
                        stringDecoder.Decode(ref content);
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        bool toStopDecode = false;
        private void DecodeLoop(short intervalMillisecondsf)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            long ticks = 0;
            while (!toStopDecode)
            {
                if(watch.ElapsedMilliseconds >= ticks+ intervalMillisecondsf)
                {
                    ticks = watch.ElapsedMilliseconds;
                    hexDecoder.Decode(ref ReplayBytesQueue);
                }
            }
        }

        private void OverTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //重发机制
           if(preStrCommand != null)
            {
                if(MaxRepeatCount == repeat)
                {
                    overTimer.Stop();
                    preStrCommand = null;
                    repeat = 0;
                    StringDataReceivedHandler?.Invoke(ErrorString.Error + ErrorString.OverTime);
                }
                else
                {
                    WriteLine(preStrCommand, preWaitMilliseconds, false);
                    System.Console.WriteLine("=================ReWrite " + preStrCommand);
                }
            }
           else if(preHexCommand != null)
            {
                if (MaxRepeatCount == repeat)
                {
                    overTimer.Stop();
                    preHexCommand = null;
                    repeat = 0;
                    if (ToTranslate)
                        StringDataReceivedHandler?.Invoke(ErrorString.Error + ErrorString.OverTime);
                    else
                        ErrorReceivedHandler?.Invoke(HexProtocol.ERROR_COMMON_OVERTIME);
                }
                else
                {
                    Write(preHexCommand, preWaitMilliseconds, false);
                    System.Console.WriteLine("=================ReWrite " + BitConverter.ToString(preHexCommand));
                }
            }
            repeat++;
        }

        private void OnHexFrameReach(byte[] frame)
        {
            preHexCommand = null;
            overTimer.Stop();
            repeat = 0;
            if(ToTranslate)
            {
                StringDataReceivedHandler?.Invoke(translater.Translate(CommHexProtocolDecoder.DecodeFrame(frame)));
            }
            else
                BytesFrameDataReceivedHandler?.Invoke(frame);
        }

        private void OnHexResultReach(Tuple<byte, byte, ushort, byte[]> data)
        {
            if(CheckFunctionCode(data.Item3))
            {
                if(preWaitMilliseconds != -1)
                    preHexCommand = null;
                overTimer.Stop();
                repeat = 0;

                if (ToTranslate)
                {
                    StringDataReceivedHandler?.Invoke(translater.Translate(data));
                }
                else
                    BytesDecodedDataReceivedHandler?.Invoke(data);
            }
        }

        private void OnHexResultError(ushort func, byte err)
        {
            if(CheckFunctionCode(func))
            {
                overTimer.Stop();
                preHexCommand = null;
                repeat = 0;
                if (ToTranslate)
                {
                    StringDataReceivedHandler?.Invoke(translater.TranslateErrorCode(func, err));
                }
                else
                    ErrorReceivedHandler(err);
            }
        }

        private bool CheckFunctionCode(ushort func)
        {
            return CheckFuncCode?(preHexCommand==null?false:CommHexProtocolDecoder.CheckFunctionCode(ref preHexCommand, func)):true;
        }

        private void OnHexResultCRCFail()
        {
            //overtime to repeat
            System.Console.WriteLine("=================OnHexResultCRCFail=================");
        }

        private void OnStringResultReach(string replay)
        {
            overTimer.Stop();
            preStrCommand = null;
            repeat = 0;
            StringDataReceivedHandler?.Invoke(replay);
        }
        public bool IsOpen
        {
            get
            {
                if (_serial != null)
                    return _serial.IsOpen;
                return false;
            }
        }

        public bool open()
        {
            try
            {
                if (_serial == null)
                    return false;
                if (_serial.IsOpen == false)
                    _serial.Open();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public int Rate
        {
            get
            {
                return _serial.BaudRate;
            }
            set
            {
                _serial.BaudRate = value;
            }
        }
        public int Bits
        {
            get
            {
                return _serial.DataBits;
            }
            set
            {
                _serial.DataBits = value;
            }
        }
        public int TimeOut
        {
            get
            {
                return _serial.ReadTimeout;
            }
            set
            {
                _serial.ReadTimeout = value;
            }
        }
        public string PortName
        {
            get
            {
                return _serial.PortName;
            }
            set
            {
                _serial.PortName = value;
            }
        }

        SerialReceiveType _type;

        public SerialReceiveType Type { 
            get => _type; 
            set {
                if (ToTranslate)
                    _type = SerialReceiveType.Bytes;
                else
                    _type = value;
            } 
        }

        public void WriteLine(string content, int milliseconds = 300, bool toStartTime = true)
        {
            try
            {
                lock(_serial)
                {
                    if(ToTranslate)
                    {
                        preHexCommand = translater.Translate(content);
                        preWaitMilliseconds = milliseconds;
                        if(preHexCommand == null)
                        {
                            StringDataReceivedHandler?.Invoke(ErrorString.Error + ErrorString.TranslateError);
                        }
                        else
                            Write(preHexCommand, milliseconds, toStartTime);
                    }
                    else
                    {
                        preStrCommand = content;
                        preHexCommand = null;
                        preWaitMilliseconds = milliseconds;
                        _serial.DiscardOutBuffer();
                        _serial.WriteLine(content);
                        if(milliseconds > 0)
                        {
                            overTimer.Interval = milliseconds;
                            if (toStartTime)
                                overTimer.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _serial.Close();
            }
        }

        public void Write(byte[] content, int milliseconds = 300, bool toStartTimer = true)
        {
            try
            {
                lock(_serial)
                {
                    preHexCommand = content;
                    preStrCommand = null;
                    preWaitMilliseconds = milliseconds;
                    _serial.Write(content, 0, content.Length);
                    if(milliseconds > 0)
                    {
                        overTimer.Interval = milliseconds;
                        if (toStartTimer)
                            overTimer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                _serial.Close();
            }
        }

        public string ReadExisting()
        {
            return _serial.ReadExisting();
        }

        public void close()
        {
            try
            {
                if(_serial != null)
                {
                    _serial.Close();
                    _serial.Dispose();
                }
                StringDataReceivedHandler = null;
                BytesFrameDataReceivedHandler = null;
                BytesDecodedDataReceivedHandler = null;
                ErrorReceivedHandler = null;
            }
            catch{ }
        }
        public void dispose()
        {
            close();
            toStopDecode = true;
        }
        internal void ClearBuffer()
        {
            lock(ReplayBytesQueue)
            {
                byte tmp;
                while (ReplayBytesQueue.TryDequeue(out tmp)) { }
            }
            stringDecoder.Clear();
        }
    }
}
