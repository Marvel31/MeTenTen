/**
 * Public Route 컴포넌트
 * 로그인하지 않은 사용자만 접근 가능 (로그인, 회원가입 페이지)
 */

import { Navigate, Outlet } from 'react-router-dom';
import { useAuthStore } from '@stores/authStore';
import { ROUTES } from '@config/routes';

interface PublicRouteProps {
  children?: React.ReactNode;
}

const PublicRoute: React.FC<PublicRouteProps> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuthStore();

  // 로딩 중일 때
  if (isLoading) {
    return (
      <div
        style={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          height: '100vh',
        }}
      >
        <div>로딩 중...</div>
      </div>
    );
  }

  // 이미 인증된 경우 홈으로 리다이렉트
  if (isAuthenticated) {
    return <Navigate to={ROUTES.HOME} replace />;
  }

  // 인증되지 않은 경우 자식 컴포넌트 또는 Outlet 렌더링
  return children ? <>{children}</> : <Outlet />;
};

export default PublicRoute;

