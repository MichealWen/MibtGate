using MbitGate.helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

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
            for (int i = 1; i < loopCount + 1; i++)
            {
                _stream.Read(content, 0, 1024 * 1024);
                _strBuilder.Append(System.Text.Encoding.Default.GetString(content));
                Array.Clear(content, 0, 1024 * 1024);
            }
            loopCount = (int)size % (1024 * 1024);
            _stream.Read(content, 0, loopCount);
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
                _stream.Read(content, i * 1024 * 1024, 1024 * 1024);
            }
            loopCount = (int)size % (1024 * 1024);
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
        CommunicateHexProtocolDecoder hexDecoder;
        CommunicateStringProtocolDecoder stringDecoder;
        Timer overTimer = null;
        Timer consumerTimer = null;
        ConcurrentQueue<byte> ReplayData;
        string preStrCommand;
        byte[] preHexCommand;
        byte repeat;
        const byte MaxRepeatCount = 3;
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
            stringDecoder = new CommunicateStringProtocolDecoder();
            hexDecoder = new CommunicateHexProtocolDecoder();
            stringDecoder.NotifyDecodeResult = OnStringResultReach;
            hexDecoder.NotifyCRCError = OnHexResultCRCFail;
            hexDecoder.NotifyReplayError = OnHexResultError;
            hexDecoder.NotifyFullResult = OnHexFullResultReach;

            Type = type;
            EndStr = "Done";
            EndBytes = new byte[] { };
            CompareEndString = true;
            CompareEndBytesCount = 1;

            overTimer = new Timer(300);
            overTimer.Elapsed += OverTimer_Elapsed;

            ReplayData = new ConcurrentQueue<byte>();
            preStrCommand = null;
            preHexCommand = null;
            repeat = 0;

            consumerTimer = new System.Timers.Timer(100);
            consumerTimer.Elapsed += ToDecodeTimer_Elapsed;
            consumerTimer.Start();
        }

        public Action<string> StringDataReceivedHandler { get; set; }
        public Action<byte[]> BytesDataReceivedHandler { get; set; }

        public SerialManager(string name, SerialReceiveType type = SerialReceiveType.Chars)
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
            PortName = name;
            _serial.DataReceived += OnSerialDataReceived;
            stringDecoder = new CommunicateStringProtocolDecoder();
            hexDecoder = new CommunicateHexProtocolDecoder();
            stringDecoder.NotifyDecodeResult = OnStringResultReach;
            hexDecoder.NotifyCRCError = OnHexResultCRCFail;
            hexDecoder.NotifyReplayError = OnHexResultError;
            hexDecoder.NotifyFullResult = OnHexFullResultReach;

            Type = type;
            EndStr = "Done";
            EndBytes = new byte[] { };
            CompareEndString = true;
            CompareEndBytesCount = 1;

            overTimer = new Timer(300);
            overTimer.Elapsed += OverTimer_Elapsed;

            ReplayData = new ConcurrentQueue<byte>();
            preStrCommand = null;
            preHexCommand = null;
            repeat = 0;

            consumerTimer = new System.Timers.Timer(100);
            consumerTimer.Elapsed += ToDecodeTimer_Elapsed;
            consumerTimer.Start();
        }

        public string EndStr { 
            get=>stringDecoder.CustomFoot; set=>stringDecoder.CustomFoot=value; 
        }
        public byte[] EndBytes { get; set; }

        public bool CompareEndString { 
            get=>stringDecoder.CompareEndString; set=>stringDecoder.CompareEndString=value; 
        }
        public int CompareEndBytesCount { get; set; }
        private void OnSerialDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                switch (Type)
                {
                    case SerialReceiveType.Bytes:
                        byte[] data = new byte[_serial.BytesToRead];
                        _serial.Read(data, 0, data.Length);
                        foreach (byte val in data)
                        {
                            ReplayData.Enqueue(val);
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

        private void ToDecodeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            hexDecoder.Decode(ref ReplayData);
        }

        private void OverTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
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
                    WriteLine(preStrCommand);
            }
           else if(preHexCommand != null)
            {
                if (MaxRepeatCount == repeat)
                {
                    overTimer.Stop();
                    preHexCommand = null;
                    repeat = 0;
                    preHexCommand[3] = CommunicateHexProtocolDecoder.ERROVERTIME;
                    BytesDataReceivedHandler?.Invoke(preHexCommand);
                }
                else
                    Write(preHexCommand);
            }
            repeat++;
        }

        private void OnHexFullResultReach(byte[] frame)
        {
            preHexCommand = null;
            overTimer.Stop();
            repeat = 0;
            BytesDataReceivedHandler?.Invoke(frame);
        }

        private void OnHexResultReach(Tuple<byte, byte, ushort, byte[]> data)
        {
            preHexCommand = null;
            overTimer.Stop();
            repeat = 0;
            BytesDataReceivedHandler?.Invoke(data.Item4);
        }

        private void OnHexResultError(byte err)
        {
            preHexCommand[3] = err;
            BytesDataReceivedHandler?.Invoke(preHexCommand);
            overTimer.Stop();
            preHexCommand = null;
            repeat = 0;
        }

        private void OnHexResultCRCFail()
        {
            //overtime to repeat
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
                if (_serial != null)
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

        public SerialReceiveType Type { get; set; }

        public void WriteLine(string content, int millis = 0, bool toStartTime = true)
        {
            try
            {
                preStrCommand = content;
                _serial.DiscardOutBuffer();
                _serial.WriteLine(content);
                System.Threading.Thread.Sleep(millis);
                //if (content == SerialRadarCommands.SoftReset)
                //    overTimer.Interval = 1000;
                //else
                //    overTimer.Interval = 300;
                if(toStartTime)
                    overTimer.Start();
            }
            catch (Exception ex)
            {
                _serial.Close();
            }
        }

        public void Write(byte[] content, int millis = 0)
        {
            try
            {
                preHexCommand = content;
                _serial.Write(content, 0, content.Length);
                System.Threading.Thread.Sleep(millis);
                overTimer.Start();
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
                    _serial.Close();
                StringDataReceivedHandler = null;
                BytesDataReceivedHandler = null;
            }
            catch{ }
        }
    }
}
