using SesnsitiveDataScan.Interface;

namespace SesnsitiveDataScan.Services
{
    public class UserDialogService : IUserDialogService
    {
        public Task ShowMessage(string title, string message, string buttonText = "OK")
        {
            return Shell.Current.DisplayAlert(title, message, buttonText);
        }
    }
}
