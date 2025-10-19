using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using MeTenTenMaui.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebView;

namespace MeTenTenMaui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private bool _isExitDialogShowing = false;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        // 키보드가 올라올 때 화면 크기 조정
        Window?.SetSoftInputMode(SoftInput.AdjustResize);
    }

#pragma warning disable CS0672 // Member overrides obsolete member
    public override void OnBackPressed()
#pragma warning restore CS0672 // Member overrides obsolete member
    {
        // 다이얼로그가 이미 표시 중이면 무시
        if (_isExitDialogShowing)
        {
            System.Diagnostics.Debug.WriteLine($"[MainActivity] Exit dialog already showing, ignoring back press");
            return;
        }

        try
        {
            System.Diagnostics.Debug.WriteLine($"[MainActivity] OnBackPressed called");
            
            var navigationManager = IPlatformApplication.Current?.Services.GetService<NavigationManager>();
            
            if (navigationManager != null)
            {
                string relativePath = "";
                try
                {
                    // 현재 경로 확인 (안전하게)
                    var currentUri = navigationManager.Uri;
                    var baseUri = navigationManager.BaseUri;
                    relativePath = currentUri.Replace(baseUri, "").TrimStart('/').TrimEnd('/');
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] Current path: '{relativePath}'");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] Error getting path: {ex.Message}");
                    // NavigationManager가 초기화되지 않은 경우 - 종료 다이얼로그 표시
                    ShowExitDialog();
                    return;
                }
                
                // Home 또는 Login/SignUp 페이지에서는 종료 확인 다이얼로그 표시
                if (string.IsNullOrEmpty(relativePath) || 
                    relativePath == "/" || 
                    relativePath.StartsWith("login") || 
                    relativePath.StartsWith("signup"))
                {
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] Showing exit dialog for path: '{relativePath}'");
                    ShowExitDialog();
                    return; // 기본 동작 차단
                }
                else
                {
                    // 다른 페이지에서는 Home으로 이동
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] Navigating to home from: '{relativePath}'");
                    try
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            try
                            {
                                navigationManager.NavigateTo("/", forceLoad: false);
                            }
                            catch (Exception navEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"[MainActivity] Navigation error: {navEx.Message}");
                                // 네비게이션 실패 시 종료 다이얼로그
                                ShowExitDialog();
                            }
                        });
                        return; // 기본 동작 차단
                    }
                    catch (Exception navEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"[MainActivity] Navigation setup error: {navEx.Message}");
                        ShowExitDialog();
                        return;
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] NavigationManager is null, showing exit dialog");
                ShowExitDialog();
                return;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MainActivity] Error in OnBackPressed: {ex.Message}");
            // 에러 발생 시 종료 다이얼로그 표시
            ShowExitDialog();
            return;
        }
    }

    private void ShowExitDialog()
    {
        if (_isExitDialogShowing)
        {
            return;
        }

        _isExitDialogShowing = true;
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                var alert = new AlertDialog.Builder(this);
                alert.SetTitle("앱 종료");
                alert.SetMessage("앱을 종료하시겠습니까?");
                alert.SetPositiveButton("예", (sender, args) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] User confirmed exit");
                    _isExitDialogShowing = false;
                    FinishAffinity(); // 앱 완전 종료
                });
                alert.SetNegativeButton("아니오", (sender, args) =>
                {
                    System.Diagnostics.Debug.WriteLine($"[MainActivity] User cancelled exit");
                    _isExitDialogShowing = false;
                });
                alert.SetCancelable(true);
                alert.SetOnCancelListener(new DialogCancelListener(() => {
                    _isExitDialogShowing = false;
                }));
                alert.Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainActivity] Error showing dialog: {ex.Message}");
                _isExitDialogShowing = false;
            }
        });
    }
}

// DialogInterface.IOnCancelListener 구현
public class DialogCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
{
    private readonly Action _onCancel;

    public DialogCancelListener(Action onCancel)
    {
        _onCancel = onCancel;
    }

    public void OnCancel(IDialogInterface? dialog)
    {
        _onCancel?.Invoke();
    }
}
