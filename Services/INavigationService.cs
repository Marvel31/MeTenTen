namespace MeTenTenMaui.Services
{
    public interface INavigationService
    {
        /// <summary>
        /// 뒤로 갈 수 있는지 확인
        /// </summary>
        bool CanGoBack();

        /// <summary>
        /// 상위 페이지로 이동하거나 종료 확인
        /// </summary>
        /// <returns>네비게이션이 처리되었으면 true, 그렇지 않으면 false</returns>
        Task<bool> GoBackAsync();

        /// <summary>
        /// 앱 종료 확인 다이얼로그 표시
        /// </summary>
        /// <returns>사용자가 종료를 확인했으면 true</returns>
        Task<bool> RequestExitAsync();
    }
}

