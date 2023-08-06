using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace thronefall_modloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        public struct TabButtonContext
        {
            private readonly MainWindow _window;
            private readonly int _tabIndex;
            
            public Button Button { get; private set; }

            public TabButtonContext(MainWindow window, Button button, int index)
            {
                _window = window;
                _tabIndex = index;
                Button = button;
            }

            public Visibility Visibility => _window.ChosenTab == _tabIndex ? Visibility.Visible : Visibility.Collapsed;
            public Brush NormalColor => TabButtonBrush(_window.ChosenTab == _tabIndex, 0);
            public Brush MouseOverColor => TabButtonBrush(_window.ChosenTab == _tabIndex, 1);
            public Brush ClickColor => TabButtonBrush(_window.ChosenTab == _tabIndex, 2);
        
            private static Brush TabButtonBrush(bool selected, byte level)
            {
                var color = (byte)(selected ? 0x6c : 0x3c + level * 0x11);
                return new SolidColorBrush(Color.FromRgb(color, color, color));
            }
        }
        
        private readonly List<TabButtonContext> _buttonContexts = new();
        private int _chosenTab;

        private int ChosenTab
        {
            get => _chosenTab;
            set
            {
                _chosenTab = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ModLauncherVisibility));
                OnPropertyChanged(nameof(ModBrowserVisibility));
                OnPropertyChanged(nameof(OptionsVisibility));
                OnPropertyChanged(nameof(AboutVisibility));
                foreach (var context in _buttonContexts)
                {
                    // We do it like this instead of using property changed,
                    // because apparently templates don't play nice with it.
                    context.Button.Style = null;
                    context.Button.Style = Resources["TabButtonStyle"] as Style;
                }
            }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            AddButtonContext(ModLauncher, 0);
            AddButtonContext(ModBrowser, 1);
            AddButtonContext(Options, 2);
            AddButtonContext(About, 3);
        }

        private void AddButtonContext(Button button, int index)
        {
            var context = new TabButtonContext(this, button, index);
            button.DataContext = context;
            _buttonContexts.Add(context);
        }

        private void ChangeTabs(object sender, RoutedEventArgs e)
        {
            ChosenTab = int.Parse((string)((Button)sender).Tag);
        }

        public Visibility ModLauncherVisibility => _buttonContexts[0].Visibility;
        public Visibility ModBrowserVisibility => _buttonContexts[1].Visibility;
        public Visibility OptionsVisibility => _buttonContexts[2].Visibility;
        public Visibility AboutVisibility => _buttonContexts[3].Visibility;

        // Property Changed handlers
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}