using System;
using System.Drawing;
using System.Drawing.IconLib;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IcoExtractor.Enums;
using IcoExtractor.Interfaces;
using IcoExtractor.Services;
using Microsoft.Win32;

namespace IcoExtractor.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        #region private fields
        private string _icoPath;
        private RelayCommand _pickFileCommand;
        private RelayCommand _saveIconCommand;
        private RelayCommand _setDesktopLocationCommand;
        private RelayCommand _setMyLocationCommand;
        private IIconExtractionService _iconExtractionService;
        private BitmapSource _bitmapSource;
        private Icon _icon;
        private SavingLocation _savingLocation;
        private ResourceManager _resourceManager;
        private bool _isQuickViewVisible;
        #endregion

        #region Constructor
        public MainViewModel()
        {
            _resourceManager = new ResourceManager("IcoExtractor.Properties.Resources", Assembly.GetExecutingAssembly());
            IsQuickviewVisibile = false;
        }
        #endregion

        #region Getters/Setters

        public bool IsQuickviewVisibile
        {
            get
            {
                return _isQuickViewVisible;
            }
            set
            {
                if (_isQuickViewVisible != value)
                {
                    _isQuickViewVisible = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string IcoPath
        {
            get
            {
                return _icoPath;
            }
            set
            {
                if (_icoPath != value)
                {
                    _icoPath = value;
                    RaisePropertyChanged();
                }
            }
        }

        public BitmapSource BitmapSource
        {
            get
            {
                return _bitmapSource;
                
            }
            set {
                if (_bitmapSource != value)
                {
                    _bitmapSource = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Button getters

        public RelayCommand PickFileCommand
        {
            get
            {
                return _pickFileCommand ?? (_pickFileCommand = new RelayCommand(PickFile));
            }
        }

        public RelayCommand SaveIconCommand
        {
            get
            {
                return _saveIconCommand ?? (_saveIconCommand = new RelayCommand(SaveIcon));
            }
        }

        public RelayCommand SetDesktopLocationCommand
        {
            get
            {
                return _setDesktopLocationCommand ?? (_setDesktopLocationCommand = new RelayCommand(SetDesktopLocation));
            }
        }

        public RelayCommand SetMyLocationCommand
        {
            get
            {
                return _setMyLocationCommand ?? (_setMyLocationCommand = new RelayCommand(SetMyLocation));
            }
        }
        #endregion

        #region Button actions

        private void PickFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (dlg.ShowDialog() == true)
            {
                IcoPath = dlg.FileName;
            }
            ExtractIcon();
            IsQuickviewVisibile = true;
        }



        private void SaveIcon()
        {
            if (_icon == null)
            {
                MessageBox.Show(_resourceManager.GetString("NoPathError"), _resourceManager.GetString("Error"));
                return;
            }
            string location = String.Empty;
            if (_savingLocation == SavingLocation.Desktop)
            {
                location = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                location = Path.Combine(location, "Icon"+Guid.NewGuid()+".ico");
            }
            else if (_savingLocation == SavingLocation.MyLocation)
            {
                SaveFileDialog saveDlg = new SaveFileDialog();
                saveDlg.FileName = "Icon";
                saveDlg.DefaultExt = ".ico";
                saveDlg.Filter = "Ico files (.ico)|*.ico|Png files (.png)|*.png";
                bool? result = saveDlg.ShowDialog();
                if (result == true)
                    location = saveDlg.FileName;
            }

            try
            {
                string fName = Guid.NewGuid().ToString();
                MultiIcon mIcon = new MultiIcon();
                SingleIcon sIcon = mIcon.Add(fName);
                sIcon.CreateFrom(_icon.ToBitmap(), IconOutputFormat.Vista);
                sIcon.Save(location);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void SetDesktopLocation()
        {
            _savingLocation = SavingLocation.Desktop;
        }

        private void SetMyLocation()
        {
            _savingLocation = SavingLocation.MyLocation;
        }
        #endregion

        #region Methods
        private void ExtractIcon()
        {
            if (!String.IsNullOrEmpty(IcoPath))
            {
                try
                {
                    _iconExtractionService = new IconExtractionService(IcoPath);
                    Icon ico = _iconExtractionService.GetAppIcon();
                    using (Bitmap bm = ico.ToBitmap())
                    {
                        var stream = new MemoryStream();
                        bm.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        BitmapSource = BitmapFrame.Create(stream);
                    }
                    _icon = ico;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
       
        #endregion
    }
}