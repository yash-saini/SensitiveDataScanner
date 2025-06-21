using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SesnsitiveDataScan.Services;
using System.Collections.ObjectModel;
using System.Text;

namespace SesnsitiveDataScan.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string fileContent;

        [ObservableProperty]
        private ObservableCollection<string> detectedItems = new();

        public MainViewModel()
        {
            FileContent = "No file loaded";
        }

        [RelayCommand]
        public async Task PickAndScanFile()
        {
            try
            {
                var file = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a .txt or .csv file",
                    // Corrected line:
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.plain-text", "public.comma-separated-values" } }, // iOS
                        { DevicePlatform.Android, new[] { "text/plain", "text/csv" } }, // Android
                        { DevicePlatform.WinUI, new[] { ".txt", ".csv" } }, // Windows
                        { DevicePlatform.macOS, new[] { "public.plain-text", "public.comma-separated-values" } }, // macOS
                    }),
                });

                if (file == null) return;

                string content = "";

                using (var stream = await file.OpenReadAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    content = await reader.ReadToEndAsync();
                }

                FileContent = content.Length > 500 ? content.Substring(0, 500) + "..." : content;

                var findings = DetectionService.DetectSensitiveData(content);
                DetectedItems.Clear();

                foreach (var (type, value) in findings)
                    DetectedItems.Add($"{type}: {value}");

                if (DetectedItems.Count == 0)
                    await Shell.Current.DisplayAlert("Scan Complete", "No sensitive data detected.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not read file: {ex.Message}", "OK");
            }
        }
    }
}
