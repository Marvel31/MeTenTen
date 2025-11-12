/**
 * Protected Route 컴포넌트
 * 인증된 사용자만 접근 가능
 */

import { Navigate, Outlet } from 'react-router-dom';
import { useAuthStore } from '@stores/authStore';
import { ROUTES } from '@config/routes';

interface ProtectedRouteProps {
  children?: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
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

  // 인증되지 않은 경우 로그인 페이지로 리다이렉트
  if (!isAuthenticated) {
    return <Navigate to={ROUTES.LOGIN} replace />;
  }

  // 인증된 경우 자식 컴포넌트 또는 Outlet 렌더링
  return children ? <>{children}</> : <Outlet />;
};

export default ProtectedRoute;

