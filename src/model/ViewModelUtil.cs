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
using HexProtocol = MbitGate.helper.CommHexProtocolDecoder;
using System.CodeDom;
using System.Windows.Media;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System.Windows.Shapes;

namespace MbitGate.model
{
    public class ConnectViewModel
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

        public ConnectViewModel(Action<object> _cancel, Action<object> _confirm)
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
            get => new List<string>() { Application.Current.Resources["RadarHoldType"].ToString(), Application.Current.Resources["RadarTriggerType"].ToString() };
        }

        public string SelectedRadarType
        {
            get; set;
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
        public bool verifyDeveloper()
        {
            if (Password != null && Password == "8888")
            {
                return true;
            }
            return false;
        }
        public bool verifyRoot()
        {
            if (Password != null && Password == "Mbit")
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

    public class RecordViewModel: ViewModelBase
    {
        public ICommand GetTimeCommand { get; set; }
        public ICommand SetTimeCommand { get; set; }
        public ICommand SynTimeCommand { get; set; }
        public ICommand ClearRecordCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public string InvertSearchCount { get; set; }
        public ICommand InvertSearchCommand { get; set; }
        public DateTime CurrentTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ObservableCollection<string> SearchResult { get; set; }
        
        readonly ICommand _cancelCommand;

        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand;
            }
        }
        public RecordViewModel(Action<object> cancel)
        {
            _cancelCommand = new SimpleCommand()
            {
                ExecuteDelegate = cancel
            };
        }

    }

    public class RootDevelopViewModel:ViewModelBase
    {
        public ICommand WriteCLICommand { get; set; }
        public string WriteCLICommandStr { get; set; }
        public ICommand CustomCommand { get; set; }
        public string CustomCommandStr { get; set; }
        public ICommand ReadCLIAllCommand { get; set; }
        public ICommand SensorstopCommand { get; set; }
        
        readonly ICommand _cancelCommand;

        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand;
            }
        }
        public RootDevelopViewModel(Action<object> cancel)
        {
            _cancelCommand = new SimpleCommand()
            {
                ExecuteDelegate = cancel
            };
        }
        public string Threshold { get; set; }
        public string Delay { get; set; }

    }

    public class UserDevelopViewModel
    {
        public string WriteCLIRangeCommandMaxStr { get; set; }
        public string WriteCLIRangeCommandMinStr { get; set; }
        public ICommand WriteCLIRangeCommand { get; set; }
        public ICommand WorkAnomalousCommand { get; set; }
        public ICommand WriteCLIRainCommand { get; set; }
        
        readonly ICommand _cancelCommand;

        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand;
            }
        }
        public UserDevelopViewModel(Action<object> cancel)
        {
            _cancelCommand = new SimpleCommand()
            {
                ExecuteDelegate = cancel
            };
        }
    }

    public class UpdateViewModel
    {
        readonly ICommand _cancelCommand;
        public List<string> UpdateRates { get => BauRate.GetUpdateRates(); }
        public string CustomUpdateRate { get; set; }
        public string BinPath { get; set; }
        public ICommand UpdateCommand
        {
            get; set;
        }
        public ICommand CancelCommand { get => _cancelCommand; }
        public UpdateViewModel(Action<object> cancel)
        {
            _cancelCommand = new SimpleCommand()
            {
                ExecuteDelegate = cancel
            };
        }
    }

    public class ComparisonViewModel: ViewModelBase
    {
        public ICommand GetBackPointsCommand { get; set; }

        public ICommand ComparisonCommand { get; set; }

        public LiveCharts.SeriesCollection BackgroundSeries { get; set; }
        public bool CanCompare { get; set; }

        public bool BackgroundComparison { get; set; }
        public ChartValues<ObservablePoint> BackgroundBeforePoints { get; set; }
        public ChartValues<ObservablePoint> BackgroundAfterPoints { get; set; }

        readonly ICommand _cancelCommand;
        public ICommand CancelCommand { get => _cancelCommand; }
        public ComparisonViewModel(Action<object> cancel)
        {
            _cancelCommand = new SimpleCommand()
            {
                ExecuteDelegate = cancel
            };
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
        protected ProgressView _progressCtrl = null;

        public string ItemSourceName { get; set; }
        public ObservableCollection<string> Items { get; set; }

        public bool RateEditable { get; set; }

        public MainViewModel(MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
            _progressCtrl = new ProgressView();
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
                if (!_progressCtrl.IsVisible)
                    await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
                await TaskEx.Delay(millionSecond);

                CreateBackgroundWorker(
                    (sender, args) =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                        {
                            if (_progressCtrl.IsVisible)
                                if (work != null)
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
                if (_progressCtrl.IsVisible)
                    await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                MessageView _dialog = new MessageView();
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
                if (_progressCtrl.IsVisible)
                    await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                MessageView _dialog = new MessageView();
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
            Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                try
                {
                    if (_progressCtrl.IsVisible)
                        await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                    MessageView _dialog = new MessageView();
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

        protected void ShowConfirmCancelWindow(string title, string message, Action confirm = null, Action cancel = null)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                if (_progressCtrl.IsVisible)
                    await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                ConfirmView _dialog = new ConfirmView();
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
                            confirm();
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
                            cancel();
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
                if (value != null)
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
            if (_canger.IsStart)
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
                                    Version = "V  " + BitConverter.ToString(new byte[] { data[0].Data[1], data[0].Data[2], data[0].Data[3] }).Replace("-", " : ");
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

        public CANMainViewModel(MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator) : base(dialogCoordinator)
        {
            _progressViewModel = new ProgressViewModel()
            {
                CancelCommand = new SimpleCommand()
                {
                    ExecuteDelegate = param =>
                    {
                        _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                        if (_canger != null)
                        {
                            _canger.DataReceivedHandler = null;
                            _canger.Close();
                            _canger = null;
                        }
                    }
                }
            };
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
            else if (_canger == null)
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
                                                        if (obj.ID == sendID + 1)
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
                                                    foreach (VCI_CAN_OBJ obj in data)
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
                                                    if (data[0].Data[0] == 0x0C)
                                                    {
                                                        _progressViewModel.Message = Tips.CRCing;
                                                        lastOperation = CANRadarCommands.CMDFILESUMCRC;
                                                        result = _canger.Send(radar, new byte[] { Convert.ToByte((sum & 0XFF000000) >> 24), Convert.ToByte((sum & 0X00FF0000) >> 16), Convert.ToByte((sum & 0X0000FF00) >> 8), Convert.ToByte((sum & 0X000000FF)) });
                                                    }
                                                    break;
                                                case CANRadarCommands.CMDFILESUMCRC:
                                                    if (data[0].Data[0] == 0x0D)
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
                                            if (!result)
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
                            if (!_canger.Send(radar, new byte[] { 0x01, 0x00 }))
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
    public enum ConnectType { UnRegist, Ready, Connecting, Connected, Disconnecting, Disconnected };

    public class TargetPoint : ObservablePoint
    {
        public TargetPoint(double x, double y, string message = "") : base(x, y)
        {
            msg = message;
        }
        string msg = string.Empty;

        public string Message
        {
            get => msg == string.Empty ? String.Format("X:{0:F2},Y:{1:F2}", X, Y) : msg;
            set
            {
                msg = value;
                OnPropertyChanged("Message");
            }
        }
    }
    public class SerialMainViewModel : MainViewModel
    {
        private const int ReadBytesNumber = 64;

        SerialManager serial = null;

        private ConnectView _connectView = null;
        ConnectViewModel ConnectModel { get; set; }

        PasswordView _pwdView = null;
        PwdViewModel PasswordModel { get; set; }

        ComparisonView _compareView = null;
        ComparisonViewModel CompareModel { get; set; }
        RecordsView _recordView = null;
        RecordViewModel RecordModel { get; set; }
        RootDevelopeView _rootView = null;
        RootDevelopViewModel RootModel { get; set; }
        UserDevelopeView _developView = null;
        UserDevelopViewModel DevelopModel { get; set; }
        UpdateView _updateView = null;
        UpdateViewModel UpdateModel { get; set; }

        ConnectType _connectstate = ConnectType.Ready;
        public ConnectType ConnectState
        {
            get => _connectstate;
            set
            {
                _connectstate = value;
                switch (value)
                {
                    case ConnectType.Ready:
                    case ConnectType.Connecting:
                    case ConnectType.Disconnected:
                        ConnectModelName = Tips.Connect;
                        ConnectModelIcon = @"/DAHUA_ITS_A08;component/image/connect.png";
                        break;
                    case ConnectType.Disconnecting:
                    case ConnectType.Connected:
                        ConnectModelName = Tips.Disconnect;
                        ConnectModelIcon = @"/DAHUA_ITS_A08;component/image/disconnect.png";
                        break;
                }
                OnPropertyChanged("ConnectModelName");
                OnPropertyChanged("ConnectModelIcon");
                OnPropertyChanged("ConnectState");
            }
        }
        public string ConnectModelName { get; set; }
        public string ConnectModelIcon { get; set; }
        public string MaxDistance { get; set; }
        public string MinDistance { get; set; }
        public string LRange { get; set; }
        public string RRange { get; set; }

        public string Position { get; set; }
        public string Threshold { get; set; }
        public bool DelayVisible { get; set; }

        public string Version { get; set; }

        public List<string> GateTypes { get; set; }
        string gate = string.Empty;
        public string Gate
        {
            get => gate;
            set
            {
                if (value == control.GateType.Straight)
                {
                    IsLeftRangeEditable = true;
                    IsRightRangeEditable = true;
                    GateTypePosition = imagepath;
                }
                else
                {
                    if (value == control.GateType.AdvertisingLeft)
                    {
                        IsLeftRangeEditable = false;
                        IsRightRangeEditable = true;
                        LRange = "1.0";
                        GateTypePosition = imagepath + "radar_advertising_left.png";
                    }
                    else if (value == control.GateType.AdvertisingRight)
                    {
                        IsRightRangeEditable = false;
                        IsLeftRangeEditable = true;
                        RRange = "1.0";
                        GateTypePosition = imagepath + "radar_advertising_right.png";
                    }
                    else if (value == control.GateType.FenceLeft)
                    {
                        IsLeftRangeEditable = false;
                        IsRightRangeEditable = true;
                        LRange = "1.0";
                        GateTypePosition = imagepath + "radar_fence_left.png";
                    }
                    else if (value == control.GateType.FenceRight)
                    {
                        IsRightRangeEditable = false;
                        IsLeftRangeEditable = true;
                        RRange = "1.0";
                        GateTypePosition = imagepath + "radar_fence_right.png";
                    }
                }
                gate = value;
                OnPropertyChanged("IsLeftRangeEditable");
                OnPropertyChanged("IsRightRangeEditable");
                OnPropertyChanged("RRange");
                OnPropertyChanged("LRange");
                OnPropertyChanged("GateTypePosition");
            }
        }
        public List<string> ThresholdTypes { get { return control.ThresholdType.GetAllTypes(); } }

        public bool Developer { get; set; }
        public bool Root { get; set; }

        public ICommand GetCmd { get; set; }
        public ICommand SetCmd { get; set; }
        public ICommand DefaultCmd { get; set; }
        public ICommand StudyCmd { get; set; }


        public ICommand RebootCmd { get; set; }

        public ICommand GetWeakPointsCmd { get; set; }
        public ICommand CancelGetWeakPointsCmd { get; set; }
        public ICommand RemoveWeakPointsCmd { get; set; }
        public ICommand RegetWeakPointsCmd { get; set; }
        public ICommand CancelRemoveWeakPointsCmd { get; set; }

        public ICommand GetVersionCommand { get; set; }

        public ICommand ReLoginCommand { get; set; }

        public List<string> RecordTypes { get { return control.RecordKind.GetAllTypes(); } }
        public string Record { get; set; }

        public RadarType ConnectedRadarType { get; set; }

        public Action ExtraOnceWorkToDo { get; set; }

        private bool NewVersion { get; set; }  //version over 1.2.7

        public string FencePosition
        {
            get; set;
        }
        public bool IsFencePositionTypeVisible { get; set; }
        public bool IsLeftRangeEditable { get; set; }
        public bool IsRightRangeEditable { get; set; }

        public string imagepath = @"/DAHUA_ITS_A08;component/image/";
        public string GateTypePosition { get; set; }

        public static Geometry SportsCar
        {
            get
            {
                var g = Geometry.Parse("M12,8.5H7L4,11H3C1.89,11 1,11.89 1,13V16H3.17C3.6,17.2 4.73,18 6,18C7.27,18 8.4,17.2 8.82,16H15.17C15.6,17.2 16.73,18 18,18C19.27,18 20.4,17.2 20.82,16H23V15C23,13.89 21.97,13.53 21,13L12,8.5M5.25,12L7.5,10H11.5L15.5,12H5.25M6,13.5A1.5,1.5 0 0,1 7.5,15A1.5,1.5 0 0,1 6,16.5A1.5,1.5 0 0,1 4.5,15A1.5,1.5 0 0,1 6,13.5M18,13.5A1.5,1.5 0 0,1 19.5,15A1.5,1.5 0 0,1 18,16.5A1.5,1.5 0 0,1 16.5,15A1.5,1.5 0 0,1 18,13.5Z");
                g.Freeze();
                return g;
            }
        }

        public static Path TargetShape
        {
            get => new Path() { Fill=Brushes.DeepSkyBlue, Stretch= Stretch.Fill, Width=16, Height=16, Data = Geometry.Parse("M480 960h64v-256l-32-128-32 128v256zM544-64h-64v256l32 128 32-128v-256zM1024 480v-64h-256l-128 32 128 32h256zM0 416v64h256l128-32-128-32h-256zM512 800c194.404 0 352-157.596 352-352s-157.596-352-352-352c-194.404 0-352 157.596-352 352 0.364 194.258 157.742 351.636 351.965 352zM512 896c-247.424 0-448-200.576-448-448s200.576-448 448-448c247.424 0 448 200.576 448 448s-200.576 448-448 448v0z") };
        }

        public VisualElementsCollection Visuals { get; set; }

        public ChartValues<ObservablePoint> RodPoints { get => new ChartValues<ObservablePoint>() { new ObservablePoint(0.0, 0.0), new ObservablePoint(0.0, 10.0) }; }
        double x = 5, y = 5;
        public double TargetPositionX { get => x; set { x = value; } }
        public double TargetPositionY { get => y; set { y = value; } }

        public bool IsThroughing { get; set; }

        public Brush Light { get; set; }
        public SerialMainViewModel(MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator) : base(dialogCoordinator)
        {
            _progressViewModel = new ProgressViewModel()
            {
                CancelCommand = new SimpleCommand()
                {
                    ExecuteDelegate = param =>
                    {
                        _progressViewModel.Message = string.Empty;
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                        {
                            if (serial != null)
                            {
                                serial.CompareEndString = true;
                                serial.Rate = (int)ConnectModel.CustomRate;
                                serial.close();
                            }
                            await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                            mutex.Set();
                            if (ExtraOnceWorkToDo != null)
                            {
                                ExtraOnceWorkToDo();
                                ExtraOnceWorkToDo = null;
                            }
                        }));
                    }
                },
                IsIndeterminate = true
            };
            _progressCtrl.DataViewModel = _progressViewModel;

            ConnectModel = new ConnectViewModel(
                cancel =>
                {
                    Dispose();
                    if (_connectView.IsVisible)
                    {
                        _dialogCoordinator.HideMetroDialogAsync(this, _connectView);
                    }
                },
                confirm => { connect(); }
                );
            RateEditable = true;
            ItemSourceName = Application.Current.Resources["Serial"].ToString();

            _connectView = new ConnectView();
            _connectView.DataContext = ConnectModel;

            Developer = false;
            Root = false;

            PasswordModel = new PwdViewModel()
            {
                CancelCommand = new SimpleCommand()
                {
                    ExecuteDelegate = arg => { _dialogCoordinator.HideMetroDialogAsync(this, _pwdView); }
                },
                CheckCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async arg =>
                    {
                        if (!Developer && !Root)
                        {
                            Developer = PasswordModel.verifyDeveloper();
                            Root = PasswordModel.verifyRoot();
                        }
                        if (!Developer && !Root)
                            ShowErrorWindow(Application.Current.Resources["Password"].ToString() + Application.Current.Resources["Error"].ToString());
                        else
                        {
                            await _dialogCoordinator.HideMetroDialogAsync(this, _pwdView);
                            if (Developer)
                                await _dialogCoordinator.ShowMetroDialogAsync(this, _developView);
                            if (Root)
                                await _dialogCoordinator.ShowMetroDialogAsync(this, _rootView);
                        }
                    }
                }
            };
            _pwdView = new PasswordView();
            _pwdView.DataContext = PasswordModel;

            SetCmd = new SimpleCommand()
            {
                ExecuteDelegate = async param =>
                {
                    await AsyncWork(() => { ToSet(); });
                }
            };

            GetCmd = new SimpleCommand()
            {
                ExecuteDelegate = async param =>
                {
                    if (string.IsNullOrEmpty(Version))
                    {
                        await AsyncWork(() => { ToGetVer(); });
                        await AsyncWork(() => { ToGet(); });
                    }
                    else
                    {
                        await AsyncWork(() => { ToGet(); });
                    }
                }
            };

            DefaultCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    ShowConfirmCancelWindow(string.Empty, Tips.ToReset,
                        async () =>
                        {
                            await AsyncWork(() => ToReset(), true, 6000);
                        });
                }
            };

            StudyCmd = new SimpleCommand()
            {
                ExecuteDelegate = async param =>
                {
                    await AsyncWork(() => ToStudy(), true, -1);
                }
            };

            RebootCmd = new SimpleCommand()
            {
                ExecuteDelegate = async param =>
                {
                    //IsThroughing = !IsThroughing;
                    //OnPropertyChanged("IsThroughing");
                    await AsyncWork(() => ToReboot(), true, 10000);
                }
            };

            _recordView = new RecordsView();
            RecordModel = new RecordViewModel(cancel => { Dispose(); if (_recordView.IsVisible) { _dialogCoordinator.HideMetroDialogAsync(this, _recordView); } })
            {
                GetTimeCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToGetTime());
                    }
                },
                SetTimeCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToSetTime());
                    }
                },
                SynTimeCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToSynchronizeTime());
                    }
                },
                ClearRecordCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToClarRecords(), true, 6000);
                    }
                },
                SearchCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToSearch(), false, -1);
                    }
                },
                SearchResult = new ObservableCollection<string>(),
                CurrentTime = DateTime.Now,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                InvertSearchCount = "30",
                InvertSearchCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToInvertSearch());
                    }
                }
            };
            _recordView.DataContext = RecordModel;
            GetWeakPointsCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    ShowConfirmCancelWindow(string.Empty, Tips.KeepLifting,
                        async () => {
                            await AsyncWork(() => toGetWeakPoints(), true, -1);
                        });
                }
            };
            CancelGetWeakPointsCmd = new SimpleCommand()
            {
                ExecuteDelegate = async param =>
                {
                    if (IsGettingStrongestPoints)
                        await AsyncWork(() => toCancelGetWeakPoints());
                }
            };
            RemoveWeakPointsCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    ShowConfirmCancelWindow(string.Empty, Tips.KeepLifting,
                        async () =>
                        {
                            await AsyncWork(() => toRemoveWeakPonints(), false, 6000);
                        });
                }
            };
            RegetWeakPointsCmd = new SimpleCommand()
            {
                ExecuteDelegate = async param =>
                {
                    await AsyncWork(() => toGetRemovedWeakPoints());
                }
            };
            CancelRemoveWeakPointsCmd = new SimpleCommand()
            {
                ExecuteDelegate = async param =>
                {
                    await AsyncWork(() => toCancelRemoveWeakPoints(), true, -1);
                }
            };

            StrongestWeakPoints = new ChartValues<TargetPoint>();
            RemovedWeakPoints = new ChartValues<TargetPoint>();
            RangePoints = new ChartValues<ObservablePoint>();
            //RangePoints = new ChartValues<ObservablePoint>() { new ObservablePoint(-5,1), new ObservablePoint(5,1), new ObservablePoint(5, 5), new ObservablePoint(-5, 5)};
            //Random rand = new Random();
            //for (int i = 0; i < 200; i++)
            //{
            //    StrongestWeakPoints.Add(new TargetPoint(rand.NextDouble() * 10.0 - 5.0, rand.NextDouble() * 10.0 - 5.0));
            //}
            //_randomPointTimer = new Timer(async param =>
            //{
            //    await Task.Factory.StartNew(() =>
            //    {
            //        lock (StrongestWeakPoints)
            //        {
            //            if (StrongestWeakPoints.Count > 0)
            //            {
            //                for (int i = 0; i < 200; i++)
            //                {
            //                    StrongestWeakPoints[i].X = rand.NextDouble() * 10.0 - 5.0;
            //                    StrongestWeakPoints[i].Y = rand.NextDouble() * 10.0 - 5.0;
            //                }
            //            }
            //        }
            //    });
            //}, null, 0, 100);

            pointsDataReceiveTimer = new Timer(param =>
            {
                if (showStrongestPoints)
                {
                    showStrongestPoints = false;
                }
                else
                {
                    StrongestWeakPoints.Clear();
                }
            }, null, 0, 1000);

            _compareView = new ComparisonView();
            CompareModel = new ComparisonViewModel(cancel => { Dispose(); if (_compareView.IsVisible) { _dialogCoordinator.HideMetroDialogAsync(this, _compareView); } })
            {
                BackgroundAfterPoints = new ChartValues<ObservablePoint>(),
                BackgroundBeforePoints = new ChartValues<ObservablePoint>(),
                BackgroundComparison = false,
                GetBackPointsCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToGetAfterPoints());
                    }
                },
                ComparisonCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToGetCorrelation());
                    }
                },
                BackgroundSeries = new SeriesCollection()
                {
                    new LiveCharts.Wpf.LineSeries()
                                {
                                    Title = Application.Current.Resources["BeforeChartPointsTitle"].ToString(),
                                    Values = new ChartValues<ObservablePoint>()
                                },
                    new LiveCharts.Wpf.LineSeries()
                                {
                                    Title = Application.Current.Resources["AfterChartPointsTitle"].ToString(),
                                    Values=new ChartValues<ObservablePoint>(),
                                    PointGeometry = LiveCharts.Wpf.DefaultGeometries.Diamond
                                }
                },
                CanCompare = false,
            };
            _compareView.DataContext = CompareModel;
            _rootView = new RootDevelopeView();
            RootModel = new RootDevelopViewModel(cancel => { Dispose(); if (_rootView.IsVisible) { _dialogCoordinator.HideMetroDialogAsync(this, _rootView); } })
            {
                WriteCLICommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToWriteCLI());
                    }
                },
                CustomCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToWriteCustom(), true, -1);
                    }
                },
                ReadCLIAllCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToReadAll());
                    }
                },
                SensorstopCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToSensorstop());
                    }
                },
            };
            _rootView.DataContext = RootModel;
            _developView = new UserDevelopeView();
            DevelopModel = new UserDevelopViewModel(cancel => { Dispose(); if (_developView.IsVisible) { _dialogCoordinator.HideMetroDialogAsync(this, _developView); } })
            {
                WriteCLIRangeCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToWriteRange());
                    }
                },
                WorkAnomalousCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToGetAnomalousReason(), true, -1);
                    }
                },
                WriteCLIRainCommand = new SimpleCommand()
                {
                    ExecuteDelegate = async param =>
                    {
                        await AsyncWork(() => ToWriteRain());
                    }
                },
                WriteCLIRangeCommandMaxStr = string.Empty,
                WriteCLIRangeCommandMinStr = string.Empty,
            };
            _developView.DataContext = DevelopModel;

            _updateView = new UpdateView();
            UpdateModel = new UpdateViewModel(cancel => { Dispose(); if (_updateView.IsVisible) { _dialogCoordinator.HideMetroDialogAsync(this, _updateView); } })
            {
                UpdateCommand = new SimpleCommand()
                {
                    ExecuteDelegate = param =>
                    {
                        ToDo();
                    }
                },
                CustomUpdateRate = BauRate.Rate115200
            };
            _updateView.DataContext = UpdateModel;

            GetVersionCommand = new SimpleCommand()
            {
                ExecuteDelegate = async param =>
                {
                    await AsyncWork(() => ToGetVer());
                }
            };

            ReLoginCommand = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    ToRelogin();
                }
            };

            ConnectState = ConnectType.Ready;
            Charting.For<TargetPoint>(Mappers.Xy<TargetPoint>()
                .X((value, index) => value.X)
                .Y(value => value.Y));
            Visuals = new VisualElementsCollection();
            Visuals.Add(new VisualElement {UIElement = TargetShape, X=-0.2, Y=0.2});

            Light = Brushes.Red;
        }

        private void ToRelogin()
        {
            this.Dispose();
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                ConnectViewModel tmp = ConnectModel;
                ConnectModel = new ConnectViewModel(
                async cancel => { ConnectModel.CustomRate = tmp.CustomRate; await _dialogCoordinator.HideMetroDialogAsync(this, _connectView); },
                confirm => { connect(); }
                );
                _connectView.DataContext = ConnectModel;
                this.ToConnect();
            }));
        }

        private void ToWriteRain()
        {
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    ShowConfirmWindow(Tips.ConfigSuccess, string.Empty);
                }
                else
                {
                    ShowErrorWindow(Tips.ConfigFail);
                }
                serial.StringDataReceivedHandler = null;
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.WriteCLI + " setThresholdParas  325 0.86 0.00 120 5 0.68 1.50");
        }

        private void ToGetAnomalousReason()
        {
            string lastop = " setThresholdParas";
            double standCorrelation = 0.0, area = 0.0, mc = 0.0, Var = 0.0;
            serial.StringDataReceivedHandler = async msg =>
            {
                if(msg.Contains(SerialRadarReply.Error))
                {
                    mutex.Set();
                    ShowErrorWindow(ErrorString.Error + ":" + msg);
                    return;
                }
                if (lastop == " setThresholdParas")
                {
                    var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");
                    if (collection.Count > 6)
                    {
                        try
                        {
                            standCorrelation = double.Parse(collection[2].Value) + double.Parse(collection[5].Value);
                            await TaskEx.Delay(300);
                            lastop = " setUpRodSubArea";
                            serial.WriteLine(SerialRadarCommands.ReadCLI + " setUpRodSubArea");
                            return;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (lastop == " setUpRodSubArea")
                {
                    var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");
                    if (collection.Count > 2)
                    {
                        try
                        {
                            await TaskEx.Delay(300);
                            area = double.Parse(collection[0].Value);
                            lastop = "clioutput 2";
                            serial.WriteLine(SerialRadarCommands.Output + " 2");
                            return;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (lastop == "clioutput 2")
                {
                    if (msg.Contains(SerialRadarReply.Done))
                    {
                        _progressViewModel.IsIndeterminate = false;
                        _progressViewModel.MaxValue = 100;
                        _progressViewModel.Value = 0;
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                        {
                            if (!_progressCtrl.IsVisible)
                            {
                                _progressViewModel.Message = Tips.ReasonAnomalousSearching;
                                await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
                            }
                        }));
                        serial.EndStr = "\n";
                    }
                    else if (msg.Contains("MC"))
                    {
                        var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");
                        if (collection.Count > 1)
                        {
                            try
                            {
                                mc = double.Parse(collection[0].Value);
                                Var = double.Parse(collection[1].Value);
                                if (mc < (standCorrelation) || Var > area)
                                {
                                    _progressViewModel.Message = Tips.ReasonAnomalousSearching + Tips.RangeProfileError;
                                }
                                else
                                {
                                    _progressViewModel.Message = Tips.ReasonAnomalousSearching + Tips.PointDataError;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    return;
                }
                ShowErrorWindow(msg);
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.ReadCLI + " setThresholdParas");
            ExtraOnceWorkToDo = async () =>
            {
                await AsyncWork(() =>
                {
                    serial.WriteLine(SerialRadarCommands.SoftReset, 300, false);
                    serial.StringDataReceivedHandler = null;
                    mutex.Set();
                });
            };
            //_progressViewModel.IsIndeterminate = true;
            //_progressViewModel.MaxValue = 100;
            //_progressViewModel.Value = 0;
            //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            //{
            //    if (!_progressCtrl.IsVisible)
            //    {
            //        _progressViewModel.Message = Tips.ReasonAnomalousSearching;
            //        await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
            //    }
            //}));
        }

        private void ToWriteRange()
        {
            string error = string.Empty;
            try
            {
                double max = double.Parse(DevelopModel.WriteCLIRangeCommandMaxStr);
                double min = double.Parse(DevelopModel.WriteCLIRangeCommandMinStr);
                if (max > min)
                {
                    string lastOperation = SerialRadarCommands.ReadCLI;
                    serial.StringDataReceivedHandler = async msg =>
                    {
                        if (msg.Contains(SerialRadarReply.Done))
                        {
                            if (lastOperation == SerialRadarCommands.ReadCLI)
                            {
                                msg = msg.Substring(msg.IndexOf(SerialArguments.FilterParam));
                                var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");
                                if (collection.Count > 10)
                                {
                                    try
                                    {
                                        float lrange = float.Parse(collection[2].Value) / 10;
                                        float rrange = float.Parse(collection[6].Value) / 10;

                                        if (max < rrange && min > -lrange)
                                        {
                                            await TaskEx.Delay(100);
                                            lastOperation = " setUpRodSubArea ";
                                            serial.WriteLine(SerialRadarCommands.WriteCLI + " setUpRodSubArea " + max + " " + min + " 200");
                                            return;
                                        }
                                        else
                                        {
                                            ShowErrorWindow(Tips.RangeError + "-" + lrange + "~" + rrange);
                                            mutex.Set();
                                            return;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        error = ex.Message;
                                    }
                                }
                                error = Tips.GetFail;
                            }
                            else if (lastOperation == " setUpRodSubArea ")
                            {
                                ShowConfirmWindow(Tips.ConfigSuccess, string.Empty);
                                mutex.Set();
                                return;
                            }
                            else
                            {
                                mutex.Set();
                                return;
                            }
                        }
                        if (msg.Contains("large"))
                            ShowErrorWindow(Tips.RangeError);
                        else
                            ShowErrorWindow(msg);
                        serial.StringDataReceivedHandler = null;
                        mutex.Set();
                    };
                    serial.WriteLine(SerialRadarCommands.ReadCLI + " " + SerialArguments.FilterParam);
                    return;
                }
                else
                {
                    error = max + "<" + min;
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                ShowErrorWindow(ErrorString.ParamError + ":" + ex.Message);
                mutex.Set();
            }
        }

        private void ToSensorstop()
        {
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    ShowConfirmWindow(Tips.ConfigSuccess, string.Empty);
                }
                else
                {
                    ShowErrorWindow(Tips.ConfigFail);
                }
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }

        private void ToReadAll()
        {
            string lastOperation = SerialRadarCommands.SensorStop;
            serial.StringDataReceivedHandler = async msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        lastOperation = SerialRadarCommands.ReadCLI;
                        await TaskEx.Delay(300);
                        serial.WriteLine(SerialRadarCommands.ReadCLI + " all");
                    }
                    else if (lastOperation == SerialRadarCommands.ReadCLI)
                    {
                        ShowConfirmWindow(msg.Replace("Done", "").TrimEnd('\n', '\r'), string.Empty);
                        await TaskEx.Delay(300);
                        serial.WriteLine(SerialRadarCommands.SensorStart);
                        mutex.Set();
                        serial.close();
                    }
                }
                else
                {
                    ShowErrorWindow(msg);
                    mutex.Set();
                    serial.close();
                }
            };
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }

        private void ToWriteCustom()
        {
            _progressViewModel.Message = string.Empty;
            serial.StringDataReceivedHandler = msg =>
            {
                _progressViewModel.Message += msg;
            };
            serial.WriteLine(RootModel.CustomCommandStr);
            _progressViewModel.IsIndeterminate = false;
            _progressViewModel.MaxValue = 100;
            _progressViewModel.Value = 0;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                if (!_progressCtrl.IsVisible)
                {
                    await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
                }
            }));
            serial.EndStr = "\n";
        }

        private void ToWriteCLI()
        {
            serial.StringDataReceivedHandler = msg =>
            {
                ShowConfirmWindow(msg, string.Empty);
                serial.StringDataReceivedHandler = null;
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.WriteCLI + " " + RootModel.WriteCLICommandStr);
        }

        private void toCancelGetWeakPoints()
        {
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    StrongestWeakPoints.Clear();
                    IsGettingStrongestPoints = false;
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
        public bool IsGettingStrongestPoints = false;
        private void toGetWeakPoints()
        {
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains("X"))
                {
                    var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");
                    if (collection.Count > 1)
                    {
                        double _x = double.Parse(collection[0].Value);
                        double _y = double.Parse(collection[1].Value);
                        if (StrongestWeakPoints.Count > 0)
                        {
                            StrongestWeakPoints[0].X = _x;
                            StrongestWeakPoints[0].Y = _y;
                        }
                        else
                        {
                            StrongestWeakPoints.Add(new TargetPoint(_x, _y));
                        }
                        ToAnalysePoints();
                    }
                    showStrongestPoints = true;
                }
                else if (string.IsNullOrEmpty(msg))
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
            serial.WriteLine(SerialRadarCommands.AlarmOrder0, -1);
            IsGettingStrongestPoints = true;
        }

        private void ToAnalysePoints()
        {
            foreach(var point in StrongestWeakPoints)
            {
                TargetPositionX = point.X;
                TargetPositionY = point.Y;
            }
            OnPropertyChanged("TargetPositionX");
            OnPropertyChanged("TargetPositionY");
            Application.Current.Dispatcher.BeginInvoke((Action)(() => {
                Visuals[0].X = TargetPositionX-0.1;
                Visuals[0].Y = TargetPositionY+0.1;
                //OnPropertyChanged("Visuals");
            }));
        }

        private void toCancelRemoveWeakPoints()
        {
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    RemovedWeakPoints.Clear();
                }
                else if (msg.Contains("X"))
                {
                    var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");
                    if (collection.Count > 1)
                    {
                        double _x = double.Parse(collection[0].Value);
                        double _y = double.Parse(collection[1].Value); ;
                        if (StrongestWeakPoints.Count > 0)
                        {
                            StrongestWeakPoints[0].X = _x;
                            StrongestWeakPoints[0].Y = _y;
                        }
                        else
                        {
                            StrongestWeakPoints.Add(new TargetPoint(_x, _y));
                        }
                        ToAnalysePoints();
                    }
                    showStrongestPoints = true;
                }
                else if (string.IsNullOrEmpty(msg))
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
            serial.WriteLine(SerialRadarCommands.AlarmOrder3, -1);
        }

        private void toRemoveWeakPonints()
        {
            serial.StringDataReceivedHandler = msg =>
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
            serial.StringDataReceivedHandler = msg =>
            {
                RemovedWeakPoints.Clear();
                if (msg.Contains(SerialRadarReply.Done))
                {
                    var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");
                    if (collection.Count == 0)
                    {
                        ShowConfirmWindow(Tips.NoTargets, string.Empty);
                    }
                    else
                    {
                        for (int i = 0; i < collection.Count; i += 2)
                        {
                            RemovedWeakPoints.Add(new TargetPoint(double.Parse(collection[i].Value), double.Parse(collection[i + 1].Value)));
                        }
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

        //public ChartValues<TargetPoint> StrongestWeakPoints { get => new ChartValues<TargetPoint>() { new TargetPoint(0.0, 0.0), new TargetPoint(0.5, 0.5), new TargetPoint(1.0, 1.0), new TargetPoint(0.5, 1.0), new TargetPoint(1.0, 0.5), new TargetPoint(1.5, 2.0), new TargetPoint(2.0, 1.5) }; }
        public ChartValues<TargetPoint> StrongestWeakPoints { get; set; }
        Timer _randomPointTimer = null;
        public ChartValues<TargetPoint> RemovedWeakPoints { get; set; }
        public ChartValues<ObservablePoint> RangePoints { get; set; }

        private void ToSearch()
        {
            string lastOperation = SerialRadarCommands.SearchTime;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                RecordModel.SearchResult.Clear();
            }));
            serial.EndStr = SerialRadarReply.NewLine;
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    switch (lastOperation)
                    {
                        case SerialRadarCommands.BootLoader:
                            lastOperation = SerialRadarCommands.SearchTime;
                            // serial.WriteLine("DalayFilpTime 1970 1 1 0 0 0 1970 1 1 1 1 40");
                            serial.WriteLine(SerialRadarCommands.SearchTime + " " + RecordModel.StartTime.Year +
                                                                                                                           " " + RecordModel.StartTime.Month +
                                                                                                                           " " + RecordModel.StartTime.Day +
                                                                                                                           " " + RecordModel.StartTime.Hour +
                                                                                                                           " " + RecordModel.StartTime.Minute +
                                                                                                                           " " + RecordModel.StartTime.Second +
                                                                                                                           " " + RecordModel.EndTime.Year +
                                                                                                                           " " + RecordModel.EndTime.Month +
                                                                                                                           " " + RecordModel.EndTime.Day +
                                                                                                                           " " + RecordModel.EndTime.Hour +
                                                                                                                           " " + RecordModel.EndTime.Minute +
                                                                                                                           " " + RecordModel.EndTime.Second, 6000);
                            break;
                        case SerialRadarCommands.SearchTime:
                            lastOperation = string.Empty;
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                            {
                                lock (RecordModel.SearchResult)
                                {
                                    msg.Split(new char[] { '\r', '\n' }).ToList().ForEach(str =>
                                    {
                                        if (str != string.Empty && str.Contains(OperationType.Value))
                                        {
                                            str = str.Replace(OperationType.UpValue, OperationType.Up);
                                            str = str.Replace(OperationType.DownValue, OperationType.Down);
                                            RecordModel.SearchResult.Add(str);
                                        }
                                    });

                                    if (RecordModel.SearchResult.Count == 0)
                                    {
                                        ShowConfirmWindow(Tips.SearchTimeGetNone, string.Empty);
                                    }
                                    else
                                    {
                                        ShowConfirmWindow(Tips.SearchTimeSuccess, string.Empty);
                                    }
                                    serial.StringDataReceivedHandler = null;
                                    mutex.Set();
                                }
                            }));
                            break;
                    }
                }
                else if (lastOperation == SerialRadarCommands.SensorStart)
                {
                    lock (RecordModel.SearchResult)
                    {
                        if (RecordModel.SearchResult.Count == 0)
                        {
                            ShowConfirmWindow(Tips.SearchTimeGetNone, string.Empty);
                        }
                        else
                        {
                            ShowConfirmWindow(Tips.SearchTimeSuccess, string.Empty);
                        }
                    }
                    serial.StringDataReceivedHandler = null;
                    mutex.Set();
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    mutex.Set();
                    ShowErrorWindow(Tips.SearchTimeFail);
                }
                else
                {
                    if (lastOperation == SerialRadarCommands.SearchTime)
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                        {
                            lock (RecordModel.SearchResult)
                            {
                                msg.Split(new char[] { '\r', '\n' }).ToList().ForEach(str =>
                                {
                                    if (str != string.Empty && str.Contains(OperationType.Value))
                                    {
                                        str = str.Replace(OperationType.UpValue, OperationType.Up);
                                        str = str.Replace(OperationType.DownValue, OperationType.Down);
                                        RecordModel.SearchResult.Add(str);
                                    }
                                });
                            }
                        }));
                    }
                    else
                    {
                        mutex.Set();
                        ShowErrorWindow(Tips.SearchTimeFail);
                    }
                }
            };
            serial.WriteLine(SerialRadarCommands.SearchTime + " " + RecordModel.StartTime.Year +
                                                                                                                           " " + RecordModel.StartTime.Month +
                                                                                                                           " " + RecordModel.StartTime.Day +
                                                                                                                           " " + RecordModel.StartTime.Hour +
                                                                                                                           " " + RecordModel.StartTime.Minute +
                                                                                                                           " " + RecordModel.StartTime.Second +
                                                                                                                           " " + RecordModel.EndTime.Year +
                                                                                                                           " " + RecordModel.EndTime.Month +
                                                                                                                           " " + RecordModel.EndTime.Day +
                                                                                                                           " " + RecordModel.EndTime.Hour +
                                                                                                                           " " + RecordModel.EndTime.Minute +
                                                                                                                           " " + RecordModel.EndTime.Second, 6000);
        }

        private void ToInvertSearch()
        {
            try
            {
                int number = int.Parse(RecordModel.InvertSearchCount);
            }
            catch (Exception)
            {
                RecordModel.InvertSearchCount = "30";
                OnPropertyChanged("InvertSearchCount");
            }
            string lastOperation = SerialRadarCommands.SearchTime;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                RecordModel.SearchResult.Clear();
            }));
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    switch (lastOperation)
                    {
                        case SerialRadarCommands.SearchTime:
                            lastOperation = string.Empty;
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                            {
                                lock (RecordModel.SearchResult)
                                {
                                    msg.Split(new char[] { '\r', '\n' }).ToList().ForEach(str =>
                                    {
                                        if (str != string.Empty && str.Contains(OperationType.Value))
                                        {
                                            str = str.Replace(OperationType.UpValue, OperationType.Up);
                                            str = str.Replace(OperationType.DownValue, OperationType.Down);
                                            RecordModel.SearchResult.Add(str);
                                        }
                                    });

                                    if (RecordModel.SearchResult.Count == 0)
                                    {
                                        ShowConfirmWindow(Tips.SearchTimeGetNone, string.Empty);
                                    }
                                    else
                                    {
                                        ShowConfirmWindow(Tips.SearchTimeSuccess, string.Empty);
                                    }
                                    serial.StringDataReceivedHandler = null;
                                    mutex.Set();
                                }
                            }));
                            break;
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    mutex.Set();
                    ShowErrorWindow(Tips.SearchTimeFail);
                }
                else
                {
                    if (lastOperation == SerialRadarCommands.SearchTime)
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                        {
                            lock (RecordModel.SearchResult)
                            {
                                msg.Split(new char[] { '\r', '\n' }).ToList().ForEach(str =>
                                {
                                    if (str != string.Empty && str.Contains(OperationType.Value))
                                    {
                                        str = str.Replace(OperationType.UpValue, OperationType.Up);
                                        str = str.Replace(OperationType.DownValue, OperationType.Down);
                                        RecordModel.SearchResult.Add(str);
                                    }
                                });
                            }
                        }));
                    }
                    else
                    {
                        mutex.Set();
                        ShowErrorWindow(Tips.SearchTimeFail);
                    }
                }
            };
            serial.WriteLine(SerialRadarCommands.SearchInvert + " " + RecordModel.InvertSearchCount);
        }

        private void ToClarRecords()
        {
            string lastOperation = SerialRadarCommands.ClearTime;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
            {
                _progressViewModel.IsIndeterminate = true;
                _progressViewModel.Message = Tips.ClearTiming;
                await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
            }));
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    switch (lastOperation)
                    {
                        case SerialRadarCommands.SensorStop:
                            lastOperation = SerialRadarCommands.ClearTime;
                            serial.WriteLine(SerialRadarCommands.ClearTime, 2000);
                            break;
                        case SerialRadarCommands.ClearTime:
                            lastOperation = string.Empty;
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                            {
                                _progressViewModel.Message = Tips.ClearTimeSuccess;
                                await TaskEx.Delay(1000);
                                if (_progressCtrl.IsVisible)
                                    await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                            }));
                            //serial.WriteLine(SerialRadarCommands.SoftReset, 1000, false);
                            mutex.Set();
                            break;
                        case SerialRadarCommands.SensorStart:
                            break;
                    }
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                    {
                        _progressViewModel.Message = Tips.ClearTimeFail;
                        _progressViewModel.MaxValue = 1;
                        _progressViewModel.Value = 0;
                        _progressViewModel.IsIndeterminate = false;
                    }));
                    serial.StringDataReceivedHandler = null;
                    mutex.Set();
                }
            };
            serial.WriteLine(SerialRadarCommands.ClearTime, 5000);
        }

        private void ToSetTime()
        {
            string lastOperation = SerialRadarCommands.SetTime;
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    ShowSplashWindow(Tips.SetTimeSuccess, 1000);
                }
                else
                {
                    ShowErrorWindow(Tips.SetTimeFail);
                }
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.SetTime + " " + RecordModel.CurrentTime.Year.ToString() + " " + RecordModel.CurrentTime.Month + " " + RecordModel.CurrentTime.Day + " " + RecordModel.CurrentTime.Hour + " " + RecordModel.CurrentTime.Minute + " " + RecordModel.CurrentTime.Second);
        }
        private void ToSynchronizeTime()
        {
            string lastOperation = SerialRadarCommands.SetTime;
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    ShowSplashWindow(Tips.SetTimeSuccess, 1000);
                }
                else
                {
                    ShowErrorWindow(Tips.SetTimeFail);
                }
                mutex.Set();
            };
            RecordModel.CurrentTime = System.DateTime.Now;
            RecordModel.Notify("CurrentTime");
            serial.WriteLine(SerialRadarCommands.SetTime + " " + RecordModel.CurrentTime.Year.ToString() + " " + RecordModel.CurrentTime.Month + " " + RecordModel.CurrentTime.Day + " " + RecordModel.CurrentTime.Hour + " " + RecordModel.CurrentTime.Minute + " " + RecordModel.CurrentTime.Second);
        }

        private void ToGetTime()
        {
            string lastOperation = SerialRadarCommands.GetTIme;
            serial.StringDataReceivedHandler = msg =>
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
                            try
                            {
                                msg = msg.Substring(msg.IndexOf("Time(GMT)"));
                                string[] date = msg.Trim('\n', ' ').Split('\r', '\n', ' ');
                                if (date[3] == "")
                                    RecordModel.CurrentTime = DateTime.Parse(date[1] + " " + date[2] + " " + date[4] + " " + date[6] + " " + date[5]);
                                else
                                    RecordModel.CurrentTime = DateTime.Parse(date[1] + " " + date[2] + " " + date[3] + " " + date[5] + " " + date[4]);
                                RecordModel.Notify("CurrentTime");
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

        private async void connect()
        {
            if (ConnectModel.CustomItem == "" || ConnectModel.CustomItem == null)
            {
                ShowErrorWindow(ErrorString.SerialError);
                return;
            }
            else if (ConnectModel.CustomRate == 0)
            {
                ShowErrorWindow(ErrorString.ParamError);
                return;
            }
            ConnectState = ConnectType.Connecting;
            if (serial != null)
            {
                serial.close();
            }
            serial = new SerialManager(GetSerialPortName(ConnectModel.CustomItem));
            serial.PortName = GetSerialPortName(ConnectModel.CustomItem);
            serial.Rate = (int)ConnectModel.CustomRate;
            serial.ToTranslate = ConnectModel.CustomItem.Contains("蓝牙");
            if (serial.IsOpen)
                serial.close();
            if (!serial.open())
            {
                ShowErrorWindow(ErrorString.SerialOpenError);
                ConnectState = ConnectType.Disconnected;
                return;
            }
            ConnectState = ConnectType.Connected;
            OnPropertyChanged("ConnectState");
            await AsyncWork(() => ToGetVer(), false, 1000);
            if (string.IsNullOrEmpty(Version))
            {
                if(!serial.ToTranslate)
                {
                    serial.ToTranslate = true;
                    await AsyncWork(() => ToGetVer(), false);
                    if (string.IsNullOrEmpty(Version))
                    {
                        ShowErrorWindow(ErrorString.ConnectError);
                        return;
                    }
                }
            }
            await AsyncWork(() => JudgeRadarType());
            await Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
                {
                    if (_connectView.IsVisible)
                    {
                        await _dialogCoordinator.HideMetroDialogAsync(this, _connectView);
                    }
                }));
                if (ConnectModel.SelectedRadarType == Application.Current.Resources["RadarTriggerType"].ToString())
                {
                    ConnectedRadarType = RadarType.Trigger;
                }
                else
                {
                    ConnectedRadarType = RadarType.Hold;
                }
                OnPropertyChanged("IsHoldRadarType");
            });
        }

        ManualResetEvent mutex = new ManualResetEvent(false);
        bool finished = true;
        private async Task AsyncWork(Action towork, bool toShowOverTimeTip = true, int waitmillionseoconds = 3000, Action overTimeToDo = null)
        {
            if (finished)
            {
                finished = false;
                if (serial == null)
                {
                    serial = new SerialManager(GetSerialPortName(ConnectModel.CustomItem));
                }
                serial.Rate = (int)ConnectModel.CustomRate;
                serial.Type = SerialReceiveType.Chars;
                serial.CompareEndString = true;
                serial.EndStr = SerialRadarReply.Done;
                serial.ClearBuffer();
                if (!serial.IsOpen && !serial.open())
                {
                    ShowErrorWindow(ErrorString.SerialOpenError);
                    finished = true;
                    return;
                }
                await Task.Factory.StartNew(
                    () =>
                    {
                        towork();
                        if (waitmillionseoconds == -1)
                            finished = true;
                        if (!mutex.WaitOne(waitmillionseoconds))
                        {
                            if (toShowOverTimeTip)
                                ShowErrorWindow(ErrorString.OverTime);
                            if (overTimeToDo != null)
                            {
                                overTimeToDo();
                            }
                        }
                        mutex.Reset();
                        finished = true;
                    });
            }
        }
        private void StepWork(Action towork, bool toShowOverTimeTip = true, int waitmillionseoconds = 500, Action overTimeToDo = null)
        {
            if (serial == null)
                serial = new SerialManager(GetSerialPortName(ConnectModel.CustomItem));
            serial.close();
            serial.Rate = (int)ConnectModel.CustomRate;
            serial.Type = SerialReceiveType.Chars;
            if (!serial.open())
            {
                ShowErrorWindow(ErrorString.SerialOpenError);
                return;
            }
            towork();
            if (!mutex.WaitOne(waitmillionseoconds))
            {
                if (toShowOverTimeTip)
                    ShowErrorWindow(ErrorString.OverTime);
                if (overTimeToDo != null)
                {
                    overTimeToDo();
                }
            }
            mutex.Reset();
        }
        private void ToGetDelayAndFencePosition(string tip)
        {
            string lastOperation = SerialRadarCommands.ReadCLI;
            serial.StringDataReceivedHandler = async msg =>
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
                         RootModel.Delay = System.Text.RegularExpressions.Regex.Match(msg, @"\d+").Value;
                         RootModel.Notify("Delay");
                         await TaskEx.Delay(100);
                         if (Gate == control.GateType.Straight)
                         {
                             IsLeftRangeEditable = true;
                             IsRightRangeEditable = true;
                             OnPropertyChanged("IsLeftRangeEditable");
                             OnPropertyChanged("IsRightRangeEditable");
                         }
                         else if (IsFencePositionTypeVisible)
                         {
                             lastOperation = SerialArguments.RodDirection;
                             serial.WriteLine(SerialRadarCommands.ReadCLI + " " + SerialArguments.RodDirection);
                             return;
                         }
                         OnPropertyChanged("Gate");
                         lastOperation = string.Empty;
                         if (tip != string.Empty)
                             ShowConfirmWindow(tip, string.Empty);
                         mutex.Set();
                         serial.close();
                     }
                     else if (lastOperation == SerialArguments.RodDirection)
                     {
                         string position = System.Text.RegularExpressions.Regex.Match(msg, @"\d").Value;
                         Gate = control.GateType.GetTypeByPosition(Gate, position);
                         FencePosition = control.FencePositionType.GetType(position);
                         OnPropertyChanged("Gate");
                         lastOperation = string.Empty;
                         if (tip != string.Empty)
                             ShowConfirmWindow(tip, string.Empty);
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
                 else if (msg.Contains(SerialRadarReply.Start))
                 {
                 }
             };
            serial.WriteLine(SerialRadarCommands.ReadCLI + " " + SerialArguments.DelayTimeParam);
        }
        private void ToSetDelayAndFencePosition(bool reset = false)
        {
            int delay = 6;
            try
            {
                if (!reset)
                    delay = int.Parse(RootModel.Delay);
                if (delay < 0 || delay > 300.00001)
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
            serial.StringDataReceivedHandler = async msg =>
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
                            RootModel.Delay = delay.ToString();
                            RootModel.Notify("Delay");
                        }
                        if (IsFencePositionTypeVisible && gate != control.GateType.Straight)
                        {
                            lastOperation = SerialArguments.RodDirection;
                            await TaskEx.Delay(100);
                            serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.RodDirection + " " + control.FencePositionType.GetValue(gate));
                        }
                        else
                        {
                            lastOperation = string.Empty;
                            await TaskEx.Delay(300);
                            serial.EndStr = SerialRadarReply.Start;
                            serial.WriteLine(SerialRadarCommands.SoftReset, 300, false);
                            //mutex.Set();
                        }
                    }
                    else if (lastOperation == SerialArguments.RodDirection)
                    {
                        lastOperation = string.Empty;
                        await TaskEx.Delay(300);
                        serial.EndStr = SerialRadarReply.Start;
                        serial.WriteLine(SerialRadarCommands.SoftReset, 300, false);
                        //mutex.Set();
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    serial.StringDataReceivedHandler = null;
                    ShowErrorWindow(Tips.ConfigFail);
                    mutex.Set();
                }
                else if (msg.Contains(SerialRadarReply.Start))
                {
                    mutex.Set();
                    ShowConfirmWindow(Tips.ConfigSuccess, string.Empty);
                }
            };
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }

        const double SampleRate = 0.045;
        double standCorrelation = 0.68;
        double correlation = 0.0;
        List<double> beforeVals = null;
        List<double> afterVals = null;
        private void ToGetAfterPoints()
        {
            serial.EndStr = SerialRadarReply.Done;
            serial.StringDataReceivedHandler = msg =>
            {
                var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");
                if (collection.Count > 0)
                {
                    string tip = string.Empty;
                    if (CompareModel.BackgroundAfterPoints.Count == 0)
                    {
                        tip = Tips.ToSaveCorrelationData;
                    }
                    else if (CompareModel.BackgroundAfterPoints.Count > 0 && CompareModel.BackgroundBeforePoints.Count == 0)
                    {
                        tip = Tips.ToSaveCorrelationData2;
                    }
                    else
                    {
                        tip = Tips.ToSaveCorrelationData3;
                    }
                    ShowConfirmCancelWindow(tip, string.Empty,
                        () =>
                        {
                            if (CompareModel.BackgroundAfterPoints.Count > 0)
                            {
                                CompareModel.BackgroundBeforePoints.Clear();
                                foreach (var point in CompareModel.BackgroundAfterPoints.AsEnumerable())
                                {
                                    CompareModel.BackgroundBeforePoints.Add(point);
                                }
                                beforeVals = afterVals;

                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                                {
                                    CompareModel.BackgroundSeries[0].Values.Clear();
                                    CompareModel.BackgroundSeries[0].Values.AddRange(CompareModel.BackgroundBeforePoints);
                                    CompareModel.BackgroundSeries[0].OnSeriesUpdatedFinish();
                                }));
                            }
                            CompareModel.BackgroundAfterPoints.Clear();
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                            {
                                CompareModel.BackgroundSeries[1].Values.Clear();
                            }));
                            afterVals = new List<double>();
                            int x = 0;
                            for (int index = 0; index < collection.Count - 1; index++)
                            {
                                double y = double.Parse(collection[index].Value);
                                if (y > 0.00001 || y < -0.00001)
                                {
                                    CompareModel.BackgroundAfterPoints.Add(new ObservablePoint(x * SampleRate, y));
                                    afterVals.Add(y);
                                    x++;
                                }
                            }
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                            {
                                CompareModel.BackgroundSeries[1].Values.AddRange(CompareModel.BackgroundAfterPoints);
                                CompareModel.BackgroundSeries[1].OnSeriesUpdatedFinish();
                            }));

                            if (beforeVals != null)
                            {
                                correlation = Helper.MathHelper.Corrcoef(beforeVals.ToArray(), afterVals.ToArray());
                            }
                            CompareModel.CanCompare = (CompareModel.BackgroundBeforePoints.Count > 0 && CompareModel.BackgroundAfterPoints.Count > 0);
                            CompareModel.Notify("CanCompare");
                        });
                }
                else
                {
                    ShowConfirmWindow(Tips.CorrelationNoDataError, string.Empty);
                }
                serial.StringDataReceivedHandler = null;
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.Output + " 12",  1000);
        }

        private void ToGetBeforePoints()
        {
            serial.StringDataReceivedHandler = msg =>
            {
                CompareModel.BackgroundBeforePoints.Clear();
                var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");

                for (int index = 0; index < collection.Count - 1; index++)
                {
                    CompareModel.BackgroundBeforePoints.Add(new ObservablePoint(double.Parse(collection[index].Value), index * SampleRate));
                }
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.Output + " 12");
        }

        private void ToGetCorrelation()
        {
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Error))
                {
                    mutex.Set();
                    ShowErrorWindow(ErrorString.Error + ":" + msg);
                    return;
                }
                string[] param = msg.Split(' ');
                if (param.Length > 6)
                {
                    try
                    {
                        standCorrelation = double.Parse(param[2]) + double.Parse(param[5]);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (correlation < standCorrelation)
                {
                    ShowConfirmWindow(Tips.CorrelationLow + "[" + correlation.ToString("F2") + "<" + standCorrelation + "]", string.Empty);
                }
                else
                {
                    ShowConfirmWindow(Tips.CorrelationHigh + "[" + correlation.ToString("F2") + ">" + standCorrelation + "]", string.Empty);
                }
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.ReadCLI + " setThresholdParas");
        }

        private void ToStudy()
        {
            serial.EndStr = "\n";
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
            {
                if (!_progressCtrl.IsVisible)
                {
                    _progressViewModel.Message = Tips.Studying;
                    await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
                }
            }));
            serial.StringDataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.StudyEnd) || msg.Contains("end"))
                {
                    _progressViewModel.Message = string.Empty;
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
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
                    //serial.CompareEndString = false;
                    _progressViewModel.IsIndeterminate = true;
                    _progressViewModel.MaxValue = 100;
                    _progressViewModel.Value = 0;
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
                    {
                        if (!_progressCtrl.IsVisible && _progressViewModel.Message != Tips.Studying)
                        {
                            _progressViewModel.Message = Tips.Studying;
                            await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
                        }
                    }));
                }
                else
                {
                    _progressViewModel.Message = Tips.Studying + ":    " + msg.Trim('\n', '\r');
                }
            };
            serial.WriteLine(SerialRadarCommands.Output + " 4", 5000);
            ExtraOnceWorkToDo = async () => {
                await AsyncWork(() => {
                    serial.WriteLine(SerialRadarCommands.SoftReset, 300, false);
                    serial.StringDataReceivedHandler = null;
                    mutex.Set();
                });
            };
        }
        private void ToReboot()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
            {
                if (!_progressCtrl.IsVisible)
                {
                    _progressViewModel.Message = Tips.Rebooting;
                    await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
                }
            }));
            serial.CompareEndString = false;
            serial.StringDataReceivedHandler = msg =>
            {
                if(msg.Contains(SerialRadarReply.Start))
                {
                    mutex.Set();
                    ShowConfirmWindow(Tips.RebootSuccess, string.Empty);
                }
                else if(msg.Contains(SerialRadarReply.Error))
                {
                    mutex.Set();
                    ShowConfirmWindow(Tips.RebootFail, string.Empty);
                }
            };
            serial.WriteLine(SerialRadarCommands.SoftReset, 300, false);
        }

        private void ToReset()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
            {
                _progressViewModel.IsIndeterminate = true;
                _progressViewModel.Message = Tips.Reseting;
                await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
            }));
            bool toResetBaud = (ConnectModel.CustomRate != uint.Parse(BauRate.Rate115200));
            string lastOperation = SerialRadarCommands.Output;
            serial.StringDataReceivedHandler = async msg =>
            {
                if (msg.Contains(SerialRadarReply.Start))
                {
                    if (lastOperation == SerialRadarCommands.Output)
                    {
                        serial.EndStr = SerialRadarReply.Done;
                        lastOperation = SerialRadarCommands.ReadCLI;
                        await TaskEx.Delay(100);
                        serial.WriteLine(SerialRadarCommands.ReadCLI + " " + SerialArguments.FilterParam);
                    }
                }
                else if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.Output)
                    {
                        serial.EndStr = SerialRadarReply.Done;
                        lastOperation = SerialRadarCommands.ReadCLI;
                        await TaskEx.Delay(100);
                        serial.WriteLine(SerialRadarCommands.ReadCLI + " " + SerialArguments.FilterParam);
                    }
                    else if (lastOperation == SerialRadarCommands.ReadCLI)
                    {
                        var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");
                        if (collection.Count > 10)
                        {
                            try
                            {
                                float _lrange = float.Parse(collection[2].Value) / 10;
                                float _rrange = float.Parse(collection[6].Value) / 10;
                                float _mindistance = float.Parse(collection[3].Value) / 10;
                                float _maxdistance = float.Parse(collection[5].Value) / 10;
                                LRange = _lrange.ToString("F1");
                                MinDistance = _mindistance.ToString("F1");
                                MaxDistance = _maxdistance.ToString("F1");
                                RRange = _rrange.ToString("F1");
                                Gate = control.GateType.GetType(collection[8].Value);
                                RootModel.Threshold = (collection[9].Value);
                                Threshold = control.ThresholdType.GetType(collection[9].Value);
                                Record = control.RecordKind.GetType(collection[10].Value);
                                OnPropertyChanged("LRange");
                                OnPropertyChanged("MinDistance");
                                OnPropertyChanged("MaxDistance");
                                OnPropertyChanged("RRange");
                                OnPropertyChanged("Gate");
                                OnPropertyChanged("Threshold");
                                OnPropertyChanged("Record");
                                RootModel.Notify("Threshold");

                                RangePoints.Clear();
                                RangePoints.AddRange(new ObservablePoint[] { new ObservablePoint(-_lrange, _mindistance), new ObservablePoint(_rrange, _mindistance), new ObservablePoint(_rrange, _maxdistance), new ObservablePoint(-_lrange, _maxdistance), new ObservablePoint(-_lrange, _mindistance) });

                                Thread.Sleep(300);
                                if (DelayVisible)
                                {
                                    lastOperation = SerialArguments.DelayTimeParam;
                                    serial.WriteLine(SerialRadarCommands.ReadCLI + " " + SerialArguments.DelayTimeParam);
                                }
                                if (IsTriggerRadarType)
                                {
                                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
                                    {
                                        await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                                        ShowConfirmWindow(Tips.ResetSuccess, string.Empty);
                                    }));
                                }
                                return;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
                        {
                            await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                            ShowErrorWindow(Tips.ResetFail);
                        }));
                        mutex.Set();
                    }
                    else if (lastOperation == SerialArguments.DelayTimeParam)
                    {
                        RootModel.Delay = System.Text.RegularExpressions.Regex.Match(msg, @"\d+").Value;
                        RootModel.Notify("Delay");
                        await TaskEx.Delay(100);
                        if (IsFencePositionTypeVisible)
                        {
                            if (Gate == control.GateType.Straight)
                            {
                                IsLeftRangeEditable = true;
                                IsRightRangeEditable = true;
                                OnPropertyChanged("IsLeftRangeEditable");
                                OnPropertyChanged("IsRightRangeEditable");
                            }
                            else
                            {
                                lastOperation = SerialArguments.RodDirection;
                                serial.WriteLine(SerialRadarCommands.ReadCLI + " " + SerialArguments.RodDirection);
                                return;
                            }
                        }

                        if (toResetBaud)
                        {
                            lastOperation = SerialRadarCommands.WriteBaudRate;
                            serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialRadarCommands.WriteBaudRate + " " + ConnectModel.CustomRate);
                        }
                        else
                        {
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
                            {
                                await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                                ShowConfirmWindow(Tips.ResetSuccess, string.Empty);
                            }));
                            mutex.Set();
                            serial.close();
                        }
                    }
                    else if (lastOperation == SerialArguments.RodDirection)
                    {
                        string position = System.Text.RegularExpressions.Regex.Match(msg, @"\d").Value;
                        Gate = control.GateType.GetTypeByPosition(Gate, position);
                        FencePosition = control.FencePositionType.GetType(position);
                        OnPropertyChanged("Gate");
                        if (toResetBaud)
                        {
                            lastOperation = SerialRadarCommands.WriteBaudRate;
                            serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialRadarCommands.WriteBaudRate + " " + ConnectModel.CustomRate);
                        }
                        else
                        {
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
                            {
                                await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                                ShowConfirmWindow(Tips.ResetSuccess, string.Empty);
                            }));
                            mutex.Set();
                            serial.close();
                        }
                    }
                    else if (lastOperation == SerialRadarCommands.WriteBaudRate)
                    {
                        lastOperation = string.Empty;
                        await TaskEx.Delay(100);
                        serial.WriteLine(SerialRadarCommands.SoftReset);
                        mutex.Set();
                        serial.close();
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
                        {
                            await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                            ShowConfirmWindow(Tips.ResetSuccess, string.Empty);
                        }));
                    }
                }
                else
                {
                    if (lastOperation == SerialRadarCommands.Output)
                    {
                        serial.Rate = int.Parse(BauRate.Rate115200);
                        serial.EndStr = SerialRadarReply.Start;
                        serial.CompareEndString = true;
                    }
                }

            };
            if (toResetBaud)
            {
                serial.CompareEndString = false;
            }
            else
            {
                serial.EndStr = SerialRadarReply.Start;
            }
            serial.WriteLine(SerialRadarCommands.Output + " 13", 3000);
        }

        private void ToGet()
        {
            string lastOperation = SerialRadarCommands.ReadCLI;
            serial.StringDataReceivedHandler = async msg =>
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
                        msg = msg.Substring(msg.IndexOf(SerialArguments.FilterParam));
                        var collection = System.Text.RegularExpressions.Regex.Matches(msg, @"-?\d+(\.\d+)?");
                        if (collection.Count > 10)
                        {
                            try
                            {
                                float _lrange = float.Parse(collection[2].Value)/10;
                                float _rrange = float.Parse(collection[6].Value)/10;
                                float _minY = float.Parse(collection[3].Value) / 10;
                                float _maxY = float.Parse(collection[5].Value) / 10;
                                LRange = _lrange.ToString("F1");
                                MinDistance = _minY.ToString("F1");
                                MaxDistance = _maxY.ToString("F1");
                                RRange = _rrange.ToString("F1");
                                RangePoints.Clear();
                                RangePoints.AddRange(new ObservablePoint[]{ new ObservablePoint(-_lrange, _minY), new ObservablePoint(_rrange, _minY), new ObservablePoint(_rrange, _maxY), new ObservablePoint(-_lrange, _maxY), new ObservablePoint(-_lrange, _minY)});
                                Gate = control.GateType.GetType(collection[8].Value);
                                RootModel.Threshold = collection[9].Value;
                                Threshold = control.ThresholdType.GetType(collection[9].Value);
                                Record = control.RecordKind.GetType(collection[10].Value);
                                OnPropertyChanged("LRange");
                                OnPropertyChanged("MinDistance");
                                OnPropertyChanged("MaxDistance");
                                OnPropertyChanged("RRange");
                                OnPropertyChanged("Threshold");
                                OnPropertyChanged("Record");
                                RootModel.Notify("Threshold");
                                mutex.Set();
                                Thread.Sleep(100);
                                if (DelayVisible)
                                {
                                    await AsyncWork(() => ToGetDelayAndFencePosition(Tips.GetSuccess));
                                }
                                if (IsTriggerRadarType)
                                {
                                    OnPropertyChanged("Gate");
                                    ShowConfirmWindow(Tips.GetSuccess, string.Empty);
                                }
                                return;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        ShowErrorWindow(Tips.GetFail);
                        mutex.Set();
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
            float _mindistance = 0.0f, _maxdistance = 0.0f, _lrange = 0.0f, _rrange = 0.0f;
            try
            {
                _maxdistance = float.Parse(MaxDistance);
                if (_maxdistance > 6.0 || _maxdistance < 1.0)
                    throw new Exception();
            }
            catch
            {
                ShowErrorWindow(ErrorString.DistacneMaxError);
                mutex.Set();
                return;
            }
            try
            {
                _mindistance = float.Parse(MinDistance);
                if (_mindistance > 1.0 || _mindistance < 0.2)
                    throw new Exception();
            }
            catch (Exception)
            {
                ShowErrorWindow(ErrorString.DistacneMinError);
                mutex.Set();
                return;
            }
            try
            {
                _lrange = float.Parse(LRange);
                _rrange = float.Parse(RRange);
                string error = string.Empty;
                if (Gate == control.GateType.Straight)
                {
                    if (_lrange < 0.09999 || _rrange < 0.09999 || _lrange > 1.50001 || _rrange > 1.50001)
                    {
                        error = ErrorString.RangeError1;
                        throw new Exception(error);
                    }
                }
                else
                {
                    if (_lrange < 0.69999 || _rrange < 0.69999 || _lrange > 1.50001 || _rrange > 1.50001)
                    {
                        error = ErrorString.RangeError2;
                        throw new Exception(error);
                    }
                }
            }
            catch (Exception e)
            {
                ShowErrorWindow(e.Message);
                mutex.Set();
                return;
            }
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
            {
                if (!_progressCtrl.IsVisible)
                {
                    _progressViewModel.Message = Tips.Configing;
                    await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);
                }
            }));
            string lastOperation = SerialRadarCommands.SensorStop;
            serial.StringDataReceivedHandler = async msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        lastOperation = SerialRadarCommands.WriteCLI;
                        await TaskEx.Delay(300);
                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.FilterParam + " 0 0 " + (_lrange * 10).ToString("F1") + " " + (_mindistance * 10).ToString("F1") + " 2 " + (_maxdistance * 10).ToString("F1") + " " + (_rrange * 10).ToString("F1") + " 32 " + control.GateType.GetValue(Gate) + " " + control.ThresholdType.GetValue(Threshold) + " " + control.RecordKind.GetValue(Record));
                    }
                    else if (lastOperation == SerialRadarCommands.WriteCLI)
                    {
                        RangePoints.Clear();
                        RangePoints.AddRange(new ObservablePoint[] { new ObservablePoint(-_lrange, _mindistance), new ObservablePoint(_rrange, _mindistance), new ObservablePoint(_rrange, _maxdistance), new ObservablePoint(-_lrange, _maxdistance), new ObservablePoint(-_lrange, _mindistance) });
                        OnPropertyChanged("Record");
                        mutex.Set();
                        if (DelayVisible)
                        {
                            finished = true;
                            await AsyncWork(() => ToSetDelayAndFencePosition(), true, 15000);
                        }
                        else
                        {
                            serial.EndStr = SerialRadarReply.Start;
                            serial.WriteLine(SerialRadarCommands.SoftReset, 300, false);
                            if (IsTriggerRadarType)
                                ShowConfirmWindow(Tips.ConfigSuccess, string.Empty);
                        }
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    ShowErrorWindow(Tips.ConfigFail);
                    serial.StringDataReceivedHandler = null;
                    mutex.Set();
                }
                else if (msg.Contains(SerialRadarReply.Start))
                {
                    ShowConfirmWindow(Tips.ConfigSuccess, string.Empty);
                }
            };
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }

        private void ToGetVer(Action proceedToDo = null)
        {
            string lastoperation = string.Empty;
            serial.StringDataReceivedHandler = msg =>
            {
                if (lastoperation == SerialRadarCommands.Version)
                {
                    if (msg.Contains("ITS"))
                    {
                        int startIndex = msg.IndexOf("ITS");
                        int endIndex = msg.IndexOf("Done");
                        Version = msg.Substring(startIndex, endIndex - startIndex).Trim('\r', '\n');
                        GateTypes = control.GateType.getAllTypesWithoutFence();
                        IsLeftRangeEditable = true;
                        IsRightRangeEditable = true;
                        if (compareVersion2("1.1.1"))
                        {
                            DelayVisible = true;
                            if (compareVersion2("1.2.7"))
                            {
                                NewVersion = true;
                                if (compareVersion2("1.2.9"))
                                {
                                    IsFencePositionTypeVisible = true;
                                    GateTypes = control.GateType.GetAllTypes();
                                    serial.ToTranslate = compareVersion2("1.3.0");
                                }
                                else 
                                {
                                    IsFencePositionTypeVisible = false;
                                }
                            }
                            else
                            {
                                NewVersion = false;
                            }
                        }
                        else
                        {
                            DelayVisible = false;
                        }
                        OnPropertyChanged("Version");
                        OnPropertyChanged("DelayVisible");
                        OnPropertyChanged("IsFencePositionTypeVisible");
                        OnPropertyChanged("GateTypes");
                        OnPropertyChanged("IsLeftRangeEditable");
                        OnPropertyChanged("IsRightRangeEditable");
                        mutex.Set();
                        if (proceedToDo != null)
                            proceedToDo();
                    }
                }
            };
            //Thread.Sleep(1000);
            lastoperation = SerialRadarCommands.Version;
            serial.WriteLine(SerialRadarCommands.Version);
        }

        private async void ToResetBaudRate()
        {
            serial.Rate = int.Parse(BauRate.Rate115200);
            string lastOperation = SerialRadarCommands.SensorStop;
            serial.StringDataReceivedHandler = async msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        await TaskEx.Delay(100);
                        lastOperation = SerialRadarCommands.WriteBaudRate;
                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialRadarCommands.WriteBaudRate + " " + ConnectModel.CustomRate);
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
                        await AsyncWork(() => ToGetVer());
                    }
                }
            };
            await TaskEx.Delay(200);
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }

        private string GetSerialPortName(string name)
        {
            int pos = name.IndexOf("  ");
            if (pos > 0)
                return name.Substring(0, name.IndexOf("  "));
            return name;
        }

        FileIOManager reader = null;
        Timer overTimer = null;
        byte[] dataTmp = null;
        const int preByteSizeAdded = 32;
        const int ignorePreByteSize = 8;
        string preUpdateRate = string.Empty;
        protected override async void ToDo()
        {
            if (UpdateModel.BinPath == "" || !UpdateModel.BinPath.Contains('\\'))
            {
                ShowErrorWindow(ErrorString.BinPathError);
                return;
            }
            await AsyncWork(() =>
            {
                string lastMessage = string.Empty;
                overTimer = new Timer(obj =>
                {
                    if (_progressViewModel.Message == Tips.Updating)
                    {
                        if (lastMessage == _progressViewModel.Value.ToString())
                        {
                            _progressViewModel.Message = ErrorString.OverTime;
                        }
                        else
                        {
                            lastMessage = _progressViewModel.Value.ToString();
                        }
                    }
                    else
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
                        string lastOperation = string.Empty;
                        reader = new FileIOManager(UpdateModel.BinPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                        if (reader.Length == -1)
                        {
                            _progressViewModel.Message = ErrorString.FileError;
                            return;
                        }
                        uint sum = 0, pos = 0;
                        if (reader.Length < ignorePreByteSize)
                        {
                            _progressViewModel.Message = ErrorString.FileError;
                            return;
                        }

                        //通用两种情况，带版本号不带版本号
                        string version = System.Text.Encoding.Default.GetString(reader.ReadBytes(ignorePreByteSize));
                        pos += ignorePreByteSize;
                        if (version.Contains("ITS"))
                        {
                            if (!compareVersion(System.Text.Encoding.Default.GetString(reader.ReadBytes(preByteSizeAdded - ignorePreByteSize))))
                            {
                                _progressViewModel.Message = ErrorString.SmallVersion;
                                reader.Close();
                                return;
                            }
                            pos += (preByteSizeAdded - ignorePreByteSize);
                            if (!NewVersion)
                            {
                                reader.ReadBytes(ignorePreByteSize);
                                pos += ignorePreByteSize;
                            }
                        }
                        else
                        {
                            if (NewVersion)
                            {
                                reader.Seek(0);
                                pos = 0;
                            }
                        }

                        serial.DecodeFrame = NewVersion;
                        bool toContinue = false;
                        bool toTranslate = serial.ToTranslate;
                        if (NewVersion)
                        {
                            overTimer.Dispose();
                            serial.StringDataReceivedHandler = async msg =>
                            {
                                if (msg.Contains(SerialRadarReply.Done))
                                {
                                    switch (lastOperation)
                                    {
                                        case SerialRadarCommands.SensorStop:
                                            //await TaskEx.Delay(100);
                                            lastOperation = SerialRadarCommands.WriteCLI;
                                            preUpdateRate = UpdateModel.CustomUpdateRate;
                                            if (ConnectModel.CustomItem.Contains("蓝牙"))
                                            {
                                                serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 9");
                                            }
                                            else
                                            {
                                                switch (UpdateModel.CustomUpdateRate)
                                                {
                                                    case BauRate.Rate57600:
                                                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 2");
                                                        break;
                                                    case BauRate.Rate38400:
                                                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 3");
                                                        break;
                                                    case BauRate.Rate19200:
                                                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 4");
                                                        break;
                                                    default:
                                                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 1");
                                                        break;
                                                }
                                            }
                                            break;
                                        case SerialRadarCommands.WriteCLI:
                                            await TaskEx.Delay(100);
                                            lastOperation = ExtraSerialRadarCommands.SoftInercludeReset;
                                            _progressViewModel.Message = Tips.WaitForOpen;
                                            serial.CompareEndString = false;
                                            serial.WriteLine(SerialRadarCommands.SoftReset, 10000);
                                            //await TaskEx.Delay(500);
                                            //lastOperation = SerialRadarCommands.FlashErase;
                                            //_progressViewModel.Message = Tips.Flashing;
                                            //serial.Rate = int.Parse(CustomUpdateRate);
                                            //serial.Write(new byte[] { 0x01, 0xCD });
                                            break;
                                        case ExtraSerialRadarCommands.SoftInercludeReset:
                                            lastOperation = SerialRadarCommands.Test;
                                            serial.Type = SerialReceiveType.Bytes;
                                            serial.Rate = int.Parse(UpdateModel.CustomUpdateRate);
                                            serial.ToTranslate = false;
                                            serial.Write(HexProtocol.Code(HexProtocol.ADDRESS_TARGET_0, HexProtocol.SUCCESS, HexProtocol.FUNCTION_UPDATE_TEST, new byte[] { }));
                                            break;
                                    }
                                }
                                else
                                {
                                    if (msg.Contains(SerialRadarReply.Error))
                                    {
                                        if (lastOperation == SerialRadarCommands.SensorStop)
                                        {
                                            lastOperation = ExtraSerialRadarCommands.SoftInercludeReset;
                                            serial.Rate = int.Parse(preUpdateRate);
                                            serial.BytesDecodedDataReceivedHandler(new Tuple<byte, byte, ushort, byte[]>(HexProtocol.ADDRESS_TARGET_0, HexProtocol.SUCCESS, HexProtocol.FUNCTION_UPDATE_TEST, new byte[] { }));
                                            return;
                                        }

                                        _progressViewModel.Message = ErrorString.Error + msg.Trim();
                                        if (reader != null)
                                            reader.Close();
                                        if (serial != null)
                                        {
                                            serial.CompareEndString = true;
                                            serial.Rate = (int)ConnectModel.CustomRate;
                                            mutex.Set();
                                        }
                                    }
                                    else
                                    {
                                        if (lastOperation == ExtraSerialRadarCommands.SoftInercludeReset)
                                        {
                                            if (msg.Contains("init succses")) 
                                            {
                                                lastOperation = SerialRadarCommands.Test;
                                                serial.Type = SerialReceiveType.Bytes;
                                                serial.Rate = int.Parse(UpdateModel.CustomUpdateRate);
                                                await TaskEx.Delay(10000);
                                                serial.Write(HexProtocol.Code(HexProtocol.ADDRESS_TARGET_0, HexProtocol.SUCCESS, HexProtocol.FUNCTION_UPDATE_TEST, new byte[] { }));
                                            }
                                            if (msg.Length > 1)  /*波特率低时可能返回乱码，以字节数量作为判断依据下一步命令发送的依据，避免过早发送命令导致命令不起作用，导致重发*/
                                            {
                                                lastOperation = SerialRadarCommands.Test;
                                                serial.Type = SerialReceiveType.Bytes;
                                                serial.Rate = int.Parse(UpdateModel.CustomUpdateRate);
                                                serial.Write(HexProtocol.Code(HexProtocol.ADDRESS_TARGET_0, HexProtocol.SUCCESS, HexProtocol.FUNCTION_UPDATE_TEST, new byte[] { }));
                                            }
                                        }
                                    }
                                }
                            };
                            serial.ErrorReceivedHandler = data =>
                            {
                                if (data == CommHexProtocolDecoder.ERROR_COMMON_OVERTIME)
                                {
                                    _progressViewModel.Message = ErrorString.Error + ErrorString.OverTime;
                                }
                                else
                                    _progressViewModel.Message = ErrorString.Error + lastOperation + "   0x" + BitConverter.ToString(new byte[] { data });
                                if (reader != null)
                                    reader.Close();
                                if (serial != null)
                                {
                                    serial.CompareEndString = true;
                                    serial.Rate = (int)ConnectModel.CustomRate;
                                    mutex.Set();
                                }
                            };
                            serial.BytesDecodedDataReceivedHandler = async decodedDataTuple =>
                            {
                                switch (lastOperation)
                                {
                                    case ExtraSerialRadarCommands.SoftInercludeReset:
                                        await TaskEx.Delay(100);
                                        lastOperation = SerialRadarCommands.Test;
                                        serial.Type = SerialReceiveType.Bytes;
                                        serial.Write(HexProtocol.Code(HexProtocol.ADDRESS_TARGET_0, HexProtocol.SUCCESS, HexProtocol.FUNCTION_UPDATE_TEST, new byte[] { }));
                                        break;
                                    case SerialRadarCommands.Test:
                                        lastOperation = SerialRadarCommands.FlashErase;
                                        serial.Write(HexProtocol.Code(HexProtocol.ADDRESS_TARGET_0, HexProtocol.SUCCESS, HexProtocol.FUNCTION_UPDATE_ERASE, new byte[] { }), 8000);
                                        break;
                                    case SerialRadarCommands.FlashErase:
                                        //await TaskEx.Delay(500);
                                        lastOperation = SerialRadarCommands.BootLoader;
                                        _progressViewModel.Message = Tips.Updating;
                                        int times = (int)((reader.Length - pos) / ReadBytesNumber + ((reader.Length - pos) % ReadBytesNumber == 0 ? 0 : 1));
                                        byte[] tmp = BitConverter.GetBytes(times);
                                        _progressViewModel.MaxValue = (uint)times;
                                        _progressViewModel.Value = 0;
                                        serial.Write(HexProtocol.Code(HexProtocol.ADDRESS_TARGET_0, HexProtocol.SUCCESS, HexProtocol.FUNCTION_UPDATE_SIZE, tmp));
                                        break;
                                    case SerialRadarCommands.BootLoader:
                                    case SerialRadarCommands.T:
                                        //await TaskEx.Delay(1);
                                        if (lastOperation == SerialRadarCommands.BootLoader)
                                        {
                                            toContinue = true;
                                            _progressViewModel.Message = Tips.Updating;
                                            _progressViewModel.IsIndeterminate = false;
                                        }
                                        if (toContinue)
                                        {
                                            lastOperation = SerialRadarCommands.T;
                                            if (pos >= reader.Length)
                                            {
                                                await TaskEx.Delay(100);
                                                lastOperation = SerialRadarCommands.CRC;
                                                serial.Write(HexProtocol.Code(HexProtocol.ADDRESS_TARGET_0, HexProtocol.SUCCESS, HexProtocol.FUNCTION_UPDATE_REFRESH, new byte[] { }), 30000);
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    byte[] dataTmp = reader.ReadBytes(ReadBytesNumber);
                                                    Array.ForEach<byte>(dataTmp, b => { sum += b; });
                                                    byte[] msgData = new byte[dataTmp.Length + 4];
                                                    Array.Copy(BitConverter.GetBytes(_progressViewModel.Value), msgData, 4);
                                                    Array.Copy(dataTmp, 0, msgData, 4, dataTmp.Length);
                                                    serial.Write(HexProtocol.Code(HexProtocol.ADDRESS_TARGET_0, HexProtocol.SUCCESS, HexProtocol.FUNCTION_UPDATE_POSITION_DATA, msgData));
                                                }
                                                catch (Exception)
                                                {
                                                }
                                                pos += ReadBytesNumber;
                                                _progressViewModel.Value += 1;
                                            }
                                        }
                                        break;
                                    case SerialRadarCommands.CRC:
                                        await TaskEx.Delay(100);
                                        lastOperation = SerialRadarCommands.SoftReset;
                                        //lastOperation = string.Empty;
                                        _progressViewModel.Value = _progressViewModel.MaxValue;
                                        serial.Write(HexProtocol.Code(HexProtocol.ADDRESS_TARGET_0, HexProtocol.SUCCESS, HexProtocol.FUNCTION_UPDATE_RESTART, new byte[] { }));
                                        reader.Close();
                                        //_progressViewModel.Message = Tips.Updated;
                                        //if (serial != null)
                                        //{
                                        //    serial.CompareEndString = true;
                                        //    serial.Rate = (int)ConfigModel.CustomRate;
                                        //}
                                        //mutex.Set();
                                        //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                        //{
                                        //    _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                                        //}));
                                        //await TaskEx.Delay(1000);
                                        //ShowConfirmWindow(Tips.Updated, string.Empty);
                                        //SerialWork(() => ToGetVer());
                                        break;
                                    case SerialRadarCommands.SoftReset:
                                        lastOperation = string.Empty;
                                        _progressViewModel.Message = Tips.Updated;
                                        if (serial != null)
                                        {
                                            serial.CompareEndString = true;
                                            serial.Rate = (int)ConnectModel.CustomRate;
                                            serial.ToTranslate = toTranslate;
                                        }
                                        mutex.Set();
                                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                                            _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                                        }));
                                        await TaskEx.Delay(2000);
                                        ShowConfirmWindow(Tips.Updated, string.Empty);
                                        await AsyncWork(() => ToGetVer());
                                        break;
                                }
                            };
                        }
                        else
                        {
                            serial.CompareEndBytesCount = 2;
                            serial.StringDataReceivedHandler = async msg =>
                            {
                                if (msg.Contains(SerialRadarReply.Done))
                                {
                                    switch (lastOperation)
                                    {
                                        case SerialRadarCommands.SensorStop:
                                            await TaskEx.Delay(1000);
                                            lastOperation = SerialRadarCommands.WriteCLI;
                                            serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 1", 300, false);
                                            break;
                                        case SerialRadarCommands.WriteCLI:
                                            await TaskEx.Delay(500);
                                            //lastOperation = ExtraSerialRadarCommands.SoftInercludeReset;
                                            _progressViewModel.Message = Tips.WaitForOpen;
                                            serial.CompareEndString = false;
                                            serial.Type = SerialReceiveType.Bytes;
                                            serial.WriteLine(SerialRadarCommands.SoftReset, 300, false);
                                            await TaskEx.Delay(500);
                                            lastOperation = SerialRadarCommands.FlashErase;
                                            _progressViewModel.Message = Tips.Flashing;
                                            serial.Rate = int.Parse(BauRate.Rate115200);
                                            serial.Write(new byte[] { 0x01, 0xCD }, 300, false);
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
                                            serial.Rate = (int)ConnectModel.CustomRate;
                                            mutex.Set();
                                        }
                                        overTimer.Dispose();
                                    }
                                    else if (msg.Contains(SerialRadarReply.NotRecognized))
                                    {
                                        switch (lastOperation)
                                        {
                                            case SerialRadarCommands.SensorStop:
                                                serial.WriteLine(SerialRadarCommands.SensorStop, 300, false);
                                                break;
                                            case SerialRadarCommands.WriteCLI:
                                                serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 1", 300, false);
                                                break;
                                            case ExtraSerialRadarCommands.SoftInercludeReset:
                                                serial.WriteLine(SerialRadarCommands.SoftReset, 300, false);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        if (lastOperation == ExtraSerialRadarCommands.SoftInercludeReset)
                                        {
                                            lastOperation = SerialRadarCommands.FlashErase;
                                            serial.Rate = int.Parse(BauRate.Rate115200);
                                            serial.Write(new byte[] { 0x01, 0xCD }, 300, false);
                                        }
                                    }
                                }
                            };
                            serial.BytesFrameDataReceivedHandler = async data =>
                            {
                                if (data.Length > 1)
                                {
                                    if (data[0] == 0xEE && data[1] == 0xCD)
                                    {
                                        switch (lastOperation)
                                        {
                                            case ExtraSerialRadarCommands.SoftInercludeReset:
                                                serial.WriteLine(SerialRadarCommands.SoftReset, 300, false);
                                                return;
                                            case SerialRadarCommands.FlashErase:
                                                serial.Write(new byte[] { 0x01, 0xCD }, 300, false);
                                                return;
                                            case SerialRadarCommands.BootLoader:
                                            case SerialRadarCommands.T:
                                                serial.Write(dataTmp, 300, false);
                                                return;
                                            case SerialRadarCommands.CRC:
                                                byte[] tmp1 = BitConverter.GetBytes(sum);
                                                serial.Write(new byte[] { 0x04, 0xCD, tmp1[0], tmp1[1], tmp1[2], tmp1[3] }, 300, false);
                                                return;
                                            case SerialRadarCommands.SoftReset:
                                                serial.Write(new byte[] { 0x05, 0xCD }, 300, false);
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
                                                serial.Write(new byte[] { 0x01, 0xCD }, 300, false);
                                                break;
                                            case SerialRadarCommands.FlashErase:
                                                if (data[data.Length - 2] == 0x11 && data[data.Length - 1] == 0xCD)
                                                {
                                                    await TaskEx.Delay(500);
                                                    lastOperation = SerialRadarCommands.BootLoader;
                                                    _progressViewModel.Message = Tips.Updating;
                                                    int times = (int)((reader.Length - pos) / ReadBytesNumber + ((reader.Length - pos) % ReadBytesNumber == 0 ? 0 : 1));
                                                    byte[] tmp = BitConverter.GetBytes(times);
                                                    _progressViewModel.MaxValue = (uint)times;
                                                    _progressViewModel.Value = 0;
                                                    serial.Write(new byte[] { 0x02, 0xCD, tmp[0], tmp[1], tmp[2], tmp[3] }, 300, false);
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
                                                    if (pos >= reader.Length)
                                                    {
                                                        if (data[data.Length - 2] == 0x23 && data[data.Length - 1] == 0xCD)
                                                        {
                                                            await TaskEx.Delay(100);
                                                            lastOperation = SerialRadarCommands.CRC;
                                                            _progressViewModel.Message = Tips.CRCing;
                                                            byte[] tmp1 = BitConverter.GetBytes(sum);
                                                            serial.Write(new byte[] { 0x04, 0xCD, tmp1[0], tmp1[1], tmp1[2], tmp1[3] }, 300, false);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            dataTmp = reader.ReadBytes(ReadBytesNumber);
                                                            Array.ForEach<byte>(dataTmp, b => { sum += b; });
                                                            serial.Write(dataTmp, 300, false);
                                                        }
                                                        catch (Exception)
                                                        {
                                                        }
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
                                                serial.Write(new byte[] { 0x05, 0xCD }, 300, false);
                                                reader.Close();
                                                break;
                                            case SerialRadarCommands.SoftReset:
                                                _progressViewModel.Message = Tips.Updated;
                                                if (serial != null)
                                                {
                                                    serial.CompareEndString = true;
                                                    serial.Rate = (int)ConnectModel.CustomRate;
                                                }
                                                mutex.Set();
                                                overTimer.Dispose();
                                                await AsyncWork(() => ToResetBaudRate());
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
                                        serial.Rate = (int)ConnectModel.CustomRate;
                                        mutex.Set();
                                    }
                                    overTimer.Dispose();
                                }
                            };
                        }
                        lastOperation = SerialRadarCommands.SensorStop;
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
                }
                catch
                {
                    return true;
                }
            }
            catch
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
            Version = null;
            if (serial != null)
            {
                serial.close();
            }
        }

        public bool IsHoldRadarType { get => ConnectedRadarType == RadarType.Hold; }
        public bool IsTriggerRadarType { get => ConnectedRadarType == RadarType.Trigger; }
        private void JudgeRadarType()
        {
            if (ConnectModel.SelectedRadarType == Application.Current.Resources["RadarHoldType"].ToString())
            {
                ConnectedRadarType = RadarType.Hold;
                OnPropertyChanged("IsHoldRadarType");
                mutex.Set();
            }
            else
            {
                int delay = 0;
                string lastOperation = SerialRadarCommands.SensorStop;
                serial.StringDataReceivedHandler = async msg =>
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

        internal async void ToUpdate()
        {
            await _dialogCoordinator.ShowMetroDialogAsync(this, _updateView);
        }

        internal async void ToConnect()
        {
            if (ConnectState == ConnectType.Connected)
            {
                //已连接
                this.Dispose();
                ConnectState = ConnectType.Disconnected;
            }
            else
            {
                await _dialogCoordinator.ShowMetroDialogAsync(this, _connectView);
            }
        }

        internal async void ToRecord()
        {
            await _dialogCoordinator.ShowMetroDialogAsync(this, _recordView);
        }

        internal async void ToCompare()
        {
            await _dialogCoordinator.ShowMetroDialogAsync(this, _compareView);
        }

        internal async void ToRoot()
        {
            if (!Developer && !Root)
                await _dialogCoordinator.ShowMetroDialogAsync(this, _pwdView);
            if (Developer)
                await _dialogCoordinator.ShowMetroDialogAsync(this, _developView);
            if (Root)
                await _dialogCoordinator.ShowMetroDialogAsync(this, _rootView);
        }
    }

    public class WifiMainViewModel : MainViewModel
    {
        public UpdateModel ConfigModel { get; set; }
        public WifiMainViewModel(MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator) : base(dialogCoordinator)
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
