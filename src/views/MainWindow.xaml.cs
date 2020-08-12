﻿using MbitGate.model;
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
            ic_tab_config.IsChecked = true;
            this.AddHandler(MahApps.Metro.Controls.Dialogs.CustomDialog.MouseMoveEvent, new MouseEventHandler(DialogCoordinatorWindow_Drag));
            this.Title = Application.Current.Resources["MainWindowTitle"].ToString() + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        }

        private void Choose_Click(object sender, RoutedEventArgs e)
        {
            ATFileDialog dialog = new ATFileDialog(Application.Current.Resources["BinChoose"].ToString(), Application.Current.Resources["BinType"].ToString(), false, "");
            if (dialog.ShowDialog() == true)
            {
                PathText.Text = model.VMATFileDialogModel.Instance.SelectedFileItem.Path;
            }
        }

        private void DialogCoordinatorWindow_Drag(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if(e.OriginalSource is MahApps.Metro.Controls.MetroThumbContentControl)
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
            mainVModel?.start();
        }

        private void Develop_Choose_Click(object sender, RoutedEventArgs e)
        {
            ic_tab_config.IsChecked = false;
            ic_tab_update.IsChecked = false;
            ic_tab_develop.IsChecked = true;
            ic_tab_search.IsChecked = false;
            ic_tab_alarm.IsChecked = false;
            ic_tab_comparison.IsChecked = false;
            mainVModel?.root();
        }

        private void Update_Choose_Click(object sender, RoutedEventArgs e)
        {
            ic_tab_config.IsChecked = false;
            ic_tab_update.IsChecked = true;
            ic_tab_develop.IsChecked = false;
            ic_tab_search.IsChecked = false;
            ic_tab_alarm.IsChecked = false;
            ic_tab_comparison.IsChecked = false;
        }

        private void Config_Choose_Click(object sender, RoutedEventArgs e)
        {
            ic_tab_config.IsChecked = true;
            ic_tab_update.IsChecked = false;
            ic_tab_develop.IsChecked = false;
            ic_tab_search.IsChecked = false;
            ic_tab_alarm.IsChecked = false;
            ic_tab_comparison.IsChecked = false;
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
            ic_tab_config.IsChecked = false;
            ic_tab_update.IsChecked = false;
            ic_tab_develop.IsChecked = false;
            ic_tab_search.IsChecked = true;
            ic_tab_alarm.IsChecked = false;
            ic_tab_comparison.IsChecked = false;
        }

        private void Alarm_Choose_Click(object sender, RoutedEventArgs e)
        {
            ic_tab_config.IsChecked = false;
            ic_tab_update.IsChecked = false;
            ic_tab_develop.IsChecked = false;
            ic_tab_search.IsChecked = false;
            ic_tab_alarm.IsChecked = true;
            ic_tab_comparison.IsChecked = false;
        }

        private void Compare_Choose_Click(object sender, RoutedEventArgs e)
        {
            ic_tab_config.IsChecked = false;
            ic_tab_update.IsChecked = false;
            ic_tab_develop.IsChecked = false;
            ic_tab_search.IsChecked = false;
            ic_tab_alarm.IsChecked = false;
            ic_tab_comparison.IsChecked = true;
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
}
