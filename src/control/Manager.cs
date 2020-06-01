using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        public SerialManager(SerialReceiveType type = SerialReceiveType.Chars)
        {
            _serial = new System.IO.Ports.SerialPort();
            _serial.BaudRate = 921600;
            _serial.DataBits = 8;
            _serial.StopBits = System.IO.Ports.StopBits.One;
            _serial.Parity = System.IO.Ports.Parity.None;
            _serial.DtrEnable = false;
            _serial.RtsEnable = false;
            _serial.ReceivedBytesThreshold = 2;
            _serial.ReadTimeout = 1000;
            _serial.DataReceived += OnSerialDataReceived;
            Type = type;
            EndStr = "Done";
            EndBytes = new byte[] { };
            CompareEndString = true;
            CompareEndBytesCount = 1;
        }

        public Action<string> DataReceivedHandler { get; set; }
        public Action<byte[]> BytesDataReceivedHandler { get; set; }

        public SerialManager(string name, SerialReceiveType type = SerialReceiveType.Chars)
        {
            _serial = new System.IO.Ports.SerialPort();
            _serial.BaudRate = 921600;
            _serial.DataBits = 8;
            _serial.StopBits = System.IO.Ports.StopBits.One;
            _serial.Parity = System.IO.Ports.Parity.None;
            _serial.DtrEnable = false;
            _serial.RtsEnable = false;
            _serial.ReceivedBytesThreshold = 2;
            _serial.ReadTimeout = 1000;
            PortName = name;
            _serial.DataReceived += OnSerialDataReceived;
            Type = type;
            EndStr = "Done";
            EndBytes = new byte[] { };
            CompareEndString = true;
            CompareEndBytesCount = 1;
        }
        private string _receivedStr = string.Empty;

        public string EndStr { get; set; }
        public byte[] EndBytes { get; set; }

        public bool CompareEndString { get; set; }
        public int CompareEndBytesCount { get; set; }
        private void OnSerialDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                switch (Type)
                {
                    case SerialReceiveType.Bytes:
                        if (BytesDataReceivedHandler != null)
                        {
                            try
                            {
                                //if(_serial.BytesToRead >= CompareEndBytesCount)
                                //{
                                //    byte[] data = new byte[_serial.BytesToRead];
                                //    _serial.Read(data, 0, data.Length);
                                //    if (data.Length > 0)
                                //    {
                                //        BytesDataReceivedHandler(data);
                                //    }
                                //}
                                byte[] data = new byte[_serial.BytesToRead];
                                _serial.Read(data, 0, data.Length);
                                //if (EndBytes.Length > 0)
                                //    for (int i = (data.Length - EndBytes.Length), j = 0; i < data.Length; i++, j++)
                                //    {
                                //        if (data[i] != EndBytes[j])
                                //            return;
                                //    }
                                BytesDataReceivedHandler(data);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        break;
                    case SerialReceiveType.Chars:
                        if (DataReceivedHandler != null)
                        {
                            while (_serial.BytesToRead > 0)
                            {
                                _receivedStr += _serial.ReadExisting();
                                if(CompareEndString)
                                {
                                    if (_receivedStr.Contains("Error") || _receivedStr.Contains(EndStr))
                                    {
                                        DataReceivedHandler(_receivedStr);
                                        _receivedStr = string.Empty;
                                    }
                                }
                                else
                                {
                                    DataReceivedHandler(_receivedStr);
                                    _receivedStr = string.Empty;
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _receivedStr = string.Empty;
            }
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

        public void WriteLine(string content, int millis = 0)
        {
            try
            {
                _serial.DiscardOutBuffer();
                _serial.WriteLine(content);
                System.Threading.Thread.Sleep(millis);
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
                _serial.Write(content, 0, content.Length);
                System.Threading.Thread.Sleep(millis);
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
                _receivedStr = string.Empty;
                DataReceivedHandler = null;
                BytesDataReceivedHandler = null;
            }
            catch{ }
        }
    }
}
