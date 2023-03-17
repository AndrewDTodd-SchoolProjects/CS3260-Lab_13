using ImageRotatorGUI_MAUI.Views;
using ImageRotatorGUI_MAUI.ViewModels;

using ImageRotatorBackend.Services;

namespace ImageRotatorGUI_MAUI;

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

        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();

#if WINDOWS
        builder.Services.AddTransient<IFolderPicker, ImageRotatorBackend.Services.FolderPicker>();
#endif

        return builder.Build();
	}
}
