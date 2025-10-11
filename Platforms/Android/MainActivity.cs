using Android.App;
using Android.Content.PM;
using Android.OS;
using MeTenTenMaui.Services;
using Microsoft.AspNetCore.Components;

namespace MeTenTenMaui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
#pragma warning disable CS0672 // Member overrides obsolete member
    public override void OnBackPressed()
#pragma warning restore CS0672 // Member overrides obsolete member
    {
        try
        {
            var navService = IPlatformApplication.Current?.Services.GetService<INavigationService>();
            var navigationManager = IPlatformApplication.Current?.Services.GetService<NavigationManager>();
            
            if (navService != null && navigationManager != null)
            {
                // 현재 경로 확인
                var currentUri = navigationManager.Uri;
                var baseUri = navigationManager.BaseUri;
                var relativePath = currentUri.Replace(baseUri, "").TrimStart('/').TrimEnd('/');
                
                System.Diagnostics.Debug.WriteLine($"[MainActivity] OnBackPressed - Current path: '{relativePath}'");
                
                // Home 또는 Login/SignUp 페이지에서는 종료 확인 다이얼로그 표시
                if (string.IsNullOrEmpty(relativePath) || 
                    relativePath == "/" || 
                    relativePath.StartsWith("login") || 
                    relativePath.StartsWith("signup"))
                {
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] Showing exit dialog");
                    
                    // 네이티브 다이얼로그 표시
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        var alert = new AlertDialog.Builder(this);
                        alert.SetTitle("앱 종료");
                        alert.SetMessage("앱을 종료하시겠습니까?");
                        alert.SetPositiveButton("예", (sender, args) =>
                        {
                            System.Diagnostics.Debug.WriteLine($"[MainActivity] User confirmed exit");
                            FinishAffinity(); // 앱 완전 종료
                        });
                        alert.SetNegativeButton("아니오", (sender, args) =>
                        {
                            System.Diagnostics.Debug.WriteLine($"[MainActivity] User cancelled exit");
                            // 아무것도 하지 않음
                        });
                        alert.SetCancelable(true);
                        alert.Show();
                    });
                    
                    return; // 기본 동작 차단
                }
                else
                {
                    // 다른 페이지에서는 Home으로 이동
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] Navigating to home");
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        navigationManager.NavigateTo("/", forceLoad: false);
                    });
                    return; // 기본 동작 차단
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MainActivity] Error in OnBackPressed: {ex.Message}\n{ex.StackTrace}");
        }

        // 에러 발생 시 기본 동작
#pragma warning disable CS0618 // Type or member is obsolete
        base.OnBackPressed();
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
