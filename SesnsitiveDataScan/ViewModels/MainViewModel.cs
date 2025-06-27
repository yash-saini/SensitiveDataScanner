using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SesnsitiveDataScan.Interface;
using SesnsitiveDataScan.Models;
using SesnsitiveDataScan.Services;
using SesnsitiveDataScan.Utilities;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace SesnsitiveDataScan.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool showSettingsPanel = false;

        [ObservableProperty]
        private bool scanEmails = true;

        [ObservableProperty]
        private bool scanSSNs = true;

        [ObservableProperty]
        private bool scanCreditCards = true;

        [ObservableProperty]
        private bool scanPhones = true;

        [ObservableProperty]
        private bool usePartialMasking = true;

        [ObservableProperty]
        private bool isDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;

        // Redaction mode
        [ObservableProperty] bool useFullRedaction;

        [ObservableProperty]
        private string fileContent;

        [ObservableProperty]
        ObservableCollection<DetectedItem> detectedItems = new();

        [ObservableProperty]
        string filterText;

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        private bool hasDetectedItems;

        [ObservableProperty]
        private string fileName = "No file selected";

        [ObservableProperty]
        private string redactedContent;

        [ObservableProperty]
        private List<string> displayedItems = new();

        private readonly IUserDialogService _dialogService;

        public IEnumerable<DetectedItem> FilteredDetectedItems =>
                string.IsNullOrWhiteSpace(FilterText)
                    ? DetectedItems
                    : DetectedItems.Where(i =>
                        i.Original.ToLowerInvariant().StartsWith(FilterText.ToLowerInvariant()) ||
                        i.Masked.ToLowerInvariant().StartsWith(FilterText.ToLowerInvariant()));

        partial void OnFilterTextChanged(string oldValue, string newValue)
        {
            OnPropertyChanged(nameof(FilteredDetectedItems));
        }

        partial void OnIsDarkModeChanged(bool value)
        {
            Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
        }

        public ICommand ScanEmailsToggleCommand { get; }
        public ICommand ScanPhonesToggleCommand { get; }
        public ICommand ScanSSNsToggleCommand { get; }
        public ICommand ScanCreditCardsToggleCommand { get; }
        public ICommand SetPartialMaskingCommand { get; }
        public ICommand SetFullRedactionCommand { get; }
        public ICommand ToggleThemeCommand { get; }

        [ObservableProperty]
        private bool showContentOverviewPanel = false;

        [ObservableProperty]
        private ContentAnalysisResult contentAnalysis;

        [ObservableProperty]
        private bool isAnalyzingContent = false;

        private readonly ContentAnalysisService _contentAnalysisService;

        public MainViewModel(IUserDialogService dialogService)
        {
            _dialogService = dialogService;
            _contentAnalysisService = new ContentAnalysisService();
            FileContent = "No file loaded";
            ScanEmailsToggleCommand = new Command(ToggleScanEmails);
            ScanPhonesToggleCommand = new Command(ToggleScanPhones);
            ScanSSNsToggleCommand = new Command(ToggleScanSSNs);
            ScanCreditCardsToggleCommand = new Command(ToggleScanCreditCards);
            SetPartialMaskingCommand = new Command(SetPartialMasking);
            SetFullRedactionCommand = new Command(SetFullRedaction);
            ToggleThemeCommand = new Command(ToggleTheme);
        }

        [RelayCommand]
        private void ToggleContentOverview()
        {
            ShowContentOverviewPanel = !ShowContentOverviewPanel;

            if (ShowContentOverviewPanel)
            {
                ShowSettingsPanel = false;
                if (!string.IsNullOrEmpty(FileContent) && FileContent != "No file loaded")
                {
                    AnalyzeContentAsync();
                }
            }
        }

        private async void AnalyzeContentAsync()
        {
            if (IsAnalyzingContent) return;

            IsAnalyzingContent = true;
            try
            {
                ContentAnalysis = await _contentAnalysisService.AnalyzeContentAsync(FileContent);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessage("Analysis Error", $"Could not analyze content: {ex.Message}", "OK");
            }
            finally
            {
                IsAnalyzingContent = false;
            }
        }

        private void ToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
        }

        private void ToggleScanEmails()
        {
            // Logic to toggle ScanEmails
            ScanEmails = !ScanEmails;
            OnPropertyChanged(nameof(ScanEmails));
        }
        private void ToggleScanPhones()
        {
            ScanPhones = !ScanPhones;
            OnPropertyChanged(nameof(ScanPhones));
        }

        private void ToggleScanSSNs()
        {
            ScanSSNs = !ScanSSNs;
            OnPropertyChanged(nameof(ScanSSNs));
        }

        private void ToggleScanCreditCards()
        {
            ScanCreditCards = !ScanCreditCards;
            OnPropertyChanged(nameof(ScanCreditCards));
        }

        private void SetPartialMasking()
        {
            UsePartialMasking = true;
            UseFullRedaction = false;
        }

        private void SetFullRedaction()
        {
            UseFullRedaction = true;
            UsePartialMasking = false;
        }

        [RelayCommand]
        public async Task PickAndScanFile()
        {
            IsBusy = true;
            try
            {
                var file = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a .txt or .csv file",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "com.microsoft.word.doc", "org.openxmlformats.wordprocessingml.document", "org.openxmlformats.spreadsheetml.sheet", "public.plain-text", "public.comma-separated-values" } },
                        { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "text/plain", "text/csv" } },
                        { DevicePlatform.WinUI, new[] { ".txt", ".csv", ".docx", ".xlsx" } },
                        { DevicePlatform.macOS, new[] { "org.openxmlformats.wordprocessingml.document", "org.openxmlformats.spreadsheetml.sheet", "public.plain-text", "public.comma-separated-values" } },
                    }),
                });

                if (file == null) return;

                string content = "";
                string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                using var stream = await file.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                if (extension == ".docx")
                {
                    content = WordTextExtractorUtility.ExtractText(fileBytes);
                }
                else if (extension == ".xlsx")
                {
                    content = ExcelTextExtractorUtility.ExtractText(fileBytes);
                }
                else
                {
                    content = Encoding.UTF8.GetString(fileBytes);
                }

                FileContent = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
                FileName = file.FileName;

                var redactionResult = await Task.Run(() =>
                    DetectionService.DetectAndRedactSensitiveData(
                        content,
                        ScanEmails, ScanPhones, ScanCreditCards, ScanSSNs,
                        UseFullRedaction));
                RedactedContent = redactionResult.RedactedContent;
                DetectedItems = new ObservableCollection<DetectedItem>(
                    redactionResult.DetectedItems.Select(d => new DetectedItem
                    {
                        Original = d.Value,
                        Masked = UseFullRedaction ? new string('*', d.Value.Length) : MaskingUtils.MaskData(d.Type, d.Value),
                        Type = d.Type
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
            finally
            {
                IsBusy = false;
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

        [RelayCommand]
        private void ToggleSettings()
        {
            ShowSettingsPanel = !ShowSettingsPanel;
        }

        [RelayCommand]
        private async Task ApplySettings()
        {
            if (string.IsNullOrEmpty(FileContent) || FileContent == "No file loaded")
            {
                await _dialogService.ShowMessage("Info", "Please select a file first.", "OK");
                return;
            }

            // Hide the settings panel
            ShowSettingsPanel = false;

            // Re-scan with new settings (reuse existing file content)
            IsBusy = true;
            try
            {
                var redactionResult = await Task.Run(() =>
                    DetectionService.DetectAndRedactSensitiveData(
                        FileContent,
                        ScanEmails, ScanPhones, ScanCreditCards, ScanSSNs,
                        UseFullRedaction));

                RedactedContent = redactionResult.RedactedContent;
                DetectedItems = new ObservableCollection<DetectedItem>(
                    redactionResult.DetectedItems.Select(d => new DetectedItem
                    {
                        Original = d.Value,
                        Masked = UseFullRedaction ? new string('*', d.Value.Length) : MaskingUtils.MaskData(d.Type, d.Value),
                        Type = d.Type
                    }));

                OnPropertyChanged(nameof(FilteredDetectedItems));
                HasDetectedItems = DetectedItems.Any();

                if (!HasDetectedItems)
                    await _dialogService.ShowMessage("Scan Complete", "No sensitive data detected.");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessage("Error", $"Could not process file: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
