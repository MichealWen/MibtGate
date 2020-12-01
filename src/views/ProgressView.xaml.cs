using MbitGate.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MbitGate.views
{
    /// <summary>
    /// ProgressControl.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressView
    {
        public ProgressViewModel DataViewModel
        {
            get { return MainGrid.DataContext as ProgressViewModel; }
            set { MainGrid.DataContext = value; }
        }

        public ProgressView()
        {
            InitializeComponent();
        }
        
    }
}
