using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MbitGate.model
{
    public class FileFolderInfo : NotifyPropertyChangedBase
    {
        private string _Path;
        public string Path
        {
            get
            {
                return _Path;
            }

            set
            {
                _Path = value;
                OnPropertyChanged("Path");
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        private bool _IsFile;
        public bool IsFile
        {
            get
            {
                return _IsFile;
            }

            set
            {
                _IsFile = value;
                OnPropertyChanged("IsFile");
            }
        }

        private bool _IsDirectory;
        public bool IsDirectory
        {
            get
            {
                return _IsDirectory;
            }

            set
            {
                _IsDirectory = value;
                OnPropertyChanged("IsDirectory");
            }
        }

        private bool _IsDrive;
        public bool IsDrive
        {
            get
            {
                return _IsDrive;
            }

            set
            {
                _IsDrive = value;
                OnPropertyChanged("IsDrive");
            }
        }

        private DateTime _DateModified;
        public DateTime DateModified
        {
            get
            {
                return _DateModified;
            }

            set
            {
                _DateModified = value;
                OnPropertyChanged("DateModified");
            }
        }

        private long _fileSize;
        public long FileSize
        {
            get
            {
                return _fileSize;
            }
            set
            {
                _fileSize = value;
                OnPropertyChanged("FileSize");
            }
        }

        private ObservableCollection<FileFolderInfo> _FileFolders;
        public ObservableCollection<FileFolderInfo> FileFolders
        {
            get
            {
                return _FileFolders;
            }

            set
            {
                _FileFolders = value;
                OnPropertyChanged("FileFolders");
            }
        }

        internal string GetDirectory()
        {
            if (IsDirectory)
            {
                return _Path;
            }
            else
            {
                return _Path.Substring(0, _Path.LastIndexOf('\\') + 1);
            }
        }
    }

    public class HardDriveInfo : NotifyPropertyChangedBase
    {
        private string _Path;
        public string Path
        {
            get
            {
                return _Path;
            }

            set
            {
                _Path = value;
                OnPropertyChanged("Path");
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        private bool _IsFile;
        public bool IsFile
        {
            get
            {
                return _IsFile;
            }

            set
            {
                _IsFile = value;
                OnPropertyChanged("IsFile");
            }
        }

        private bool _IsDirectory;
        public bool IsDirectory
        {
            get
            {
                return _IsDirectory;
            }

            set
            {
                _IsDirectory = value;
                OnPropertyChanged("IsDirectory");
            }
        }

        private bool _IsDrive;
        public bool IsDrive
        {
            get
            {
                return _IsDrive;
            }

            set
            {
                _IsDrive = value;
                OnPropertyChanged("IsDrive");
            }
        }

        private ObservableCollection<FileFolderInfo> _FileFolders;
        public ObservableCollection<FileFolderInfo> FileFolders
        {
            get
            {
                return _FileFolders;
            }

            set
            {
                _FileFolders = value;
                OnPropertyChanged("FileFolders");
            }
        }
    }
}
