/**
 * 홈 페이지 (대시보드)
 */

import { Card } from 'antd';
import { useAuthStore } from '@stores/authStore';

const Home: React.FC = () => {
  const { user } = useAuthStore();

  return (
    <div style={{ padding: '24px' }}>
      <h1>환영합니다, {user?.displayName}님! 🎉</h1>
      <p>MeTenTen 웹앱에 오신 것을 환영합니다.</p>

      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
          gap: '16px',
          marginTop: '32px',
        }}
      >
        <Card title="나의 10&10" hoverable>
          <p>내가 작성한 10&10을 확인하고 관리합니다.</p>
        </Card>

        <Card title="배우자 10&10" hoverable>
          <p>배우자가 작성한 10&10을 확인합니다.</p>
        </Card>

        <Card title="느낌 표현" hoverable>
          <p>감정 표현 예시를 확인하고 관리합니다.</p>
        </Card>

        <Card title="기도문" hoverable>
          <p>부부를 위한 기도문을 확인합니다.</p>
        </Card>

        <Card title="설정" hoverable>
          <p>내 정보 및 배우자 관리를 합니다.</p>
        </Card>
      </div>
    </div>
  );
};

export default Home;

