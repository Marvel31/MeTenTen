/**
 * 로그인 페이지 (임시)
 */

import { Card, Button } from 'antd';
import { Link } from 'react-router-dom';
import { ROUTES } from '@config/routes';

const Login: React.FC = () => {
  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        padding: '24px',
      }}
    >
      <Card
        title="로그인"
        style={{ width: '100%', maxWidth: '400px' }}
        extra={
          <Link to={ROUTES.SIGNUP}>
            <Button type="link">회원가입</Button>
          </Link>
        }
      >
        <p>로그인 페이지 (Phase 2에서 구현 예정)</p>
        <p style={{ marginTop: '16px', color: '#8c8c8c', fontSize: '12px' }}>
          현재는 임시 페이지입니다. Phase 2에서 실제 로그인 기능이 구현됩니다.
        </p>
      </Card>
    </div>
  );
};

export default Login;

