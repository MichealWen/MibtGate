using MahApps.Metro.Controls;
using MbitGate.model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MbitGate.views
{
    /// <summary>
    /// Interaction logic for ATFileDialog.xaml
    /// </summary>
    public partial class ATFileDialog : MetroWindow
    {
        public ATFileDialog()
        {
            InitializeComponent();
            this.DataContext = VMATFileDialogModel.Instance;
            VMATFileDialogModel.Instance.SelectedTreeViewItem = new FileFolderInfo() { Path = Properties.Settings.Default.ChoosenFileDirectory == string.Empty ? @".\" : Properties.Settings.Default.ChoosenFileDirectory, IsDirectory = true };
        }

        public ATFileDialog(string Title, string Filters, bool IsMultiSelect, string AvoidedPaths)
        {
            InitializeComponent();
            VMATFileDialogModel.Instance.FilterList = Filters.Split('!').ToList();
            VMATFileDialogModel.Instance.SelectedFilter = VMATFileDialogModel.Instance.FilterList.FirstOrDefault();
            VMATFileDialogModel.Instance.AvoidedFilePaths = AvoidedPaths;
            VMATFileDialogModel.Instance.loadTreeData();
            this.DataContext = VMATFileDialogModel.Instance;
            VMATFileDialogModel.Instance.SelectedTreeViewItem = new FileFolderInfo() { Path = Properties.Settings.Default.ChoosenFileDirectory == string.Empty ? @".\" : Properties.Settings.Default.ChoosenFileDirectory, IsDirectory = true };
        }

        private void dg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (VMATFileDialogModel.Instance.SelectedFileItem.IsDirectory)
            //{
            //    getSelectedTreeViewItem(treeview.Items);
            //}
            dg.CancelEdit(DataGridEditingUnit.Row);
            if(VMATFileDialogModel.Instance.SelectedFileItem != null)
                if(VMATFileDialogModel.Instance.SelectedFileItem.IsDirectory)
                    VMATFileDialogModel.Instance.SelectedTreeViewItem = VMATFileDialogModel.Instance.SelectedFileItem;
        }

        //public void getSelectedTreeViewItem(ItemCollection items)
        //{
        //    foreach(FileFolderInfo item in items)
        //    {
        //        TreeViewItem maintreeitem = treeview.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
        //        if (maintreeitem != null)
        //        {
        //            if (item == VMATFileDialogModel.Instance.SelectedFileItem)
        //            {
        //                maintreeitem.IsSelected = true;
        //                break;
        //            }
        //            else
        //            {
        //                maintreeitem.IsExpanded = true;
        //                maintreeitem.IsSelected = true;
        //                if (maintreeitem.HasItems)
        //                    getSelectedTreeViewItem(maintreeitem.Items);
        //            }
        //        }
        //    }
        //}

        private void Button_Open_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.ToString().ToUpper() == "OPEN")
            {
                VMATFileDialogModel.Instance.IsOk = true;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                VMATFileDialogModel.Instance.IsOk = false;
                this.Close();
            }
            Properties.Settings.Default.ChoosenFileDirectory = VMATFileDialogModel.Instance.SelectedFileItem.GetDirectory();
            Properties.Settings.Default.Save();
        }

        private void dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) 
            {
                if (dg.SelectedItems.Count > 0)
                {
                    VMATFileDialogModel.Instance.TempSelectedFileItemList = new ObservableCollection<FileFolderInfo>();
                    foreach (FileFolderInfo file in dg.SelectedItems)
                        VMATFileDialogModel.Instance.TempSelectedFileItemList.Add(file);
                }
                
            }
            else
            {
                VMATFileDialogModel.Instance.TempSelectedFileItemList = new ObservableCollection<FileFolderInfo>();
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = treeview.ItemContainerGenerator.ContainerFromItem(VMATFileDialogModel.Instance.Drives.FirstOrDefault()) as TreeViewItem;
            if (item != null)
                item.IsSelected = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Back_MouseClick(object sender, MouseButtonEventArgs e)
        {
            string parentPath = System.IO.Directory.GetParent(VMATFileDialogModel.Instance.SelectedTreeViewItem.Path)?.FullName;
            if(parentPath != null)
                VMATFileDialogModel.Instance.SelectedTreeViewItem = new FileFolderInfo() { Path = parentPath, IsDirectory = true };
        }

        private void Forward_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if(VMATFileDialogModel.Instance.PreSelectFileItem != null)
                VMATFileDialogModel.Instance.SelectedTreeViewItem = VMATFileDialogModel.Instance.PreSelectFileItem;
        }
    }
}

public static class VisualTreeExt
{
    public static IEnumerable<T> GetDescendants<T>(DependencyObject parent) where T : DependencyObject
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < count; ++i)
        {
            // Obtain the child
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T)
                yield return (T)child;

            // Return all the descendant children
            foreach (var subItem in GetDescendants<T>(child))
                yield return subItem;
        }
    }
}
