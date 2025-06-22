using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SesnsitiveDataScan.Services;
using System.Text;

namespace SesnsitiveDataScan.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string fileContent;

        [ObservableProperty]
        private List<string> detectedItems = new();

        [ObservableProperty]
        private bool hasDetectedItems;

        [ObservableProperty]
        private string fileName = "No file selected";

        [ObservableProperty]
        private List<string> displayedItems = new();

        private List<string> allDetectedItems = new();

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

                var findings = await Task.Run(() => DetectionService.DetectSensitiveData(content));

                allDetectedItems = findings.Select(f => $"{f.Type}: {f.Value}").ToList();
                DetectedItems = [.. allDetectedItems.Take(100)];
                DisplayedItems = DetectedItems;
                FileName = file.FileName;
                HasDetectedItems = DisplayedItems != null && DisplayedItems.Any();
                if (!DetectedItems.Any())
                    await Shell.Current.DisplayAlert("Scan Complete", "No sensitive data detected.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not read file: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task ExportAllResults()
        {
            try
            {
                var filename = $"ScanResults_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, filename);

                await File.WriteAllLinesAsync(filePath, allDetectedItems);

                await Shell.Current.DisplayAlert("Export Successful", $"Saved to:\n{filePath}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Export Failed", ex.Message, "OK");
            }
        }
    }
}
