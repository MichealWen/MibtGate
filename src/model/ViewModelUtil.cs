using MbitGate.control;
using MbitGate.helper;
using MbitGate.views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Linq;
using LiveCharts.Defaults;
using LiveCharts;

namespace MbitGate.model
{
    public class ConfigViewModel
    {
        readonly ICommand _cancelCommand;
        readonly ICommand _confirmCommand;

        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand;
            }
        }

        public ICommand ConfirmCommand
        {
            get
            {
                return _confirmCommand;
            }
        }

        public ConfigViewModel(Action<object> _cancel, Action<object> _confirm)
        {
            _cancelCommand = new SimpleCommand()
            {
                ExecuteDelegate = _cancel
            };

            _confirmCommand = new SimpleCommand()
            {
                ExecuteDelegate = _confirm
            };

            RefreshItems = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    GetSerials();
                }
            };

            Items = new ObservableCollection<string>();

            SelectedRate = Rates.Count > 10 ? Rates[10] : Rates.Count > 0 ? Rates[0] : new Rate();
            SelectedRadarType = Application.Current.Resources["RadarHoldType"].ToString();
        }

        public List<string> Types
        {
            get =>new List<string>(){ Application.Current.Resources["RadarHoldType"].ToString(), Application.Current.Resources["RadarTriggerType"].ToString()};
        }  
        
        public string SelectedRadarType
        {
            get;set;
        }
        public ObservableCollection<string> Items { get; set; }

        private List<Rate> _rates = null;
        public List<Rate> Rates
        {
            get
            {
                if (_rates == null)
                {
                    _rates = new List<Rate>();
                    BauRate.GetReates().ForEach(rate => _rates.Add(new Rate() { RateKey = rate, RateValue = uint.Parse(rate) }));
                }
                return _rates;
            }
        }

        public uint CustomRate { get; set; }
        public string CustomItem { get; set; }
        private Rate _selectedRate;
        public Rate SelectedRate
        {
            get
            {
                return _selectedRate;
            }
            set
            {
                if (value != null)
                {
                    _selectedRate = value;
                    CustomRate = value.RateValue;
                }
            }
        }

        public ICommand RefreshItems { get; set; }

        private void GetSerials()
        {
            Items.Clear();
            foreach (string port in helper.SerialProtFindHelper.GetSerialPort())
            {
                int startpos = port.LastIndexOf('(');
                int endpos = port.LastIndexOf(')');
                string tmpPrefix = port.Substring(startpos + 1, endpos - startpos - 1);
                string postExt = port.Substring(0, startpos);
                Items.Add(tmpPrefix + "  " + postExt);
            }
        }
    }

    public class PwdViewModel
    {
        public string Password { get; set; }
        public bool verify()
        {
            if(Password != null && Password == "Mbit")
            {
                return true;
            }
            return false;
        }

        public ICommand CheckCommand { get; set; }
        public ICommand CancelCommand { get; set; }
    }
    public class ProgressViewModel : NotifyPropertyChangedBase
    {
        string _message = string.Empty;
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("Message"); }
        }

        public ICommand CancelCommand { get; set; }

        public ICommand ConfirmCommand { get; set; }

        private uint _max;
        public uint MaxValue
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
                OnPropertyChanged("MaxValue");
            }
        }
        public uint _value;
        public uint Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private bool _indeterminate = false;
        public bool IsIndeterminate
        {
            get
            {
                return _indeterminate;
            }
            set
            {
                _indeterminate = value;
                OnPropertyChanged("IsIndeterminate");
            }
        }
    }

    public class MessageDialogViewModel
    {
        public ICommand Confirm { get; set; }
        public ICommand Cancel { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
    public class MainViewModel : NotifyPropertyChangedBase
    {
        protected readonly MahApps.Metro.Controls.Dialogs.IDialogCoordinator _dialogCoordinator;
        protected ProgressViewModel _progressViewModel;
        public ICommand UpdateCmd { get; set; }
        protected ProgressControl _progressCtrl = null;

        public string ItemSourceName { get; set; }
        public ObservableCollection<string> Items { get; set; }

        public bool RateEditable { get; set; }

        public MainViewModel(MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            _progressCtrl = new ProgressControl();
            _progressViewModel = new ProgressViewModel()
            {
                CancelCommand = new SimpleCommand()
                {
                    ExecuteDelegate = param =>
                    {
                        _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                    }
                }
            };
            _progressCtrl.DataViewModel = _progressViewModel;

            UpdateCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    ToDo();
                }
            };
            Items = new ObservableCollection<string>();
        }

        #region Util Method

        protected void CreateBackgroundWorker(DoWorkEventHandler work, RunWorkerCompletedEventHandler completed = null)
        {
            BackgroundWorker _worker = new BackgroundWorker();
            _worker.DoWork += work;
            _worker.RunWorkerCompleted += completed;
            _worker.RunWorkerAsync();
        }

        protected void ShowSplashWindow(string message, int millionSecond = 0, Action work = null, Action successCompleted = null, Action failCompleted = null)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                _progressViewModel.Message = message;
                _progressViewModel.IsIndeterminate = true;
                if(!_progressCtrl.IsVisible)
                    await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
                await TaskEx.Delay(millionSecond);

                CreateBackgroundWorker(
                    (sender, args) =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                        {
                            if (_progressCtrl.IsVisible)
                                if(work != null)
                                    work();
                        }));
                    },
                    (sender, args) =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
                        {
                            if (_progressCtrl.IsVisible)
                            {
                                await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                                if (successCompleted != null)
                                    successCompleted();
                            }
                            else
                            {
                                if (failCompleted != null)
                                    failCompleted();
                            }
                        }));
                    }
                );
            }));
        }

        protected void ShowSplashWindow(string message, Action work = null)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                _progressViewModel.Message = message;
                _progressViewModel.IsIndeterminate = true;
                await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);

                CreateBackgroundWorker(
                    (sender, args) =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                        {
                            if (_progressCtrl.IsVisible)
                                work();
                        }));
                    }
                );
            }));
        }
        protected void ShowWarnWindow(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                MessageControl _dialog = new MessageControl();
                _dialog.DataContext = new MessageDialogViewModel()
                {
                    Title = Application.Current.Resources["Warnning"].ToString(),
                    Message = message,
                    Confirm = new SimpleCommand()
                    {
                        ExecuteDelegate = param =>
                        {
                            _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                        }
                    }
                };

                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }));
        }

        protected void ShowConfirmWindow(string message, string title)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                MessageControl _dialog = new MessageControl();
                _dialog.DataContext = new MessageDialogViewModel()
                {
                    Title = title,
                    Message = message,
                    Confirm = new SimpleCommand()
                    {
                        ExecuteDelegate = param =>
                        {
                            _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                        }
                    }
                };

                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }));
        }
        protected void ShowErrorWindow(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                try
                {
                    MessageControl _dialog = new MessageControl();
                    _dialog.DataContext = new MessageDialogViewModel()
                    {
                        Title = Application.Current.Resources["Error"].ToString(),
                        Message = message,
                        Confirm = new SimpleCommand()
                        {
                            ExecuteDelegate = param =>
                            {
                                _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                            }
                        }
                    };

                    await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
                }
                catch (Exception e)
                {

                }
            }));
        }

        protected void ShowConfirmCancelWindow(string title, string message, SimpleCommand confirm = null, SimpleCommand cancel = null)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                ConfirmControl _dialog = new ConfirmControl();
                SimpleCommand _defaultCmd = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                    }
                };
                SimpleCommand _confirm = null, _cancel = null;

                if (confirm == null)
                {
                    _confirm = _defaultCmd;
                }
                else
                {
                    _confirm = new SimpleCommand()
                    {
                        ExecuteDelegate = async param =>
                        {
                            await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                            confirm.ExecuteDelegate(null);
                        }
                    };
                }
                if (cancel == null)
                {
                    _cancel = _defaultCmd;
                }
                else
                {
                    _cancel = new SimpleCommand()
                    {
                        ExecuteDelegate = async param =>
                        {
                            await _dialogCoordinator.HideMetroDialogAsync(this, _dialog);
                            cancel.ExecuteDelegate(null);
                        }
                    };
                }
                _dialog.DataContext = new MessageDialogViewModel()
                {
                    Title = title,
                    Message = message,
                    Confirm = _confirm,
                    Cancel = _cancel
                };
                await _dialogCoordinator.ShowMetroDialogAsync(this, _dialog);
            }));
        }
        #endregion
        public virtual void Dispose() { Items.Clear(); }
        protected virtual void ToDo() { }
        
    }
    public class CANMainViewModel : MainViewModel
    {
        public UpdateModel ConfigModel { get; set; }
        private Rate _selectedRate;
        public Rate SelectedRate
        {
            get
            {
                return _selectedRate;
            }
            set
            {
                if(value != null)
                {
                    ConfigModel.Rate = value.RateValue;
                    _selectedRate = value;
                    GetRadars();
                }
                
            }
        }
        private string _version;
        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged("Version");
            }
        }
        private ObservableCollection<Rate> _rates;
        public ObservableCollection<Rate> Rates
        {
            get
            {
                return _rates;
            }
        }

        public override void Dispose()
        {
            Items.Clear();
            if (_canger != null)
                _canger.Close();
        }

        CANManager _canger = null;


        uint[] radarID = { 0x201, 0x211, 0x221, 0x231, 0x241, 0x251, 0x261, 0x271, 0x281, 0x291, 0x2A1, 0x2B1, 0x2C1, 0x2D1, 0x2E1, 0x2F1 };
        uint[] replyRadarID = { 0x202, 0x212, 0x222, 0x232, 0x242, 0x252, 0x262, 0x272, 0x282, 0x292, 0x2A2, 0x2B2, 0x2C2, 0x2D2, 0x2E2, 0x2F2 };
        internal unsafe void GetRadars()
        {
            Items.Clear();
            if (_canger == null)
                _canger = new CANManager(Devices.VCI_USBCAN_2E_U, 0, 0, ConfigModel.Rate);
            if(_canger.IsStart)
            {
                foreach (uint id in radarID)
                {
                    //_canger.Send(id, new byte[] { 0x84, 0x01 });
                    string lastOp = CANRadarCommands.StopOutput;
                    _canger.DataReceivedHandler = data =>
                    {
                        switch (lastOp)
                        {
                            case CANRadarCommands.StopOutput:
                                foreach (VCI_CAN_OBJ obj in data)
                                {
                                    if (obj.Data[0] == 0x84)
                                    {
                                        lastOp = CANRadarCommands.Synchronize;
                                        _canger.Send(obj.ID - 1, new byte[] { 0x01, 0x00 });
                                        break;
                                    }
                                }
                                break;
                            case CANRadarCommands.Synchronize:
                                if (data[0].Data[0] == 129)
                                {
                                    byte radar = data[0].Data[1];
                                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { Items.Add(Convert.ToString(radar, 16)); }));
                                    lastOp = CANRadarCommands.Version;
                                    _canger.Send(data[0].ID - 1, new byte[] { 0x02, 0x00 });
                                }
                                break;
                            case CANRadarCommands.Version:
                                if (data[0].Data[0] == 0x82)
                                {
                                    Version = "V  " + BitConverter.ToString(new byte[] {data[0].Data[1], data[0].Data[2], data[0].Data[3] }).Replace("-", " : ");
                                    _canger.DataReceivedHandler = null;
                                    _canger.Send(data[0].ID - 1, new byte[] { 0x84, 0x01 });
                                    _canger.Close();
                                    _canger = null;
                                }
                                break;
                        }
                    };
                    _canger.Send(id, new byte[] { 0x84, 0x00 });
                }
            }
            else
            {
                ShowErrorWindow(ErrorString.CANOpenError);
                _canger.DataReceivedHandler = null;
                _canger.Close();
                _canger = null;
            }
        }

        public CANMainViewModel(MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator):base(dialogCoordinator)
        {
            _progressViewModel = new ProgressViewModel() { CancelCommand = new SimpleCommand() { ExecuteDelegate = param => 
                    {
                        _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                        if (_canger != null)
                        {
                            _canger.DataReceivedHandler = null;
                            _canger.Close();
                            _canger = null;
                        }
                    }
            } };
            _progressCtrl.DataViewModel = _progressViewModel;

            ConfigModel = new UpdateModel();
            RateEditable = false;
            ItemSourceName = Application.Current.Resources["Radar"].ToString();
            _rates = new ObservableCollection<Rate>() { new Rate() { RateKey = CANBauRate.Rate250, RateValue = CANBauRate.GetUINTRate(CANBauRate.Rate250) }, new Rate() { RateKey = CANBauRate.Rate500, RateValue = CANBauRate.GetUINTRate(CANBauRate.Rate500) }, new Rate() { RateKey = CANBauRate.Rate1000, RateValue = CANBauRate.GetUINTRate(CANBauRate.Rate1000) } };
            SelectedRate = _rates[0];
            RefreshItems = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    GetRadars();
                }
            };
        }

        public readonly int ReadByteSize = 8;
        protected async override void ToDo()
        {
            if (ConfigModel.BinPath == null || ConfigModel.BinPath == "")
            {
                ShowErrorWindow(ErrorString.BinPathError);
                return;
            }
            else if (!System.IO.File.Exists(ConfigModel.BinPath))
            {
                ShowErrorWindow(ErrorString.FileError);
                return;
            }
            else if (ConfigModel.Item == null || ConfigModel.Item == "")
            {
                ShowErrorWindow(ErrorString.RadarError);
                return;
            }
            else if (ConfigModel.Rate == 0)
            {
                ShowErrorWindow(ErrorString.ParamError);
                return;
            }
            else if(_canger == null)
            {
                _canger = new CANManager(Devices.VCI_USBCAN_2E_U, 0, 0, ConfigModel.Rate);
            }
            if (!_canger.IsStart)
            {
                ShowErrorWindow(ErrorString.CANOpenError);
                return;
            }

            _progressViewModel.Message = Tips.Initializing;
            _progressViewModel.IsIndeterminate = true;
            _progressViewModel.MaxValue = 100;
            _progressViewModel.Value = 0;
            await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
            CreateBackgroundWorker(
                (sender, args) =>
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                    {
                        if (_progressCtrl.IsVisible)
                        {
                            FileIOManager reader = new FileIOManager(ConfigModel.BinPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                            _progressViewModel.MaxValue = (uint)reader.Length;
                            uint sum = 0, pos = 0;
                            byte[] filePrefix = new byte[8];
                            uint sendID = Convert.ToUInt16("0x2" + ConfigModel.Item + "1", 16);
                            //uint radar = (uint)(0x300 + Convert.ToByte(ConfigModel.Item, 16) * 0x10);
                            uint radar = (uint)0x300;
                            string lastOperation = CANRadarCommands.Synchronize;
                            _canger.DataReceivedHandler = 
                                data =>
                                    {
                                        unsafe
                                        {
                                            bool result = true;
                                            switch (lastOperation)
                                            {
                                                case CANRadarCommands.StopOutput:
                                                    foreach (VCI_CAN_OBJ obj in data)
                                                    {
                                                        if(obj.ID == sendID + 1)
                                                        {
                                                            if (obj.Data[0] == 0x84)
                                                            {
                                                                lastOperation = CANRadarCommands.Synchronize;
                                                                result = _canger.Send(radar, new byte[] { 0x01 });
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case CANRadarCommands.Synchronize:
                                                    foreach(VCI_CAN_OBJ obj in data)
                                                    {
                                                        if (obj.ID == radar + 1)
                                                        {
                                                            if (obj.Data[0] == 0x02)
                                                            {
                                                                lastOperation = CANRadarCommands.CMDFILEPATHCRC;
                                                                byte[] bytePathCRCArray = new byte[5] { 0x03, 0x00, 0x00, 0x00, 0x00 };
                                                                byte[] bytePathLen = BitConverter.GetBytes(reader.Length - 8);
                                                                Array.Reverse(bytePathLen);
                                                                Array.Copy(bytePathLen, 4, bytePathCRCArray, 1, 4);
                                                                result = _canger.Send(radar, bytePathCRCArray);
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case CANRadarCommands.CMDFILEPATHCRC:
                                                    _progressViewModel.Message = Tips.Flashing;
                                                    lastOperation = CANRadarCommands.CMDFlashErase;
                                                    if (data[0].Data[0] == 0x04)
                                                    {
                                                        result = _canger.Send(radar, new byte[] { 0x06 });
                                                    }
                                                    break;
                                                case CANRadarCommands.CMDFlashErase:
                                                    _progressViewModel.IsIndeterminate = false;
                                                    _progressViewModel.Message = Tips.Updating;
                                                    lastOperation = CANRadarCommands.CMDFILETRANS;
                                                    if (data[0].Data[0] == 0x07)
                                                    {
                                                        pos += 8;
                                                        byte[] tmp = reader.ReadBytes(ReadByteSize);
                                                        _progressViewModel.Value = pos;
                                                        Array.Copy(tmp, filePrefix, 8);
                                                        //Array.ForEach<byte>(tmp, b => { sum += b; });

                                                        while (pos < reader.Length)
                                                        {
                                                            tmp = reader.ReadBytes(ReadByteSize);
                                                            result = _canger.Send(radar, tmp);
                                                            if (!result)
                                                                break;
                                                            Array.ForEach<byte>(tmp, b => { sum += b; });
                                                            pos += 8;
                                                            _progressViewModel.Value = pos;
                                                        }
                                                    }
                                                    break;
                                                case CANRadarCommands.CMDFILETRANS:
                                                    if(data[0].Data[0] == 0x0C)
                                                    {
                                                        _progressViewModel.Message = Tips.CRCing;
                                                        lastOperation = CANRadarCommands.CMDFILESUMCRC;
                                                        result = _canger.Send(radar, new byte[] { Convert.ToByte((sum & 0XFF000000) >> 24), Convert.ToByte((sum & 0X00FF0000) >> 16), Convert.ToByte((sum & 0X0000FF00) >> 8), Convert.ToByte((sum & 0X000000FF)) });
                                                    }
                                                    break;
                                                case CANRadarCommands.CMDFILESUMCRC:
                                                    if(data[0].Data[0] == 0x0D)
                                                    {
                                                        lastOperation = CANRadarCommands.CMDFILEPREFIXCRC;
                                                        result = _canger.Send(radar, filePrefix);
                                                    }
                                                    break;
                                                case CANRadarCommands.CMDFILEPREFIXCRC:
                                                    if (data[0].Data[0] == 0x09)
                                                    {
                                                        _progressViewModel.Message = Tips.Updated;
                                                        _canger.DataReceivedHandler = null;
                                                    }
                                                    break;
                                            }
                                            if(!result)
                                            {
                                                _canger.Close();
                                                _canger = null;
                                                reader.Close();
                                                _canger.DataReceivedHandler = null;
                                                _progressViewModel.Message = Tips.UpdateFail;
                                                return;
                                            }
                                        }

                                        if (_progressViewModel.Message == Tips.Updated)
                                        {
                                            reader.Close();
                                            _canger.Close();
                                            _canger = null;
                                            if (_progressCtrl.IsVisible)
                                            {
                                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                                                {
                                                    await TaskEx.Delay(1000);
                                                    await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                                                }));
                                            }
                                        }
                                    };
                            if(!_canger.Send(radar, new byte[] { 0x01, 0x00}))
                            {
                                _progressViewModel.Message = ErrorString.CANOpenError;
                            }
                        }
                    }));
                }
            );
        }

        public ICommand RefreshItems { get; set; }

    }

    public enum RadarType { Hold, Trigger }

    public class SerialMainViewModel : MainViewModel
    {
        private const int ReadBytesNumber = 64;
        ConfigViewModel ConfigModel { get; set; }
        PwdViewModel PasswordModel { get; set; }

        SerialManager serial = null;

        private SettingView _settingView = null;
        PasswordView _pwdView = null;
        public string Distance { get; set; }
        public string LRange { get; set; }
        public string RRange { get; set; }

        public string Position { get; set; }
        public string Threshold { get; set; }
        public string Delay { get; set; }
        public bool DelayVisible { get; set; }

        public string Version { get; set; }

        public string BinPath { get; set; }

        public List<string> GateTypes { get { return control.GateType.GetAllTypes(); } }
        public string Gate { get; set; }
        public List<string> ThresholdTypes { get { return control.ThresholdType.GetAllTypes(); } }
        public bool Developer { get; set; }

        public ICommand GetCmd { get; set; }
        public ICommand SetCmd { get; set; }
        public ICommand DefaultCmd { get; set; }
        public ICommand StudyCmd { get; set; }

        public ICommand GetTimeCmd { get; set; }
        public ICommand SetTimeCmd { get; set; }
        public ICommand ClearTimeCmd { get; set; }
        public ICommand SearchCmd { get; set; }
        public ICommand RebootCmd { get; set; }

        public ICommand GetWeakPointsCmd { get; set; }
        public ICommand CancelGetWeakPointsCmd { get; set; }
        public ICommand RemoveWeakPointsCmd { get; set; }
        public ICommand RegetWeakPointsCmd { get; set; }
        public ICommand CancelRemoveWeakPointsCmd { get; set; }

        public DateTime CurrentTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ObservableCollection<string> SearchResult { get; set; }

        public List<string> RecordTypes { get { return control.RecordKind.GetAllTypes(); } }
        public string Record { get; set; }

        public RadarType ConnectedRadarType{get;set;}

        public SerialMainViewModel(MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator):base(dialogCoordinator)
        {
            _progressViewModel = new ProgressViewModel()
            {
                CancelCommand = new SimpleCommand()
                {
                    ExecuteDelegate = param =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                        {
                            if (serial != null)
                            {
                                serial.CompareEndString = true;
                                serial.Rate = (int)ConfigModel.CustomRate;
                                serial.close();
                            }
                            if (overTimer != null)
                                overTimer.Dispose();
                            await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                            mutex.Set();
                        }));
                    }
                }, IsIndeterminate = true
            };
            _progressCtrl.DataViewModel = _progressViewModel;

            ConfigModel = new ConfigViewModel(
                cancel => { Dispose(); Application.Current.Shutdown(); },
                confirm => { connect();  }
                );
            RateEditable = true;
            ItemSourceName = Application.Current.Resources["Serial"].ToString();

            _settingView = new SettingView();
            _settingView.DataContext = ConfigModel;

            Developer = false;
            PasswordModel = new PwdViewModel()
            {
                CancelCommand = new SimpleCommand() {
                    ExecuteDelegate = arg => {  _dialogCoordinator.HideMetroDialogAsync(this, _pwdView); }
                },
                 CheckCommand = new SimpleCommand() {
                     ExecuteDelegate  = arg => {
                         if(!Developer)
                            Developer = PasswordModel.verify();
                         if (!Developer)
                             ShowErrorWindow(Application.Current.Resources["Error"].ToString());
                         else
                         {
                             _dialogCoordinator.HideMetroDialogAsync(this, _pwdView);
                             SerialWork(() => ToGetVer());
                         }
                         OnPropertyChanged("Developer");
                     }
                 }
            };
            _pwdView = new PasswordView();
            _pwdView.DataContext = PasswordModel;

            SetCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(()=> { ToSet();});
                }
            };

            GetCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    if(string.IsNullOrEmpty(Version))
                    {
                        SerialWork(() => { ToGetVer(() => { SerialWork(() => { ToGet(); }); }); });
                    }
                    else
                    {
                        SerialWork(() => { ToGet(); });
                    }
                }
            };

            DefaultCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(()=>ToReset());
                }
            };

            StudyCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(()=>ToStudy(), true, -1);
                }
            };

            RebootCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => ToReboot());
                }
            };

            timekeeper.Tick += Timekeeper_Tick;
            timekeeper.Interval = TimeSpan.FromSeconds(10);

            GetTimeCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => toGetTime());
                }
            };
            SetTimeCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => ToSetTime());
                }
            };
            ClearTimeCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => toClarTime());
                }
            };
            SearchCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => toSearch());
                }
            };
            SearchResult = new ObservableCollection<string>();

            GetWeakPointsCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => toGetWeakPoints(), true, -1);
                }
            };
            CancelGetWeakPointsCmd  = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => toCancelGetWeakPoints());
                }
            };
            RemoveWeakPointsCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => toRemoveWeakPonints());
                }
            };
            RegetWeakPointsCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => toGetRemovedWeakPoints());
                }
            };
            CancelRemoveWeakPointsCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => toCancelRemoveWeakPoints(), true, -1);
                }
            };

            StrongestWeakPoints = new ChartValues<ObservablePoint>();
            RemovedWeakPoints = new ChartValues<ObservablePoint>();

            pointsDataReceiveTimer = new Timer(param => {
                if (showStrongestPoints)
                {
                    showStrongestPoints = false;
                }
                else
                {
                    StrongestWeakPoints.Clear();
                }
            }, null, 0, 1000);

            CurrentTime = DateTime.Now;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
        }

        private void toCancelGetWeakPoints()
        {
            serial.DataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    StrongestWeakPoints.Clear();
                }
                else
                {
                    ShowErrorWindow(Tips.ConfigFail);
                }
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.AlarmOrder4);
        }

        Timer pointsDataReceiveTimer = null;
        bool showStrongestPoints = false;
        private void toGetWeakPoints()
        {
            serial.DataReceivedHandler = msg =>
            {
                if (msg.Contains("X"))
                {
                    var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+.\d+");
                    if (collection.Count > 1)
                    {
                        if(StrongestWeakPoints.Count > 0)
                        {
                            StrongestWeakPoints[0].X = double.Parse(collection[0].Value);
                            StrongestWeakPoints[0].Y = double.Parse(collection[1].Value);
                        }
                        else
                        {
                            StrongestWeakPoints.Add(new ObservablePoint(double.Parse(collection[0].Value), double.Parse(collection[1].Value)));
                        }
                    }
                    showStrongestPoints = true;
                }
                else if(string.IsNullOrEmpty(msg))
                {
                    StrongestWeakPoints.Clear();
                }
                //else
                //{
                //    ShowErrorWindow(Tips.ConfigFail);
                //    mutex.Set();
                //}
            };
            serial.CompareEndString = false;
            serial.WriteLine(SerialRadarCommands.AlarmOrder0);
        }

        private void toCancelRemoveWeakPoints()
        {

            serial.DataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    RemovedWeakPoints.Clear();
                }
                else if (msg.Contains("X"))
                {
                    var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+.\d+");
                    if (collection.Count > 1)
                    {
                        if (StrongestWeakPoints.Count > 0)
                        {
                            StrongestWeakPoints[0].X = double.Parse(collection[0].Value);
                            StrongestWeakPoints[0].Y = double.Parse(collection[1].Value);
                        }
                        else
                        {
                            StrongestWeakPoints.Add(new ObservablePoint(double.Parse(collection[0].Value), double.Parse(collection[1].Value)));
                        }
                    }
                    showStrongestPoints = true;
                }
                else if(string.IsNullOrEmpty(msg))
                {
                    StrongestWeakPoints.Clear();
                }
                //else 
                //{
                //    ShowErrorWindow(Tips.ConfigFail);
                //    mutex.Set();
                //}
            };
            serial.CompareEndString = false;
            serial.WriteLine(SerialRadarCommands.AlarmOrder3);
        }

        private void toRemoveWeakPonints()
        {
            serial.DataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    StrongestWeakPoints.Clear();
                }
                else
                {
                    ShowErrorWindow(Tips.ConfigFail);
                }
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.AlarmOrder1);
        }

        private void toGetRemovedWeakPoints()
        {
            serial.DataReceivedHandler = msg =>
            {
                RemovedWeakPoints.Clear();
                if (msg.Contains(SerialRadarReply.Done))
                {
                    var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+.\d+");
                    for(int i=0; i<collection.Count; i+=2)
                    {
                        RemovedWeakPoints.Add(new ObservablePoint(double.Parse(collection[i].Value), double.Parse(collection[i+1].Value)));
                    }
                }
                else
                {
                    ShowErrorWindow(Tips.ConfigFail);
                }
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.AlarmOrder2);
        }

        public ChartValues<ObservablePoint> StrongestWeakPoints { get; set; }
        public ChartValues<ObservablePoint> RemovedWeakPoints { get; set; }

        private void toSearch()
        {
            string lastOperation = SerialRadarCommands.BootLoader;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                SearchResult.Clear();
            }));
            serial.DataReceivedHandler = msg =>
            {
                if(msg.Contains(SerialRadarReply.Done))
                {
                    switch(lastOperation)
                    {
                        case SerialRadarCommands.BootLoader:
                            lastOperation = SerialRadarCommands.SearchTime;
                           // serial.WriteLine("DalayFilpTime 1970 1 1 0 0 0 1970 1 1 1 1 40");
                            serial.WriteLine(SerialRadarCommands.SearchTime + " " + StartTime.Year +
                                                                                                                           " " + StartTime.Month +
                                                                                                                           " " + StartTime.Day +
                                                                                                                           " " + StartTime.Hour +
                                                                                                                           " " + StartTime.Minute +
                                                                                                                           " " + StartTime.Second +
                                                                                                                           " " + EndTime.Year +
                                                                                                                           " " + EndTime.Month +
                                                                                                                           " " + EndTime.Day +
                                                                                                                           " " + EndTime.Hour +
                                                                                                                           " " + EndTime.Minute +
                                                                                                                           " " + EndTime.Second);
                            break;
                        case SerialRadarCommands.SearchTime:
                            lastOperation = SerialRadarCommands.SensorStart;
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(()=> {
                                lock(SearchResult)
                                {
                                    msg.Split(new char[] { '\r', '\n' }).ToList().ForEach(str =>
                                    {
                                        if (str != string.Empty)
                                        {
                                            str = str.Replace(OperationType.UpValue, OperationType.Up);
                                            str = str.Replace(OperationType.DownValue, OperationType.Down);
                                            SearchResult.Add(str);
                                        }
                                    });
                                }
                            }));
                            serial.CompareEndString = false;
                            serial.WriteLine(SerialRadarCommands.SoftReset);
                            break;
                    }
                }
                else if(lastOperation == SerialRadarCommands.SensorStart)
                {
                    lock (SearchResult)
                    {
                        if (SearchResult.Count == 0)
                        {
                            ShowConfirmWindow(Tips.SearchTimeGetNone, string.Empty);
                        }
                        else
                        {
                            ShowConfirmWindow(Tips.SearchTimeSuccess, string.Empty);
                        }
                    }
                    serial.DataReceivedHandler = null;
                    mutex.Set();
                }
                else
                {
                    ShowErrorWindow(Tips.SearchTimeFail);
                }
            };
            serial.WriteLine(SerialRadarCommands.BootLoader);
        }

        private void toClarTime()
        {
            string lastOperation = SerialRadarCommands.SensorStop;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async ()=> {
                _progressViewModel.Message = Tips.ClearTiming;
                await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
            }));
            serial.DataReceivedHandler = msg =>
            {
                if(msg.Contains(SerialRadarReply.Done))
                {
                    switch(lastOperation)
                    {
                        case SerialRadarCommands.SensorStop:
                            lastOperation = SerialRadarCommands.ClearTime;
                            serial.WriteLine(SerialRadarCommands.ClearTime);
                            break;
                        case SerialRadarCommands.ClearTime:
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () => {
                                _progressViewModel.Message = Tips.ClearTimeSuccess;
                                await TaskEx.Delay(1000);
                                if (_progressCtrl.IsVisible)
                                    await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                            }));
                            serial.WriteLine(SerialRadarCommands.SoftReset);
                            mutex.Set();
                            break; 
                        case SerialRadarCommands.SensorStart:
                            break;
                    }
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () => {
                        _progressViewModel.Message = Tips.ClearTimeFail;
                        _progressViewModel.MaxValue = 1;
                        _progressViewModel.Value = 0;
                        _progressViewModel.IsIndeterminate = false;
                        if(_progressCtrl.IsVisible)
                            await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                    }));
                    mutex.Set();
                }
            };
            serial.WriteLine(SerialRadarCommands.BootLoader);
        }

        private void ToSetTime()
        {
            string lastOperation = SerialRadarCommands.SetTime;
            serial.DataReceivedHandler = msg => 
            {
                if(msg.Contains(SerialRadarReply.Done))
                {
                    ShowSplashWindow(Tips.SetTimeSuccess, 1000);
                }else
                {
                    ShowErrorWindow(Tips.SetTimeFail);
                }
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.SetTime + " " + CurrentTime.Year.ToString() + " " + CurrentTime.Month + " " + CurrentTime.Day + " " + CurrentTime.Hour + " " + CurrentTime.Minute + " " + CurrentTime.Second);
        }

        private void toGetTime()
        {
            string lastOperation = SerialRadarCommands.GetTIme;
            serial.DataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    switch (lastOperation)
                    {
                        case SerialRadarCommands.BootLoader:
                            lastOperation = SerialRadarCommands.GetTIme;
                            serial.WriteLine(SerialRadarCommands.GetTIme);
                            break;
                        case SerialRadarCommands.GetTIme:
                            string[] date = msg.Split(new char[] { '\r', '\n', ' ' });
                            try
                            {
                                if (date[3] == "")
                                    CurrentTime = DateTime.Parse(date[1] + " " + date[2] + " " + date[4] + " " + date[6] + " " + date[5]);
                                else
                                    CurrentTime = DateTime.Parse(date[1] + " " + date[2] + " " + date[3] + " " + date[5] + " " + date[4]);
                                OnPropertyChanged("CurrentTime");
                            }
                            catch
                            {
                                ShowErrorWindow(Tips.GetTimeFail);
                            }
                            mutex.Set();
                            break;
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    ShowErrorWindow(Tips.GetTimeFail);
                }
            };
            serial.WriteLine(SerialRadarCommands.GetTIme);
        }

        private void connect()
        {
            if (ConfigModel.CustomItem == "" || ConfigModel.CustomItem == null)
            {
                ShowErrorWindow(ErrorString.SerialError);
                return;
            }
            else if (ConfigModel.CustomRate == 0)
            {
                ShowErrorWindow(ErrorString.ParamError);
                return;
            }
            if (serial == null)
            {
                serial = new SerialManager(GetSerialPortName(ConfigModel.CustomItem));
            }
            serial.PortName = GetSerialPortName(ConfigModel.CustomItem);
            serial.Rate = (int)ConfigModel.CustomRate;
            if (serial.IsOpen)
                serial.close();
            if (!serial.open())
            {
                ShowErrorWindow(ErrorString.SerialOpenError);
                return;
            }
            if (_settingView.IsVisible)
            {
                _dialogCoordinator.HideMetroDialogAsync(this, _settingView);
            }
            
            SerialWork(() => ToGetVer(()=> { SerialWork(() => JudgeRadarType()); }), false);

            if (ConfigModel.SelectedRadarType == Application.Current.Resources["RadarTriggerType"].ToString())
            {
                ConnectedRadarType = RadarType.Trigger;
                OnPropertyChanged("IsHoldRadarType");
            }
        }

        ManualResetEvent mutex = new ManualResetEvent(false);
        private async void SerialWork(Action towork, bool toShowOverTimeTip = true, int waitmillionseoconds = 3000)
        {
            if (serial != null)
            {
                serial.close();
            }
            serial = new SerialManager(GetSerialPortName(ConfigModel.CustomItem));
            serial.Rate = (int)ConfigModel.CustomRate;
            serial.Type = SerialReceiveType.Chars;
            if (!serial.open())
            {
                ShowErrorWindow(ErrorString.SerialOpenError);
                return;
            }
            await Task.Factory.StartNew(
                ()=> { 
                    towork();
                    if (!mutex.WaitOne(waitmillionseoconds))
                    {
                        if(toShowOverTimeTip)
                            ShowErrorWindow(ErrorString.OverTime);
                    }
                    mutex.Reset();
                });
        }
        private void ToGetDelay()
        {
            string lastOperation = SerialRadarCommands.SensorStop;
            serial.DataReceivedHandler =async msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        lastOperation = SerialRadarCommands.ReadCLI;
                        await TaskEx.Delay(300);
                        serial.WriteLine(SerialRadarCommands.ReadCLI + " " + SerialArguments.DelayTimeParam);
                    }
                    else if (lastOperation == SerialRadarCommands.ReadCLI)
                    {
                        Delay = System.Text.RegularExpressions.Regex.Match(msg, @"\d+").Value;
                        OnPropertyChanged("Delay");
                        await TaskEx.Delay(300);
                        lastOperation = string.Empty;
                        serial.WriteLine(SerialRadarCommands.SensorStart);
                        ShowConfirmWindow(Tips.GetSuccess, string.Empty);
                        mutex.Set();
                        serial.close();
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    ShowErrorWindow(Tips.GetFail);
                    mutex.Set();
                    serial.close();
                }
                else if(msg.Contains(SerialRadarReply.Start))
                {
                }
            };
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }
        private void ToSetDelay(bool reset=false)
        {
            int delay = 6;
            try
            {
                if (!reset)
                    delay = int.Parse(Delay);
                if (delay < 1.99999 || delay > 300.00001)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                ShowErrorWindow(ErrorString.DelayError);
                mutex.Set();
                return;
            }
           
           
            string lastOperation = SerialRadarCommands.SensorStop;
            serial.DataReceivedHandler = async msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        lastOperation = SerialRadarCommands.WriteCLI;
                        await TaskEx.Delay(300);
                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.DelayTimeParam + " " + delay.ToString());
                    }
                    else if (lastOperation == SerialRadarCommands.WriteCLI)
                    {
                        if (reset)
                        {
                            Delay = delay.ToString();
                            OnPropertyChanged("Delay");
                        }
                        ShowConfirmWindow(Tips.ConfigSuccess, string.Empty);
                        await TaskEx.Delay(300);
                        serial.EndStr = SerialRadarReply.Start;
                        serial.WriteLine(SerialRadarCommands.SoftReset);
                        mutex.Set(); 
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    ShowErrorWindow(Tips.ConfigFail);
                    mutex.Set();
                }
                else if (msg.Contains(SerialRadarReply.Start))
                {
                }
            };
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }

        DispatcherTimer timekeeper = new DispatcherTimer();
        private void ToStudy()
        {
            serial.EndStr = "\n";
            serial.DataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.StudyEnd))
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                    {
                        await TaskEx.Delay(500);
                        if (_progressCtrl.IsVisible)
                        {
                            await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                        }
                        ShowConfirmWindow(Tips.StudySuccess, string.Empty);
                    }));
                    mutex.Set();
                }
                else if (msg.Contains(SerialRadarReply.Done))
                {
                    _progressViewModel.Message = Tips.Studying;
                    _progressViewModel.IsIndeterminate = true;
                    _progressViewModel.MaxValue = 100;
                    _progressViewModel.Value = 0;
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                    {
                        await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
                    }));
                }
                else
                {
                    _progressViewModel.Message = Tips.Studying + ":    " + msg.Trim('\n', '\r');
                }
            };
            serial.WriteLine(SerialRadarCommands.Output + " 4");
        }
        private void ToReboot()
        {
            serial.WriteLine(SerialRadarCommands.SoftReset);
            ShowConfirmWindow(Tips.RebootSuccess, Tips.ConfigSuccess);
            serial.DataReceivedHandler = null;
            mutex.Set();
        }
        private void Timekeeper_Tick(object sender, EventArgs e)
        {
            if(_progressCtrl.IsVisible)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                {
                    _progressViewModel.Message = Tips.StudyEnd;
                    await TaskEx.Delay(1000);
                    await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                }));
                mutex.Set();
                timekeeper.Stop();
            }
        }

        private void ToReset()
        {
            string lastOperation = SerialRadarCommands.SensorStop;
            serial.DataReceivedHandler =async msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        lastOperation = SerialRadarCommands.WriteCLI;
                        await TaskEx.Delay(300);
                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.FilterParam + " 0 0 5 2 2 30 5 32 0 0 0");
                    }
                    else if (lastOperation == SerialRadarCommands.WriteCLI)
                    {
                        LRange = "0.5";
                        Distance = "3";
                        RRange = "0.5";
                        Gate = control.GateType.Straight;
                        Threshold = control.ThresholdType.High;
                        Record = control.RecordKind.Ignore;
                        OnPropertyChanged("LRange");
                        OnPropertyChanged("Distance");
                        OnPropertyChanged("RRange");
                        OnPropertyChanged("Gate");
                        OnPropertyChanged("Threshold");
                        OnPropertyChanged("Record");
                        mutex.Set();
                        if (DelayVisible)
                        {
                            SerialWork(() => ToSetDelay(true));
                        }
                        else
                        {
                            serial.EndStr = SerialRadarReply.Start;
                            serial.WriteLine(SerialRadarCommands.SoftReset);
                        }
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    ShowErrorWindow(Tips.ConfigFail);
                    mutex.Set();
                }
                else if(msg.Contains(SerialRadarReply.Start))
                {
                    ShowConfirmWindow(Tips.ConfigSuccess, string.Empty);
                }
            };
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }

        private void ToGet()
        {
            string lastOperation = SerialRadarCommands.ReadCLI;
            serial.DataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        lastOperation = SerialRadarCommands.ReadCLI;
                        serial.WriteLine(SerialRadarCommands.ReadCLI + " " + SerialArguments.FilterParam);
                    }
                    else if (lastOperation == SerialRadarCommands.ReadCLI)
                    {
                        string[] result = msg.Split(new char[] { ' ', '\n', '\r' });
                        if(result.Length > 11)
                        {
                            LRange = (float.Parse(result[3]) / 10).ToString();
                            Distance = (float.Parse(result[6]) / 10).ToString();
                            RRange = (float.Parse(result[7]) / 10).ToString();
                            Gate = control.GateType.GetType(result[9]);
                            Threshold = control.ThresholdType.GetType(result[10]);
                            Record = control.RecordKind.GetType(result[11]);
                            OnPropertyChanged("LRange");
                            OnPropertyChanged("Distance");
                            OnPropertyChanged("RRange");
                            OnPropertyChanged("Gate");
                            OnPropertyChanged("Threshold");
                            OnPropertyChanged("Record");
                        }else
                        {
                            ShowErrorWindow(Tips.GetFail);
                        }
                        mutex.Set();
                        Thread.Sleep(500);
                        if (DelayVisible)
                        {
                            SerialWork(() => ToGetDelay());
                        }
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    ShowErrorWindow(Tips.GetFail);
                    mutex.Set();
                }
            };
            serial.WriteLine(SerialRadarCommands.ReadCLI + " " + SerialArguments.FilterParam);
        }

        private void ToSet()
        {
            float distance = 0.0f, lrange = 0.0f, rrange = 0.0f;
            try
            {
                distance = float.Parse(Distance);
                lrange = float.Parse(LRange);
                rrange = float.Parse(RRange);
                string error = string.Empty;
                if (distance < 0 || lrange < 0 || rrange < 0)
                {
                    error = ErrorString.ParamError;
                    throw new Exception(error);
                }
                else if (distance > 6.0 || distance < 1.0)
                {
                    error = ErrorString.DisntacneError;
                    throw new Exception(error);
                }
                else if(Gate == control.GateType.Straight)
                {
                    if (lrange < 0.09999 || rrange < 0.09999 || lrange > 1.00001 || rrange > 1.00001)
                    {
                        error = ErrorString.RangeError1;
                        throw new Exception(error);
                    }
                }
                else if (Gate == control.GateType.AdvertisingFence)
                {
                    if (lrange < 0.09999 || rrange < 0.09999 || lrange > 1.00001 || rrange > 1.00001)
                    {
                        error = ErrorString.RangeError2;
                        throw new Exception(error);
                    }
                }
            }
            catch(Exception e)
            {
                ShowErrorWindow(e.Message);
                mutex.Set();
                return;
            }
            
            string lastOperation = SerialRadarCommands.SensorStop;
            serial.DataReceivedHandler = async msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        lastOperation = SerialRadarCommands.WriteCLI;
                        await TaskEx.Delay(300);
                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.FilterParam + " 0 0 " + (float.Parse(LRange) * 10).ToString("F0") + " 2 2 " + (float.Parse(Distance) * 10).ToString("F0") + " " + (float.Parse(RRange) * 10).ToString("F0") + " 32 " + control.GateType.GetValue(Gate) + " " + control.ThresholdType.GetValue(Threshold) + " " + control.RecordKind.GetValue(Record));
                    }
                    else if (lastOperation == SerialRadarCommands.WriteCLI)
                    {
                        OnPropertyChanged("Record");
                        mutex.Set();
                        if (DelayVisible)
                        {
                            SerialWork(() => ToSetDelay());
                        }
                        else
                        {
                            serial.EndStr = SerialRadarReply.Start;
                            serial.WriteLine(SerialRadarCommands.SoftReset);
                        }
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    ShowErrorWindow(Tips.ConfigFail);
                    mutex.Set();
                }
                else if(msg.Contains(SerialRadarReply.Start))
                {
                    ShowConfirmWindow(Tips.ConfigSuccess, string.Empty);
                }
            };
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }

        private void ToGetVer(Action proceedToDo= null)
        {
            serial.DataReceivedHandler = msg =>
            {
                Version = msg.Substring(0, msg.IndexOf("Done")).Trim('\r', '\n');
                if(compareVersion2("1.1.1"))
                {
                    DelayVisible = true;
                }else
                {
                    DelayVisible = false;
                }
                OnPropertyChanged("Version");
                OnPropertyChanged("DelayVisible");
                mutex.Set();
                if (proceedToDo != null)
                    proceedToDo();
            };
            serial.WriteLine(SerialRadarCommands.Version);
        }

        private async void ToResetBaudRate()
        {
            serial.Rate = int.Parse(BauRate.Rate115200);
            string lastOperation = SerialRadarCommands.SensorStop;
            serial.DataReceivedHandler = async msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        await TaskEx.Delay(100);
                        lastOperation = SerialRadarCommands.WriteBaudRate;
                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialRadarCommands.WriteBaudRate + " " + ConfigModel.CustomRate);
                    }
                    else if (lastOperation == SerialRadarCommands.WriteBaudRate)
                    {
                        lastOperation = SerialRadarCommands.SoftReset;
                        serial.CompareEndString = false;
                        await TaskEx.Delay(500);
                        serial.WriteLine(SerialRadarCommands.SoftReset);
                    }
                }
                else
                {
                    if (lastOperation == SerialRadarCommands.SoftReset)
                    {
                        lastOperation = string.Empty;
                        mutex.Set();
                        await TaskEx.Delay(2000);
                        _progressViewModel.Message = Tips.GetVersion;
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
                        {
                            if (_progressCtrl.IsVisible)
                            {
                                await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                            }
                            ShowConfirmWindow(Tips.Updated, string.Empty);
                        }));
                        SerialWork(() => ToGetVer());
                    }
                }
            };
            await TaskEx.Delay(200);
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }

        internal async void start()
        {
            await _dialogCoordinator.ShowMetroDialogAsync(this, _settingView);
        }

        internal async void root()
        {
            if(!Developer)
                await _dialogCoordinator.ShowMetroDialogAsync(this, _pwdView);
        }
        private string GetSerialPortName(string name)
        {
            int pos = name.IndexOf("  ");
            if(pos > 0)
                return name.Substring(0, name.IndexOf("  "));
            return name;
        }

        FileIOManager reader = null;
        Timer overTimer = null;
        byte[] dataTmp = null;
        const int preByteSizeAdded = 32;
        const int ignorePreByteSize = 8;
        protected override void ToDo()
        {
            SerialWork(() =>
            {
                string lastMessage = string.Empty;
                overTimer = new Timer(obj =>
                {
                    if(_progressViewModel.Message == Tips.Updating)
                    {
                        if(lastMessage == _progressViewModel.Value.ToString())
                        {
                            _progressViewModel.Message = ErrorString.OverTime;
                        }else
                        {
                            lastMessage = _progressViewModel.Value.ToString();
                        }
                    }else
                    {
                        if (lastMessage == _progressViewModel.Message)
                        {
                            _progressViewModel.Message = ErrorString.OverTime;
                        }
                        else
                        {
                            lastMessage = _progressViewModel.Message;
                        }
                    }
                }, null, 0, 5000);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                {
                    //serial.CompareEndString = false;
                    _progressViewModel.Message = Tips.Initializing;
                    _progressViewModel.IsIndeterminate = true;
                    _progressViewModel.MaxValue = 100;
                    _progressViewModel.Value = 0;
                    await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);

                    if (_progressCtrl.IsVisible)
                    {
                        string lastOperation = SerialRadarCommands.SensorStop;
                        reader = new FileIOManager(BinPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                        if (reader.Length == -1)
                        {
                            _progressViewModel.Message = ErrorString.FileError;
                            return;
                        }
                        uint sum = 0, pos = preByteSizeAdded+ignorePreByteSize-1;
                        if (reader.Length < ignorePreByteSize)
                        {
                            _progressViewModel.Message = ErrorString.FileError;
                            return;
                        }
                        if (!compareVersion(System.Text.Encoding.Default.GetString(reader.ReadBytes(preByteSizeAdded))))
                        {
                            _progressViewModel.Message = ErrorString.SmallVersion;
                            reader.Close();
                            return;
                        }
                        reader.ReadBytes(ignorePreByteSize);

                        serial.DataReceivedHandler = async msg =>
                        {
                            if (msg.Contains(SerialRadarReply.Done))
                            {
                                switch (lastOperation)
                                {
                                    case SerialRadarCommands.SensorStop:
                                        await TaskEx.Delay(1000);
                                        lastOperation = SerialRadarCommands.WriteCLI;
                                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 1");
                                        break;
                                    case SerialRadarCommands.WriteCLI:
                                        await TaskEx.Delay(500);
                                        lastOperation = ExtraSerialRadarCommands.SoftInercludeReset;
                                        _progressViewModel.Message = Tips.WaitForOpen;
                                        serial.CompareEndString = false;
                                        serial.Type = SerialReceiveType.Bytes;
                                        serial.WriteLine(SerialRadarCommands.SoftReset);
                                        break;
                                }
                            }
                            else
                            {
                                if (msg.Contains(SerialRadarReply.Error))
                                {
                                    _progressViewModel.Message = ErrorString.Error + msg.Trim();
                                    if (reader != null)
                                        reader.Close();
                                    if (serial != null)
                                    {
                                        serial.CompareEndString = true;
                                        serial.Rate = (int)ConfigModel.CustomRate;
                                        mutex.Set();
                                    }
                                    overTimer.Dispose();
                                }
                                else if (msg.Contains(SerialRadarReply.NotRecognized))
                                {
                                    switch (lastOperation)
                                    {
                                        case SerialRadarCommands.SensorStop:
                                            serial.WriteLine(SerialRadarCommands.SensorStop);
                                            break;
                                        case SerialRadarCommands.WriteCLI:
                                            serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 1");
                                            break;
                                        case ExtraSerialRadarCommands.SoftInercludeReset:
                                            serial.WriteLine(SerialRadarCommands.SoftReset);
                                            break;
                                    }
                                }
                                else
                                {
                                    if (lastOperation == ExtraSerialRadarCommands.SoftInercludeReset)
                                    {
                                        lastOperation = SerialRadarCommands.FlashErase;
                                        serial.Rate = int.Parse(BauRate.Rate115200);
                                        serial.Write(new byte[] { 0x01, 0xCD });
                                    }
                                }
                            }
                        };
                        bool toContinue = false;
                        serial.CompareEndBytesCount = 2;
                        serial.BytesDataReceivedHandler = async data =>
                        {
                            if (data.Length > 1)
                            {
                                if (data[0] == 0xEE && data[1] == 0xCD)
                                {
                                    switch(lastOperation)
                                    {
                                        case ExtraSerialRadarCommands.SoftInercludeReset:
                                            serial.WriteLine(SerialRadarCommands.SoftReset);
                                            return;
                                        case SerialRadarCommands.FlashErase:
                                            serial.Write(new byte[] { 0x01, 0xCD });
                                            return;
                                        case SerialRadarCommands.BootLoader:
                                        case SerialRadarCommands.T:
                                            serial.Write(dataTmp);
                                            return;
                                        case SerialRadarCommands.CRC:
                                            byte[] tmp1 = BitConverter.GetBytes(sum);
                                            serial.Write(new byte[] { 0x04, 0xCD, tmp1[0], tmp1[1], tmp1[2], tmp1[3] });
                                            return;
                                        case SerialRadarCommands.SoftReset:
                                            serial.Write(new byte[] { 0x05, 0xCD });
                                            return;
                                    }
                                }
                                else
                                {
                                    switch (lastOperation)
                                    {
                                        case ExtraSerialRadarCommands.SoftInercludeReset:
                                            await TaskEx.Delay(500);
                                            lastOperation = SerialRadarCommands.FlashErase;
                                            _progressViewModel.Message = Tips.Flashing;
                                            serial.Rate = int.Parse(BauRate.Rate115200);
                                            serial.Write(new byte[] { 0x01, 0xCD });
                                            break;
                                        case SerialRadarCommands.FlashErase:
                                            if(data[0] == 0x11 && data[1] == 0xCD)
                                            {
                                                await TaskEx.Delay(500);
                                                lastOperation = SerialRadarCommands.BootLoader;
                                                _progressViewModel.Message = Tips.Updating;
                                                int times = (int)((reader.Length - preByteSizeAdded - ignorePreByteSize) / 64 + ((reader.Length - preByteSizeAdded - ignorePreByteSize) % 64 == 0 ? 0 : 1));
                                                byte[] tmp = BitConverter.GetBytes(times);
                                                _progressViewModel.MaxValue = (uint)times;
                                                _progressViewModel.Value = 0;
                                                serial.Write(new byte[] { 0x02, 0xCD, tmp[0], tmp[1], tmp[2], tmp[3] });
                                            }
                                            break;
                                        case SerialRadarCommands.BootLoader:
                                        case SerialRadarCommands.T:
                                            await TaskEx.Delay(1);
                                            if (lastOperation == SerialRadarCommands.BootLoader)
                                            {
                                                if (data[0] == 0x12 && data[1] == 0xCD)
                                                {
                                                    toContinue = true;
                                                    _progressViewModel.Message = Tips.Updating;
                                                    _progressViewModel.IsIndeterminate = false;
                                                }
                                            }
                                            if (toContinue)
                                            {
                                                lastOperation = SerialRadarCommands.T;
                                                if ((pos >= reader.Length - 1))
                                                {
                                                    if(data[data.Length-2] == 0x23 && data[data.Length-1] == 0xCD)
                                                    {
                                                        await TaskEx.Delay(100);
                                                        lastOperation = SerialRadarCommands.CRC;
                                                        _progressViewModel.Message = Tips.CRCing;
                                                        byte[] tmp1 = BitConverter.GetBytes(sum);
                                                        serial.Write(new byte[] { 0x04, 0xCD, tmp1[0], tmp1[1], tmp1[2], tmp1[3] });
                                                    }
                                                }
                                                else
                                                {
                                                    dataTmp = reader.ReadBytes(ReadBytesNumber);
                                                    Array.ForEach<byte>(dataTmp, b => { sum += b; });
                                                    serial.Write(dataTmp);
                                                }
                                                pos += ReadBytesNumber;
                                                _progressViewModel.Value += 1;
                                            }
                                            break;
                                        case SerialRadarCommands.CRC:
                                            await TaskEx.Delay(500);
                                            lastOperation = SerialRadarCommands.SoftReset;
                                            _progressViewModel.Value = _progressViewModel.MaxValue;
                                            _progressViewModel.Message = Tips.WaitForOpen;
                                            serial.Write(new byte[] { 0x05, 0xCD });
                                            reader.Close();
                                            break;
                                        case SerialRadarCommands.SoftReset:
                                            _progressViewModel.Message = Tips.Updated;
                                            if (serial != null)
                                            {
                                                serial.CompareEndString = true;
                                                serial.Rate = (int)ConfigModel.CustomRate;
                                            }
                                            mutex.Set();
                                            overTimer.Dispose();
                                            SerialWork(() => ToResetBaudRate());
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                _progressViewModel.Message = Tips.UpdateFail;
                                if (reader != null)
                                    reader.Close();
                                if (serial != null)
                                {
                                    serial.CompareEndString = true;
                                    serial.Rate = (int)ConfigModel.CustomRate;
                                    mutex.Set();
                                }
                                overTimer.Dispose();
                            }

                        };
                        serial.WriteLine(SerialRadarCommands.SensorStop);
                    }
                }));
            }, true, -1);
        }

        private bool compareVersion(string ver)
        {
            try
            {
                Version binVersion = new Version(System.Text.RegularExpressions.Regex.Match(ver, @"\d+\.\d+\.\d+").Value);
                try
                {
                    Version radarVersion = new Version(System.Text.RegularExpressions.Regex.Match(Version, @"\d+\.\d+\.\d+").Value);
                    return (binVersion >= radarVersion);
                }catch
                {
                    return true;
                }
            }catch
            {
                return false;
            }
        }

        private bool compareVersion2(string ver)
        {
            try
            {
                Version delayVersion = new Version(System.Text.RegularExpressions.Regex.Match(ver, @"\d+\.\d+\.\d+").Value);
                try
                {
                    Version radarVersion = new Version(System.Text.RegularExpressions.Regex.Match(Version, @"\d+\.\d+\.\d+").Value);
                    return (delayVersion <= radarVersion);
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return false; 
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (serial != null)
            {
                serial.WriteLine(SerialRadarCommands.AlarmOrder4);
                serial.close();
            }
        }

        public bool IsHoldRadarType { get => ConnectedRadarType == RadarType.Hold; }
        public bool IsTriggerRadarType { get => ConnectedRadarType == RadarType.Trigger; }
        private void JudgeRadarType()
        {
            if (ConfigModel.SelectedRadarType == Application.Current.Resources["RadarHoldType"].ToString())
            {
                ConnectedRadarType = RadarType.Hold;
                OnPropertyChanged("IsHoldRadarType");
                mutex.Set();
            }
            else
            {
                int delay = 0;
                string lastOperation = SerialRadarCommands.SensorStop;
                serial.DataReceivedHandler = async msg =>
                {
                    if (msg.Contains(SerialRadarReply.Done))
                    {
                        if (lastOperation == SerialRadarCommands.SensorStop)
                        {
                            lastOperation = SerialRadarCommands.WriteCLI;
                            await TaskEx.Delay(200);
                            serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.DelayTimeParam + " " + delay.ToString());
                        }
                        else if (lastOperation == SerialRadarCommands.WriteCLI)
                        {
                            ConnectedRadarType = RadarType.Trigger;
                            OnPropertyChanged("IsHoldRadarType");
                            DelayVisible = false;
                            await TaskEx.Delay(200);
                            serial.EndStr = SerialRadarReply.Start;
                            serial.WriteLine(SerialRadarCommands.SoftReset);
                            mutex.Set();
                        }
                    }
                    else if (msg.Contains(SerialRadarReply.Error))
                    {
                        ShowErrorWindow(Tips.ConfigFail);
                        mutex.Set();
                    }
                    else if (msg.Contains(SerialRadarReply.Start))
                    {
                    }
                };
                serial.WriteLine(SerialRadarCommands.SensorStop);
            }
        }
    }

    public class WifiMainViewModel : MainViewModel
    {
        public UpdateModel ConfigModel { get; set; }
        public WifiMainViewModel(MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator):base(dialogCoordinator)
        {
            ConfigModel = new UpdateModel();
            RateEditable = true;
            ItemSourceName = Application.Current.Resources["IP"].ToString();
        }

        protected override void ToDo()
        {
            ShowWarnWindow(Application.Current.Resources["NoImplementation"].ToString());
        }
    }
}
