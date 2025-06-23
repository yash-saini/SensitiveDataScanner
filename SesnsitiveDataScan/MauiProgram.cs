using Microsoft.Extensions.Logging;
using SesnsitiveDataScan.Interface;
using SesnsitiveDataScan.Services;
using SesnsitiveDataScan.ViewModels;

namespace SesnsitiveDataScan
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<AppDbContext>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<IUserDialogService, UserDialogService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
