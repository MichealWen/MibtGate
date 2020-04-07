using MbitGate.control;
using MbitGate.helper;
using MbitGate.views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

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

        public string Version { get; set; }

        public string BinPath { get; set; }

        public List<string> GateTypes { get { return control.GateType.GetAllTypes(); } }
        public string Gate { get; set; }
        public List<string> ThresholdTypes { get { return control.ThresholdType.GetAllTypes(); } }
        public bool Developer { get; set; }
        public SerialMainViewModel(MahApps.Metro.Controls.Dialogs.IDialogCoordinator dialogCoordinator) : base(dialogCoordinator)
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
                }
            };
            _progressCtrl.DataViewModel = _progressViewModel;

            ConfigModel = new ConfigViewModel(
                cancel => { Dispose(); Application.Current.Shutdown(); },
                confirm => { connect(); }
                );
            RateEditable = true;
            ItemSourceName = Application.Current.Resources["Serial"].ToString();

            _settingView = new SettingView();
            _settingView.DataContext = ConfigModel;

            Developer = false;
            PasswordModel = new PwdViewModel()
            {
                CancelCommand = new SimpleCommand()
                {
                    ExecuteDelegate = arg => { _dialogCoordinator.HideMetroDialogAsync(this, _pwdView); }
                },
                CheckCommand = new SimpleCommand()
                {
                    ExecuteDelegate = arg =>
                    {
                        if (!Developer)
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
                    SerialWork(() => ToSet());
                }
            };

            GetCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => ToGet());
                }
            };

            DefaultCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => ToReset());
                }
            };

            StudyCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => toStudy());
                }
            };

            RebootCmd = new SimpleCommand()
            {
                ExecuteDelegate = param =>
                {
                    SerialWork(() => toReboot());
                }
            };
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
            serial.close();
            SerialWork(() => ToGetVer());
        }

        ManualResetEvent mutex = new ManualResetEvent(false);
        private async void SerialWork(Action towork)
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
            await Task.Factory.StartNew(towork);
            await Task.Factory.StartNew(() =>
            {
                WaitHandle.WaitAny(new WaitHandle[] { mutex });
                mutex.Reset();
                if (serial.IsOpen)
                    serial.close();
            });
        }
        private void toStudy()
        {
            serial.DataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
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
                else if (msg.Contains(SerialRadarReply.StudyEnd))
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                    {
                        _progressViewModel.Message = Tips.StudySuccess;
                        await TaskEx.Delay(1000);
                        await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                    }));
                    mutex.Set();
                }
            };
            serial.WriteLine(SerialRadarCommands.Output + " 4");
        }

        private void toReboot()
        {
            serial.DataReceivedHandler = msg =>
            {
                ShowConfirmWindow(Tips.RebootSuccess, Tips.ConfigSuccess);
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.SoftReset);
        }

        private void ToReset()
        {
            string lastOperation = SerialRadarCommands.SensorStop;
            serial.DataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        lastOperation = SerialRadarCommands.WriteCLI;
                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.FilterParam + " 0 0 5 2 2 30 5 32 0 0");
                    }
                    else if (lastOperation == SerialRadarCommands.WriteCLI)
                    {
                        LRange = "0.5";
                        Distance = "3";
                        RRange = "0.5";
                        Gate = control.GateType.Straight;
                        Threshold = control.ThresholdType.High;
                        OnPropertyChanged("LRange");
                        OnPropertyChanged("Distance");
                        OnPropertyChanged("RRange");
                        OnPropertyChanged("Gate");
                        OnPropertyChanged("Threshold");
                        serial.WriteLine(SerialRadarCommands.SoftReset);
                        ShowConfirmWindow(Tips.ManualReboot, Tips.ConfigSuccess);
                        mutex.Set();
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    ShowErrorWindow(Tips.ConfigFail);
                    mutex.Set();
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
                        LRange = (float.Parse(result[3]) / 10).ToString();
                        Distance = (float.Parse(result[6]) / 10).ToString();
                        RRange = (float.Parse(result[7]) / 10).ToString();
                        Gate = control.GateType.GetType(result[9]);
                        Threshold = control.ThresholdType.GetType(result[10]);
                        OnPropertyChanged("LRange");
                        OnPropertyChanged("Distance");
                        OnPropertyChanged("RRange");
                        OnPropertyChanged("Gate");
                        OnPropertyChanged("Threshold");
                        //serial.WriteLine(SerialRadarCommands.SoftReset);
                        //ShowSplashWindow(Tips.GetSuccess, 1000);
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
            if (Distance == string.Empty || LRange == string.Empty || RRange == string.Empty)
            {
                ShowErrorWindow(Tips.GetFail);
                mutex.Set();
                return;
            }
            float distance = float.Parse(Distance);
            float lrange = float.Parse(LRange);
            float rrange = float.Parse(RRange);
            if (distance < 0 || lrange < 0 || rrange < 0)
            {
                ShowErrorWindow(ErrorString.ParamError);
                return;
            }
            else if (distance > 6.0 || distance < 1.0)
            {
                ShowErrorWindow(ErrorString.DisntacneError);
                return;
            }
            else if (lrange < 0.5 || rrange < 0.5 || lrange > 1 || rrange > 1)
            {
                ShowErrorWindow(ErrorString.RangeError);
                return;
            }

            string lastOperation = SerialRadarCommands.SensorStop;
            serial.DataReceivedHandler = msg =>
            {
                if (msg.Contains(SerialRadarReply.Done))
                {
                    if (lastOperation == SerialRadarCommands.SensorStop)
                    {
                        lastOperation = SerialRadarCommands.WriteCLI;
                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.FilterParam + " 0 0 " + (float.Parse(LRange) * 10).ToString("F0") + " 2 2 " + (float.Parse(Distance) * 10).ToString("F0") + " " + (float.Parse(RRange) * 10).ToString("F0") + " 32 " + control.GateType.GetValue(Gate) + " " + control.ThresholdType.GetValue(Threshold));
                    }
                    else if (lastOperation == SerialRadarCommands.WriteCLI)
                    {
                        serial.WriteLine(SerialRadarCommands.SoftReset);
                        ShowConfirmWindow(Tips.ManualReboot, Tips.ConfigSuccess);
                        mutex.Set();
                    }
                }
                else if (msg.Contains(SerialRadarReply.Error))
                {
                    ShowErrorWindow(Tips.ConfigFail);
                    mutex.Set();
                }
            };
            serial.WriteLine(SerialRadarCommands.SensorStop);
        }

        private void ToGetVer()
        {
            serial.DataReceivedHandler = msg =>
            {
                Version = msg.Substring(0, msg.IndexOf("Done"));
                OnPropertyChanged("Version");
                mutex.Set();
            };
            serial.WriteLine(SerialRadarCommands.Version);
        }

        internal async void start()
        {
            await _dialogCoordinator.ShowMetroDialogAsync(this, _settingView);
        }

        internal async void root()
        {
            if (!Developer)
                await _dialogCoordinator.ShowMetroDialogAsync(this, _pwdView);
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
        protected override void ToDo()
        {
            SerialWork(() =>
            {
                string lastMessage = string.Empty;
                overTimer = new Timer(obj =>
                {
                    lastMessage = _progressViewModel.Message;
                    if (_progressViewModel.Message != Tips.Updating && lastMessage == _progressViewModel.Message)
                    {
                        _progressViewModel.Message = ErrorString.OverTime;
                    }
                }, null, 0, 5000);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                {
                    serial.CompareEndString = false;
                    _progressViewModel.Message = Tips.Initializing;
                    _progressViewModel.IsIndeterminate = true;
                    _progressViewModel.MaxValue = 100;
                    _progressViewModel.Value = 0;
                    await _dialogCoordinator.ShowMetroDialogAsync(this, _progressCtrl);

                    if (_progressCtrl.IsVisible)
                    {
                        string lastOperation = SerialRadarCommands.WriteCLI;
                        reader = new FileIOManager(BinPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                        if (reader.Length == -1)
                        {
                            _progressViewModel.Message = ErrorString.FileError;
                            return;
                        }
                        uint sum = 0, pos = 7;
                        if (reader.Length < 8)
                        {
                            _progressViewModel.Message = ErrorString.FileError;
                            return;
                        }
                        if (!compareVersion(System.Text.Encoding.Default.GetString(reader.ReadBytes(32))))
                        {
                            _progressViewModel.Message = ErrorString.SmallVersion;
                            reader.Close();
                            return;
                        }
                        reader.ReadBytes(8);

                        serial.DataReceivedHandler = msg =>
                        {
                            if (msg.Contains(SerialRadarReply.Done))
                            {
                                switch (lastOperation)
                                {
                                    case SerialRadarCommands.SensorStop:
                                        lastOperation = SerialRadarCommands.WriteCLI;
                                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 1");
                                        break;
                                    case SerialRadarCommands.WriteCLI:
                                        lastOperation = ExtraSerialRadarCommands.SoftInercludeReset;
                                        _progressViewModel.Message = Tips.WaitForOpen;
                                        serial.WriteLine(SerialRadarCommands.SoftReset);
                                        serial.Type = SerialReceiveType.Bytes;
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
                                else
                                {
                                    if (lastOperation == ExtraSerialRadarCommands.SoftInercludeReset)
                                    {
                                        lastOperation = SerialRadarCommands.FlashErase;
                                        serial.Write(new byte[] { 0x01, 0xCD });
                                    }
                                }
                            }
                        };
                        bool toContinue = false;
                        serial.BytesDataReceivedHandler = async data =>
                        {
                            if (data.Length > 0)
                            {
                                if (data[0] == 0xEE && data[1] == 0xCD)
                                {
                                    _progressViewModel.Message = ErrorString.Error + lastOperation;
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
                                else
                                {
                                    switch (lastOperation)
                                    {
                                        case ExtraSerialRadarCommands.SoftInercludeReset:
                                            lastOperation = SerialRadarCommands.FlashErase;
                                            _progressViewModel.Message = Tips.Flashing;
                                            serial.Write(new byte[] { 0x01, 0xCD });
                                            break;
                                        case SerialRadarCommands.FlashErase:
                                            lastOperation = SerialRadarCommands.BootLoader;
                                            _progressViewModel.Message = Tips.Updating;
                                            int times = (int)((reader.Length - 8) / 64 + ((reader.Length - 8) % 64 == 0 ? 0 : 1));
                                            byte[] tmp = BitConverter.GetBytes(times);
                                            _progressViewModel.MaxValue = (uint)times;
                                            _progressViewModel.Value = 0;
                                            serial.Write(new byte[] { 0x02, 0xCD, tmp[0], tmp[1], tmp[2], tmp[3] });
                                            break;
                                        case SerialRadarCommands.BootLoader:
                                        case SerialRadarCommands.T:
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
                                                if (pos >= reader.Length - 1)
                                                {
                                                    lastOperation = SerialRadarCommands.CRC;
                                                    _progressViewModel.Message = Tips.CRCing;
                                                    byte[] tmp1 = BitConverter.GetBytes(sum);
                                                    serial.Write(new byte[] { 0x04, 0xCD, tmp1[0], tmp1[1], tmp1[2], tmp1[3] });
                                                }
                                                else
                                                {
                                                    byte[] tmp2 = reader.ReadBytes(ReadBytesNumber);
                                                    Array.ForEach<byte>(tmp2, b => { sum += b; });
                                                    serial.Write(tmp2);
                                                }
                                                pos += ReadBytesNumber;
                                                _progressViewModel.Value += 1;
                                            }
                                            break;
                                        case SerialRadarCommands.CRC:
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
                                                mutex.Set();
                                            }
                                            await TaskEx.Delay(1000);
                                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () =>
                                            {
                                                if (_progressCtrl.IsVisible)
                                                {
                                                    await _dialogCoordinator.HideMetroDialogAsync(this, _progressCtrl);
                                                }
                                                overTimer.Dispose();
                                                SerialWork(() => ToGetVer());
                                            }));
                                            break;
                                    }
                                }
                            };
                        };
                        serial.WriteLine(SerialRadarCommands.WriteCLI + " " + SerialArguments.BootLoaderFlag + " 1");
                        //serial.WriteLine(SerialRadarCommands.SensorStop);
                    }
                }));
            });
        }

        private bool compareVersion(string ver)
        {
            try
            {
                if (!ver.Contains("485"))
                    return false;
                int startpos = ver.IndexOf('.') - 1;
                int endpos = ver.LastIndexOf('_') - 7;
                Version binVersion = new Version(ver.Substring(startpos, endpos - startpos));
                try
                {
                    startpos = Version.IndexOf('.') - 1;
                    endpos = Version.LastIndexOf('_') - 7;
                    Version radarVersion = new Version(Version.Substring(startpos, endpos - startpos));
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

        public ICommand GetCmd { get; set; }
        public ICommand SetCmd { get; set; }
        public ICommand DefaultCmd { get; set; }

        public ICommand StudyCmd { get; set; }

        public ICommand RebootCmd { get; set; }
        public override void Dispose()
        {
            base.Dispose();
            if (serial != null)
                serial.close();
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
