/**
 * 홈 페이지 (대시보드)
 */

import { Card, Button, message } from 'antd';
import { LogoutOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '@stores/authStore';
import { authService } from '@services/AuthService';
import { SUCCESS_MESSAGES } from '@utils/constants';
import { ROUTES } from '@config/routes';

const Home: React.FC = () => {
  const { user } = useAuthStore();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      await authService.signOut();
      message.success(SUCCESS_MESSAGES.LOGOUT_SUCCESS);
      navigate(ROUTES.LOGIN);
    } catch (error) {
      message.error('로그아웃에 실패했습니다.');
    }
  };

  return (
    <div style={{ padding: '24px', maxWidth: '1200px', margin: '0 auto' }}>
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: '32px',
        }}
      >
        <div>
          <h1 style={{ margin: 0, fontSize: '32px' }}>
            환영합니다, {user?.displayName}님! 🎉
          </h1>
          <p style={{ margin: '8px 0 0 0', color: 'var(--text-secondary)' }}>
            MeTenTen 웹앱에 오신 것을 환영합니다.
          </p>
        </div>
        <Button
          icon={<LogoutOutlined />}
          onClick={handleLogout}
          size="large"
        >
          로그아웃
        </Button>
      </div>

      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
          gap: '16px',
        }}
      >
        <Card
          title="나의 10&10"
          hoverable
          onClick={() => navigate(ROUTES.MY_TENTENS)}
          style={{ cursor: 'pointer' }}
        >
          <p>내가 작성한 10&10을 확인하고 관리합니다.</p>
        </Card>

        <Card
          title="배우자 10&10"
          hoverable
          onClick={() => navigate(ROUTES.PARTNER_TENTENS)}
          style={{ cursor: 'pointer' }}
        >
          <p>배우자가 작성한 10&10을 확인합니다.</p>
        </Card>

        <Card
          title="느낌 표현"
          hoverable
          onClick={() => navigate(ROUTES.FEELINGS)}
          style={{ cursor: 'pointer' }}
        >
          <p>감정 표현 예시를 확인하고 관리합니다.</p>
        </Card>

        <Card
          title="기도문"
          hoverable
          onClick={() => navigate(ROUTES.PRAYERS)}
          style={{ cursor: 'pointer' }}
        >
          <p>부부를 위한 기도문을 확인합니다.</p>
        </Card>

        <Card
          title="설정"
          hoverable
          onClick={() => navigate(ROUTES.SETTINGS)}
          style={{ cursor: 'pointer' }}
        >
          <p>내 정보 및 배우자 관리를 합니다.</p>
        </Card>
      </div>

      <div
        style={{
          marginTop: '48px',
          padding: '24px',
          background: 'var(--background-white)',
          borderRadius: '12px',
          boxShadow: 'var(--shadow-sm)',
        }}
      >
        <h2 style={{ margin: '0 0 16px 0', fontSize: '20px' }}>
          Marriage Encounter 10&10 프로그램
        </h2>
        <p style={{ margin: 0, lineHeight: 1.6, color: 'var(--text-secondary)' }}>
          10&10 프로그램은 10분간 편지를 쓰고 10분간 대화를 나누는 시간을 통해
          부부가 서로의 마음을 깊이 이해할 수 있도록 돕는 프로그램입니다.
          매일 꾸준히 실천하여 부부 간의 소통과 친밀감을 증진시켜보세요.
        </p>
      </div>
    </div>
  );
};

export default Home;

