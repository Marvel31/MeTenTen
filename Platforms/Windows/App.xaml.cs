using Microsoft.UI.Xaml;
using MeTenTenMaui.Services;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MeTenTenMaui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		this.InitializeComponent();
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		base.OnLaunched(args);

		// Windows Back 버튼 처리를 위한 키보드 이벤트 리스너 설정
		Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping("BackNavigation", (handler, view) =>
		{
			var nativeWindow = handler.PlatformView;
			nativeWindow.Closed += async (sender, e) =>
			{
				// Window가 닫힐 때 처리
				var navService = handler.MauiContext?.Services.GetService<INavigationService>();
				if (navService != null)
				{
					// 종료 확인 필요 시 처리
					e.Handled = true;
					var shouldExit = await navService.RequestExitAsync();
					if (shouldExit)
					{
						nativeWindow.Close();
					}
				}
			};
		});
	}
}

