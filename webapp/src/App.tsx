/**
 * App 컴포넌트 - 라우팅 설정
 */

import { lazy, Suspense, useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ConfigProvider, App as AntApp } from 'antd';
import koKR from 'antd/locale/ko_KR';
import { theme } from '@config/theme';
import { ROUTES } from '@config/routes';
import ProtectedRoute from '@components/ProtectedRoute';
import PublicRoute from '@components/PublicRoute';
import Layout from '@components/Layout';
import ErrorBoundary from '@components/ErrorBoundary';
import Loading from '@components/Loading';
import { auth } from '@config/firebase';
import { onAuthStateChanged } from 'firebase/auth';
import { authService } from '@services/AuthService';
import { useAuthStore } from '@stores/authStore';
import '@styles/global.css';

// Lazy load pages for code splitting
const Home = lazy(() => import('@pages/Home'));
const Login = lazy(() => import('@pages/Login'));
const SignUp = lazy(() => import('@pages/SignUp'));
const MyTenTens = lazy(() => import('@pages/MyTenTens'));
const PartnerTenTens = lazy(() => import('@pages/PartnerTenTens'));
const Feelings = lazy(() => import('@pages/Feelings'));
const Prayers = lazy(() => import('@pages/Prayers'));
const Settings = lazy(() => import('@pages/Settings'));
const NotFound = lazy(() => import('@pages/NotFound'));

function App() {
  const { setLoading } = useAuthStore();
  const [initializing, setInitializing] = useState(true);

  useEffect(() => {
    // Firebase 인증 상태 변경 리스너
    const unsubscribe = onAuthStateChanged(auth, async (firebaseUser) => {
      setLoading(true);
      
      try {
        if (firebaseUser) {
          // Firebase 인증 상태가 있으면 자동 로그인 시도
          console.log('Firebase user detected, attempting auto-login...');
          await authService.loadAndAutoLogin();
        } else {
          // Firebase 인증 상태가 없으면 저장된 자격증명으로 로그인 시도
          console.log('No Firebase user, attempting credential-based login...');
          await authService.loadAndAutoLogin();
        }
      } catch (error) {
        console.error('Failed to restore auth state:', error);
      } finally {
        setLoading(false);
        setInitializing(false);
      }
    });

    return () => unsubscribe();
  }, [setLoading]);

  // 초기화 중에는 로딩 화면 표시
  if (initializing) {
    return <Loading fullscreen />;
  }

  return (
    <ErrorBoundary>
      <ConfigProvider theme={theme} locale={koKR}>
        <AntApp>
          <BrowserRouter>
            <Suspense fallback={<Loading fullscreen />}>
              <Routes>
                {/* Public Routes (로그인 전에만 접근 가능) */}
                <Route element={<PublicRoute />}>
                  <Route path={ROUTES.LOGIN} element={<Login />} />
                  <Route path={ROUTES.SIGNUP} element={<SignUp />} />
                </Route>

                {/* Protected Routes (인증 필요) */}
                <Route element={<ProtectedRoute />}>
                  <Route element={<Layout />}>
                    <Route path={ROUTES.HOME} element={<Home />} />
                    <Route path={ROUTES.MY_TENTENS} element={<MyTenTens />} />
                    <Route path={ROUTES.PARTNER_TENTENS} element={<PartnerTenTens />} />
                    <Route path={ROUTES.FEELINGS} element={<Feelings />} />
                    <Route path={ROUTES.PRAYERS} element={<Prayers />} />
                    <Route path={ROUTES.SETTINGS} element={<Settings />} />
                  </Route>
                </Route>

                {/* Root Redirect */}
                <Route path="/" element={<Navigate to={ROUTES.HOME} replace />} />

                {/* 404 Page */}
                <Route path={ROUTES.NOT_FOUND} element={<NotFound />} />
              </Routes>
            </Suspense>
          </BrowserRouter>
        </AntApp>
      </ConfigProvider>
    </ErrorBoundary>
  );
}

export default App;
