using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace MbitGate
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        bool _messagegoxFlag = true;
        public App()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_messagegoxFlag)
            {
                _messagegoxFlag = false;
                MessageBox.Show(e.Exception.Message, "异常", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }

            //MessageBox.Show("我们很抱歉，当前应用程序遇到一些问题，该操作已经终止，请进行重试，如果问题继续存在，请联系管理员.", "意外的操作", MessageBoxButton.OK, MessageBoxImage.Information);//这里通常需要给用户一些较为友好的提示，并且后续可能的操作
            e.Handled = true;//使用这一行代码告诉运行时，该异常被处理了，不再作为UnhandledException抛出了。
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (_messagegoxFlag)
            {
                _messagegoxFlag = false;
                MessageBox.Show(e.ExceptionObject.ToString(), "异常", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }

        }
    }
}
