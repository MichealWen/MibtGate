using MbitGate.model;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MbitGate.views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private SerialMainViewModel mainVModel;
        public MainWindow()
        {
            InitializeComponent();
            mainVModel = new SerialMainViewModel(MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance);
            DataContext = mainVModel;
            this.AddHandler(MahApps.Metro.Controls.Dialogs.CustomDialog.MouseMoveEvent, new MouseEventHandler(DialogCoordinatorWindow_Drag));
#if DEBUG
            this.Title = Application.Current.Resources["MainWindowTitle"].ToString() + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion + "_" + System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location) + "_BETA";
#else
            this.Title = Application.Current.Resources["MainWindowTitle"].ToString() + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
#endif
        }

        private void DialogCoordinatorWindow_Drag(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if(e.OriginalSource is MahApps.Metro.Controls.MetroThumbContentControl || e.OriginalSource is System.Windows.Controls.Grid)
                {
                    this.DragMove();
                }
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            mainVModel.Dispose();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Develop_Choose_Click(object sender, RoutedEventArgs e)
        {
            ic_tab_connect.IsChecked = false;
            ic_tab_update.IsChecked = false;
            ic_tab_search.IsChecked = false;
            ic_tab_comparison.IsChecked = false;
            if(ic_tab_develop.IsChecked == true)
                mainVModel.ToRoot();
        }

        private void Update_Choose_Click(object sender, RoutedEventArgs e)
        {
            ic_tab_connect.IsChecked = false;
            ic_tab_update.IsChecked = true;
            ic_tab_develop.IsChecked = false;
            ic_tab_search.IsChecked = false;
            ic_tab_comparison.IsChecked = false;
            mainVModel.ToUpdate();
        }

        private void Connect_Choose_Click(object sender, RoutedEventArgs e)
        {
            ic_tab_connect.IsChecked = true;
            ic_tab_update.IsChecked = false;
            ic_tab_develop.IsChecked = false;
            ic_tab_search.IsChecked = false;
            ic_tab_comparison.IsChecked = false;
            mainVModel.ToConnect();
        }

        private void ComboxThreshold_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).IsEditable = true;
        }

        private void ComboxThreshold_GetFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).IsEditable = false;
        }

        private void Record_Choose_Click(object sender, RoutedEventArgs e)
        {
            ic_tab_connect.IsChecked = false;
            ic_tab_update.IsChecked = false;
            ic_tab_develop.IsChecked = false;
            ic_tab_search.IsChecked = true;
            ic_tab_comparison.IsChecked = false;
            mainVModel.ToRecord();
        }

        private void Compare_Choose_Click(object sender, RoutedEventArgs e)
        {
            ic_tab_connect.IsChecked = false;
            ic_tab_update.IsChecked = false;
            ic_tab_develop.IsChecked = false;
            ic_tab_search.IsChecked = false;
            ic_tab_comparison.IsChecked = true;
            mainVModel.ToCompare();
        }

        private void VersionTextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainVModel.GetVersionCommand.Execute(null);
        }

        private void LogoutImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainVModel.ReLoginCommand.Execute(null);
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            if (AnimationImage.Visibility == Visibility.Visible)
                AnimationImage.Visibility = Visibility.Collapsed;
            else
                AnimationImage.Visibility = Visibility.Visible;
        }

        private void Chart_Double_Click(object sender, MouseButtonEventArgs e)
        {
            WeakPointsSeries.DataLabels = !WeakPointsSeries.DataLabels;
            StrongestPointSeries.DataLabels = !StrongestPointSeries.DataLabels;
            X.MaxValue = 5; X.MinValue = -5;
            Y.MaxValue = 7; Y.MinValue = 0;
        }
    }

    public class SerialNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = value as string;
            if (name != null && name != "")
            {
                int startpos = name.LastIndexOf('(');
                int endpos = name.LastIndexOf(')');
                return name.Substring(startpos+1, endpos - startpos - 1);
            }
            return "";
        }
    }

    public class SearchModelVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string type = value?.ToString();
            if (type == null || type == control.RecordKind.Ignore)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CheckedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool checkedValue = (bool)value;
            if(checkedValue)
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RevertCheckedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool checkedValue = (bool)value;
            if (!checkedValue)
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RevertVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility adversary = (Visibility)value;
            if (adversary == Visibility.Visible)
            {
                return Visibility.Hidden;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiRevertCheckedVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool checkedValue1 = (bool)values[0];
            bool checkedValue2 = (bool)values[1];
            bool checkedValue3 = (bool)values[2];
            bool checkedValue4 = (bool)values[3];
            if(checkedValue1 || checkedValue2 || checkedValue3 || checkedValue4)
            {
                return Visibility.Hidden;
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AndCheckedMultiVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length > 1)
            {
                bool value1 = (bool)values[0];
                bool value2 = (bool)values[1];
                if(value1 && value2)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[]   targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OrCheckedMultiVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 1)
            {
                bool value1 = (bool)values[0];
                bool value2 = (bool)values[1];
                if (value1 || value2)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConnectState2Enable : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConnectType type = (ConnectType)value;
            if (type == ConnectType.Connected || type == ConnectType.UnAdjust || type == ConnectType.UnStudy || type == ConnectType.UnAdjustStudy)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConnectState2String : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConnectType type = (ConnectType)value;
            string state = string.Empty;
            switch(type)
            {
                case ConnectType.UnRegist:
                    state = Application.Current.Resources["UnRegist"].ToString();
                    break;
                case ConnectType.UnAdjust:
                    state = Application.Current.Resources["UnAdjust"].ToString();
                    break;
                case ConnectType.UnAdjustStudy:
                    state = Application.Current.Resources["UnAdjust"].ToString() + "|" + Application.Current.Resources["UnStudy"].ToString();
                    break;
                case ConnectType.UnStudy:
                    state = Application.Current.Resources["UnStudy"].ToString();
                    break;
                case ConnectType.Ready:
                    state = Application.Current.Resources["Ready"].ToString();
                    break;
                case ConnectType.Connecting:
                    state = Application.Current.Resources["Connecting"].ToString();
                    break;
                case ConnectType.Connected:
                    state = Application.Current.Resources["Connectied"].ToString();
                    break;
                case ConnectType.Disconnecting:
                    state = Application.Current.Resources["Disconnecting"].ToString();
                    break;
                case ConnectType.Disconnected:
                    state = Application.Current.Resources["Disconnected"].ToString();
                    break;
            }
            return state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
