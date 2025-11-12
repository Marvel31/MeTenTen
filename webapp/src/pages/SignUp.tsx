/**
 * 회원가입 페이지 (임시)
 */

import { Card, Button } from 'antd';
import { Link } from 'react-router-dom';
import { ROUTES } from '@config/routes';

const SignUp: React.FC = () => {
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
        title="회원가입"
        style={{ width: '100%', maxWidth: '400px' }}
        extra={
          <Link to={ROUTES.LOGIN}>
            <Button type="link">로그인</Button>
          </Link>
        }
      >
        <p>회원가입 페이지 (Phase 2에서 구현 예정)</p>
        <p style={{ marginTop: '16px', color: '#8c8c8c', fontSize: '12px' }}>
          현재는 임시 페이지입니다. Phase 2에서 실제 회원가입 기능이 구현됩니다.
        </p>
      </Card>
    </div>
  );
};

export default SignUp;

