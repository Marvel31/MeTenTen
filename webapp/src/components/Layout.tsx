/**
 * 공통 레이아웃 컴포넌트
 */

import { useState, useEffect } from 'react';
import { Layout as AntLayout, Menu, Button, Drawer, Typography, Space, Dropdown } from 'antd';
import type { MenuProps } from 'antd';
import {
  HomeOutlined,
  FileTextOutlined,
  HeartOutlined,
  BookOutlined,
  ReadOutlined,
  SettingOutlined,
  MenuOutlined,
  UserOutlined,
  LogoutOutlined,
} from '@ant-design/icons';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import { useAuthStore } from '@stores/authStore';
import { authService } from '@services/AuthService';
import { ROUTES } from '@config/routes';
import { App } from 'antd';

const { Header, Content, Sider } = AntLayout;
const { Title, Text } = Typography;

export default function Layout() {
  const { message } = App.useApp();
  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAuthStore();

  const [collapsed, setCollapsed] = useState(false);
  const [mobileDrawerOpen, setMobileDrawerOpen] = useState(false);
  const [isMobile, setIsMobile] = useState(false);

  // 모바일 감지
  useEffect(() => {
    const checkMobile = () => {
      setIsMobile(window.innerWidth < 768);
    };

    checkMobile();
    window.addEventListener('resize', checkMobile);

    return () => {
      window.removeEventListener('resize', checkMobile);
    };
  }, []);

  // 메뉴 아이템 정의
  const menuItems: MenuProps['items'] = [
    {
      key: ROUTES.HOME,
      icon: <HomeOutlined />,
      label: '홈',
      onClick: () => {
        navigate(ROUTES.HOME);
        if (isMobile) setMobileDrawerOpen(false);
      },
    },
    {
      key: ROUTES.MY_TENTENS,
      icon: <FileTextOutlined />,
      label: '나의 10&10',
      onClick: () => {
        navigate(ROUTES.MY_TENTENS);
        if (isMobile) setMobileDrawerOpen(false);
      },
    },
    {
      key: ROUTES.PARTNER_TENTENS,
      icon: <HeartOutlined />,
      label: '배우자 10&10',
      onClick: () => {
        navigate(ROUTES.PARTNER_TENTENS);
        if (isMobile) setMobileDrawerOpen(false);
      },
    },
    {
      key: ROUTES.FEELINGS,
      icon: <ReadOutlined />,
      label: '느낌 표현',
      onClick: () => {
        navigate(ROUTES.FEELINGS);
        if (isMobile) setMobileDrawerOpen(false);
      },
    },
    {
      key: ROUTES.PRAYERS,
      icon: <BookOutlined />,
      label: '기도문',
      onClick: () => {
        navigate(ROUTES.PRAYERS);
        if (isMobile) setMobileDrawerOpen(false);
      },
    },
    {
      key: ROUTES.SETTINGS,
      icon: <SettingOutlined />,
      label: '설정',
      onClick: () => {
        navigate(ROUTES.SETTINGS);
        if (isMobile) setMobileDrawerOpen(false);
      },
    },
  ];

  // 로그아웃 핸들러
  const handleLogout = async () => {
    try {
      await authService.signOut();
      message.success('로그아웃되었습니다.');
      navigate(ROUTES.LOGIN);
    } catch (error) {
      console.error('Logout error:', error);
      message.error('로그아웃에 실패했습니다.');
    }
  };

  // 사용자 메뉴
  const userMenuItems: MenuProps['items'] = [
    {
      key: 'profile',
      icon: <UserOutlined />,
      label: user?.displayName || '사용자',
      disabled: true,
    },
    {
      type: 'divider',
    },
    {
      key: 'settings',
      icon: <SettingOutlined />,
      label: '설정',
      onClick: () => navigate(ROUTES.SETTINGS),
    },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: '로그아웃',
      onClick: handleLogout,
    },
  ];

  // 현재 선택된 메뉴 키
  const selectedKey = location.pathname;

  return (
    <AntLayout style={{ minHeight: '100vh' }} role="main" aria-label="메인 레이아웃">
      {/* 데스크톱 사이드바 */}
      {!isMobile && (
        <Sider
          collapsible
          collapsed={collapsed}
          onCollapse={setCollapsed}
          style={{
            overflow: 'auto',
            height: '100vh',
            position: 'fixed',
            left: 0,
            top: 0,
            bottom: 0,
          }}
        >
          <div
            style={{
              height: '64px',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              color: 'white',
              fontSize: collapsed ? '16px' : '20px',
              fontWeight: 'bold',
              padding: '0 16px',
              transition: 'all 0.2s',
            }}
          >
            {collapsed ? '10' : '10&10'}
          </div>
          <Menu
            theme="dark"
            selectedKeys={[selectedKey]}
            mode="inline"
            items={menuItems}
            role="navigation"
            aria-label="주 메뉴"
          />
        </Sider>
      )}

      <AntLayout style={{ marginLeft: isMobile ? 0 : collapsed ? 80 : 200, transition: 'margin-left 0.2s' }}>
        {/* 헤더 */}
        <Header
          style={{
            padding: '0 24px',
            background: '#fff',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            boxShadow: '0 1px 4px rgba(0,0,0,0.08)',
            position: 'sticky',
            top: 0,
            zIndex: 10,
          }}
        >
          {isMobile && (
            <Space>
              <Button
                type="text"
                icon={<MenuOutlined style={{ fontSize: '20px' }} />}
                onClick={() => setMobileDrawerOpen(true)}
                aria-label="메뉴 열기"
              />
              <Title level={4} style={{ margin: 0 }}>
                10&10
              </Title>
            </Space>
          )}

              {!isMobile && (
                <Title level={4} style={{ margin: 0 }}>
                  {(() => {
                    const currentMenuItem = menuItems.find((item) => item && 'label' in item && item.key === selectedKey);
                    return currentMenuItem && 'label' in currentMenuItem ? currentMenuItem.label : '홈';
                  })()}
                </Title>
              )}

          <Dropdown menu={{ items: userMenuItems }} placement="bottomRight" arrow>
            <Button type="text" icon={<UserOutlined />} aria-label="사용자 메뉴">
              {!isMobile && <span>{user?.displayName}</span>}
            </Button>
          </Dropdown>
        </Header>

        {/* 컨텐츠 */}
        <Content
          style={{
            margin: 0,
            overflow: 'initial',
            minHeight: 'calc(100vh - 64px)',
          }}
          role="main"
          aria-label="메인 컨텐츠"
        >
          <Outlet />
        </Content>
      </AntLayout>

      {/* 모바일 드로어 */}
      {isMobile && (
        <Drawer
          title="10&10"
          placement="left"
          onClose={() => setMobileDrawerOpen(false)}
          open={mobileDrawerOpen}
          bodyStyle={{ padding: 0 }}
        >
          <div style={{ padding: '16px', borderBottom: '1px solid #f0f0f0' }}>
            <Space direction="vertical">
              <UserOutlined style={{ fontSize: '48px', color: '#1890ff' }} />
              <Text strong>{user?.displayName}</Text>
              <Text type="secondary" style={{ fontSize: '12px' }}>
                {user?.email}
              </Text>
            </Space>
          </div>
          <Menu
            mode="inline"
            selectedKeys={[selectedKey]}
            items={menuItems}
            style={{ border: 'none' }}
            role="navigation"
            aria-label="모바일 메뉴"
          />
          <div style={{ padding: '16px', borderTop: '1px solid #f0f0f0' }}>
            <Button type="primary" danger block icon={<LogoutOutlined />} onClick={handleLogout}>
              로그아웃
            </Button>
          </div>
        </Drawer>
      )}
    </AntLayout>
  );
}

