using Image_Processing.Core;
using System;


namespace Image_Processing.MVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand GrayscaleViewCommand { get; set; }
        public RelayCommand GrayprocessViewCommand { get; set; }
        public RelayCommand ColorViewCommand { get; set; }
        public RelayCommand ColorprocessViewCommand { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public GrayscaleViewModel GrayscaleVM { get; set; }
        public GrayprocessViewModel GrayprocessVM { get; set; }
        public ColorViewModel ColorVM { get; set; }
        public ColorprocessViewModel ColorprocessVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set { 
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            GrayscaleVM = new GrayscaleViewModel();
            GrayprocessVM = new GrayprocessViewModel();
            ColorVM = new ColorViewModel();
            ColorprocessVM = new ColorprocessViewModel();
            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o => { CurrentView = HomeVM; });
            GrayscaleViewCommand = new RelayCommand(o => { CurrentView = GrayscaleVM; });
            GrayprocessViewCommand = new RelayCommand(o => { CurrentView = GrayprocessVM; });
            ColorViewCommand = new RelayCommand(o => { CurrentView = ColorVM; });
            ColorprocessViewCommand = new RelayCommand(o => { CurrentView = ColorprocessVM; });
        }
    }
}
