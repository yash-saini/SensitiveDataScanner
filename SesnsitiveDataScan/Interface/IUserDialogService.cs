namespace SesnsitiveDataScan.Interface
{
    public interface IUserDialogService
    {
        Task ShowMessage(string title, string message, string buttonText = "OK");
    }
}
