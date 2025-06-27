using SesnsitiveDataScan.ViewModels;

namespace SesnsitiveDataScan
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement element)
            {
                await element.ScaleTo(1.2, 100);
                await element.ScaleTo(1.0, 100);
            }
        }
    }
}
