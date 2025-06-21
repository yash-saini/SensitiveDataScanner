using SesnsitiveDataScan.Services;
using System.Text;

namespace SesnsitiveDataScan
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
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

                if (FileContentLabel != null)
                {
                    FileContentLabel.Text = content.Length > 500
                        ? content.Substring(0, 500) + "..."
                        : content;
                }
                else
                {
                    Console.WriteLine("FileContentLabel not found. Content: " + (content.Length > 500 ? content.Substring(0, 500) + "..." : content));
                }

                // Run detection
                var findings = DetectionService.DetectSensitiveData(content);

                if (findings.Count == 0)
                {
                    await DisplayAlert("Scan Complete", "No sensitive data detected.", "OK");
                }
                else
                {
                    var report = string.Join("\n", findings.Select(f => $"{f.Type}: {f.Value}"));
                    await DisplayAlert("Sensitive Data Found", report, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not read file: {ex.Message}", "OK");
            }
        }
    }
}
