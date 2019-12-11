using System;
using System.Windows;
using System.Windows.Input;


namespace MbitGate.helper
{
    public class DoubleClickBehavior
    {
        public static DependencyProperty OnDoubleClickProperty = DependencyProperty.RegisterAttached(
            "OnDoubleClick",
            typeof(ICommand),
            typeof(DoubleClickBehavior),
            new UIPropertyMetadata(DoubleClickBehavior.OnDoubleClick));

        public static void SetOnDoubleClick(DependencyObject target, ICommand value)
        {
            target.SetValue(OnDoubleClickProperty, value);
        }

        private static void OnDoubleClick(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var element = target as System.Windows.Controls.Control;
            if (element == null)
            {
                throw new InvalidOperationException("This behavior can be attached to a Control item only.");
            }

            if ((e.NewValue != null) && (e.OldValue == null))
            {
                element.MouseDoubleClick += MouseDoubleClick;
            }
            else if ((e.NewValue == null) && (e.OldValue != null))
            {
                element.MouseDoubleClick -= MouseDoubleClick;
            }
        }

        private static void MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UIElement element = (UIElement)sender;
            ICommand command = (ICommand)element.GetValue(OnDoubleClickProperty);
            command.Execute(null);
        }
    }

    public class SimpleCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }
        public Action<object> ExecuteDelegate { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
                return CanExecuteDelegate(parameter);
            return true;
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
                ExecuteDelegate(parameter);
        }
    }
}
