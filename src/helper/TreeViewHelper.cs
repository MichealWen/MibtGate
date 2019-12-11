using System.Windows;
using System.Windows.Controls;


namespace MbitGate.helper
{
    public class TreeViewHelper
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached("SelectedItem", typeof(object), typeof(TreeViewHelper),
       new FrameworkPropertyMetadata(null, OnSelectedItemPropertyChanged));

        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(TreeViewHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(TreeViewHelper));

        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        public static string GetSelectedItem(DependencyObject dp)
        {
            return (string)dp.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject dp, object value)
        {
            dp.SetValue(SelectedItemProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeView treeListView = sender as TreeView;
            if (treeListView != null)
            {
                if ((bool)e.OldValue)
                    treeListView.SelectedItemChanged -= SelectedItemChanged;

                if ((bool)e.NewValue)
                    treeListView.SelectedItemChanged += SelectedItemChanged;
            }
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeView treeListView = sender as TreeView;
            if (treeListView != null)
            {
                treeListView.SelectedItemChanged -= SelectedItemChanged;

                if (!(bool)GetIsUpdating(treeListView))
                {
                    foreach (var item in treeListView.Items)
                    {
                        DependencyObject target = treeListView.ItemContainerGenerator.ContainerFromItem(item);
                        TreeViewItem targetItem = target as TreeViewItem;
                        if(targetItem != null)
                        {
                            if (targetItem == e.NewValue)
                            {
                                targetItem.IsSelected = true;
                                break;
                            }
                            else
                                targetItem.IsSelected = false;
                        }
                    }
                }
                treeListView.SelectedItemChanged += SelectedItemChanged;
            }
        }

        private static void SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            TreeView treeListView = sender as TreeView;
            if (treeListView != null)
            {
                SetIsUpdating(treeListView, true);
                SetSelectedItem(treeListView, treeListView.SelectedItem);
                SetIsUpdating(treeListView, false);
            }
        }
    }
}
