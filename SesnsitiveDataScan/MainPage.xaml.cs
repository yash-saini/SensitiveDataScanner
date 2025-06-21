using SesnsitiveDataScan.ViewModels;

namespace SesnsitiveDataScan
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
        }
    }
}
