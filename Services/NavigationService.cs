using Microsoft.AspNetCore.Components;

namespace MeTenTenMaui.Services
{
    public class NavigationService : INavigationService
    {
        private readonly NavigationManager _navigationManager;

        public NavigationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public bool CanGoBack()
        {
            var currentUri = _navigationManager.Uri;
            var baseUri = _navigationManager.BaseUri;
            var relativePath = currentUri.Replace(baseUri, "/");

            // Home 페이지가 아니면 뒤로 갈 수 있음
            return !relativePath.Equals("/") && !relativePath.Equals("");
        }

        public async Task<bool> GoBackAsync()
        {
            try
            {
                var currentUri = _navigationManager.Uri;
                var baseUri = _navigationManager.BaseUri;
                var relativePath = currentUri.Replace(baseUri, "/").TrimEnd('/');
                
                // 경로 정규화
                if (string.IsNullOrEmpty(relativePath) || relativePath == "/")
                {
                    relativePath = "/";
                }

                System.Diagnostics.Debug.WriteLine($"[Navigation] Current path: {relativePath}");

                // Login/SignUp 페이지에서도 종료 확인 (로그아웃 상태에서 Back 키)
                if (relativePath.StartsWith("/login") || relativePath.StartsWith("/signup"))
                {
                    return await RequestExitAsync();
                }

                // Home 페이지에서 Back 키를 누르면 종료 확인
                if (relativePath == "/" || relativePath == "")
                {
                    return await RequestExitAsync();
                }

                // 다른 모든 페이지에서는 Home으로 이동
                _navigationManager.NavigateTo("/", forceLoad: false);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Navigation] Error in GoBackAsync: {ex.Message}");
                // 에러가 발생하면 종료 확인 다이얼로그 표시
                return await RequestExitAsync();
            }
        }

        public async Task<bool> RequestExitAsync()
        {
            try
            {
                if (Application.Current?.Windows.Count > 0)
                {
                    var window = Application.Current.Windows[0];
                    if (window?.Page != null)
                    {
                        var result = await window.Page.DisplayAlert(
                            "앱 종료",
                            "앱을 종료하시겠습니까?",
                            "예",
                            "아니오"
                        );

                        if (result)
                        {
                            // 앱 종료
                            System.Diagnostics.Debug.WriteLine("[Navigation] User confirmed exit");
                            Application.Current?.Quit();
                        }

                        return true; // 다이얼로그를 표시했으므로 이벤트 처리됨
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Navigation] Error showing exit dialog: {ex.Message}");
            }

            return false;
        }
    }
}

