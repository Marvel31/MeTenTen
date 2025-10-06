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

		// 서비스 등록
		builder.Services.AddSingleton<ITenTenService, TenTenService>();
		builder.Services.AddSingleton<ITopicService, TopicService>();
		builder.Services.AddSingleton<IFeelingService, FeelingService>();
		builder.Services.AddSingleton<IPrayerService, PrayerService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
