using Microsoft.Extensions.Logging;
using MeTenTenMaui.Services;

namespace MeTenTenMaui;

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
			});

		builder.Services.AddMauiBlazorWebView();

		// 앱 데이터 디렉토리 로깅
		System.Diagnostics.Debug.WriteLine($"[App] AppDataDirectory: {FileSystem.AppDataDirectory}");
		System.Diagnostics.Debug.WriteLine($"[App] CacheDirectory: {FileSystem.CacheDirectory}");

	// 서비스 등록
	// Firebase Services
	builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();
	builder.Services.AddSingleton<IFirebaseDataService, FirebaseDataService>();
	builder.Services.AddSingleton<ITopicService, FirebaseTopicService>();
	builder.Services.AddSingleton<ITenTenService, FirebaseTenTenService>();
	
	// Other Services
	builder.Services.AddSingleton<IFeelingService, FeelingService>();
	builder.Services.AddSingleton<IFeelingExampleService, FeelingExampleService>();
	builder.Services.AddSingleton<IPrayerService, PrayerService>();
	builder.Services.AddSingleton<INavigationService, NavigationService>();
	
	// Keep FileStorageService for migration/export
	builder.Services.AddSingleton<IFileStorageService, FileStorageService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
