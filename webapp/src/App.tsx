/**
 * App 컴포넌트 - 라우팅 설정
 */

import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ConfigProvider } from 'antd';
import koKR from 'antd/locale/ko_KR';
import { theme } from '@config/theme';
import { ROUTES } from '@config/routes';
import ProtectedRoute from '@components/ProtectedRoute';
import PublicRoute from '@components/PublicRoute';
import {
  Home,
  Login,
  SignUp,
  MyTenTens,
  PartnerTenTens,
  Feelings,
  Prayers,
  Settings,
  NotFound,
} from '@pages/index';
import '@styles/global.css';

function App() {
  return (
    <ConfigProvider theme={theme} locale={koKR}>
      <BrowserRouter>
        <Routes>
          {/* Public Routes (로그인 전에만 접근 가능) */}
          <Route element={<PublicRoute />}>
            <Route path={ROUTES.LOGIN} element={<Login />} />
            <Route path={ROUTES.SIGNUP} element={<SignUp />} />
          </Route>

          {/* Protected Routes (인증 필요) */}
          <Route element={<ProtectedRoute />}>
            <Route path={ROUTES.HOME} element={<Home />} />
            <Route path={ROUTES.MY_TENTENS} element={<MyTenTens />} />
            <Route path={ROUTES.PARTNER_TENTENS} element={<PartnerTenTens />} />
            <Route path={ROUTES.FEELINGS} element={<Feelings />} />
            <Route path={ROUTES.PRAYERS} element={<Prayers />} />
            <Route path={ROUTES.SETTINGS} element={<Settings />} />
          </Route>

          {/* Root Redirect */}
          <Route path="/" element={<Navigate to={ROUTES.HOME} replace />} />

          {/* 404 Page */}
          <Route path={ROUTES.NOT_FOUND} element={<NotFound />} />
        </Routes>
      </BrowserRouter>
    </ConfigProvider>
  );
}

export default App;
