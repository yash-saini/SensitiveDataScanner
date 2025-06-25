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
    }
}
