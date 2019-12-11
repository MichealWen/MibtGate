using System.Collections.Generic;
using System.ComponentModel;

namespace MbitGate.model
{
    public class UpdateModel
    {
        public string Item { get; set; }
        public uint Rate { get; set; }
        public string BinPath { get; set; }
    }

    public class Rate
    {
        public string RateKey { get; set; }
        public uint RateValue { get; set; }
    }

    public class Item
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class FileItem : Item
    {

    }

    public class DirectoryItem : Item
    {
        public List<Item> Items { get; set; }

        public DirectoryItem()
        {
            Items = new List<Item>();
        }
    }

    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string args)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(args));
            }
        }
    }

}
