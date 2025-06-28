using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SesnsitiveDataScan.Utilities
{
    public class ThemeHelper : INotifyPropertyChanged
    {
        public static ThemeHelper Instance { get; } = new ThemeHelper();

        private ThemeHelper()
        {
            // Initialize with current theme
            UpdateThemeProperties();

            // Listen for theme changes
            Application.Current.RequestedThemeChanged += (s, e) =>
            {
                UpdateThemeProperties();
            };
        }

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            private set
            {
                if (_isDarkTheme != value)
                {
                    _isDarkTheme = value;
                    OnPropertyChanged();
                }
            }
        }

        // Can be bound directly in XAML
        public Color TextColor => IsDarkTheme ?
            GetResourceColor("TextColorDark") :
            GetResourceColor("TextColorLight");

        public Color CardBackground => IsDarkTheme ?
            GetResourceColor("CardBackgroundDark") :
            GetResourceColor("CardBackgroundLight");

        public Color DetectedItemBackground => IsDarkTheme ?
            GetResourceColor("DetectedItemBgDark") :
            GetResourceColor("DetectedItemBgLight");

        public Color PageBackground => IsDarkTheme ?
            GetResourceColor("PageBackgroundDark") :
            GetResourceColor("PageBackgroundLight");

        private void UpdateThemeProperties()
        {
            IsDarkTheme = Application.Current.RequestedTheme == AppTheme.Dark;
            // This triggers property change notifications for all computed properties
            OnPropertyChanged(nameof(TextColor));
            OnPropertyChanged(nameof(CardBackground));
            OnPropertyChanged(nameof(DetectedItemBackground));
            OnPropertyChanged(nameof(PageBackground));
        }

        private Color GetResourceColor(string resourceKey)
        {
            if (Application.Current.Resources.TryGetValue(resourceKey, out var color) && color is Color c)
            {
                return c;
            }
            return Colors.Gray;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnThemeChanged()
        {
            UpdateThemeProperties();
        }
    }
}