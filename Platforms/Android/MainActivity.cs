using Android.App;
using Android.Content.PM;
using Android.OS;
using MeTenTenMaui.Services;

namespace MeTenTenMaui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
#pragma warning disable CS0672 // Member overrides obsolete member
    public override void OnBackPressed()
#pragma warning restore CS0672 // Member overrides obsolete member
    {
        var navService = IPlatformApplication.Current?.Services.GetService<INavigationService>();
        
        if (navService != null)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var handled = await navService.GoBackAsync();
                if (!handled)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    base.OnBackPressed();
#pragma warning restore CS0618 // Type or member is obsolete
                }
            });
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            base.OnBackPressed();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
