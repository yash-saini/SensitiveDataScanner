using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SesnsitiveDataScan.Interface;
using SesnsitiveDataScan.Models;
using SesnsitiveDataScan.Services;
using SesnsitiveDataScan.Utilities;
using System.Collections.ObjectModel;
using System.Text;

namespace SesnsitiveDataScan.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string fileContent;

        [ObservableProperty]
        ObservableCollection<DetectedItem> detectedItems = new();

        [ObservableProperty]
        string filterText;

        [ObservableProperty]
        private bool hasDetectedItems;

        [ObservableProperty]
        private string fileName = "No file selected";

        [ObservableProperty]
        private string redactedContent;

        [ObservableProperty]
        private List<string> displayedItems = new();

        private List<string> allDetectedItems = new();

        private readonly IUserDialogService _dialogService;

        public IEnumerable<DetectedItem> FilteredDetectedItems =>
                string.IsNullOrWhiteSpace(FilterText)
                    ? DetectedItems
                    : DetectedItems.Where(i =>
                        i.Original.ToString().ToLower().StartsWith(FilterText.ToLower()) ||
                        i.Masked.ToString().ToLower().StartsWith(FilterText.ToLower()));

        partial void OnFilterTextChanged(string oldValue, string newValue)
        {
            OnPropertyChanged(nameof(FilteredDetectedItems));
        }

        public MainViewModel(IUserDialogService dialogService)
        {
            _dialogService = dialogService;
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
                FileName = file.FileName;

                var redactionResult = await Task.Run(() => DetectionService.DetectAndRedactSensitiveData(content));
                RedactedContent = redactionResult.RedactedContent;

                DetectedItems = new ObservableCollection<DetectedItem>(
                    redactionResult.DetectedItems.Select(d => new DetectedItem
                    {
                        Original = d.Value,
                        Masked = MaskingUtils.MaskData(d.Type, d.Value)
                    }));
                OnPropertyChanged(nameof(FilteredDetectedItems));

                HasDetectedItems = DetectedItems.Any();

                if (!HasDetectedItems)
                    await _dialogService.ShowMessage("Scan Complete", "No sensitive data detected.");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessage("Error", $"Could not read file: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task ExportResults()
        {
            try
            {
                if (string.IsNullOrEmpty(FileContent) || FileContent == "No file loaded")
                {
                    await _dialogService.ShowMessage("Error", "No file content to export.", "OK");
                    return;
                }

                var lines = new List<string>
                    {
                        $"Original File: {FileName}",
                        "",
                        "Detected Sensitive Items (Type | Original -> Masked):"
                    };

                lines.AddRange(DetectedItems.Select(i => $"{i.Original} -> {i.Masked}"));
                lines.Add("");
                lines.Add("---- REDACTED CONTENT ----");
                lines.Add(RedactedContent);

                var filename = $"ScanAndRedacted_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, filename);

                await File.WriteAllLinesAsync(filePath, lines);
                await _dialogService.ShowMessage("Export Successful", $"Saved to:\n{filePath}", "OK");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessage("Export Failed", ex.Message, "OK");
            }
        }
    }
}
