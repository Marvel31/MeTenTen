/**
 * 404 페이지
 */

import { Result, Button } from 'antd';
import { useNavigate } from 'react-router-dom';
import { ROUTES } from '@config/routes';

const NotFound: React.FC = () => {
  const navigate = useNavigate();

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
      <Result
        status="404"
        title="404"
        subTitle="죄송합니다. 요청하신 페이지를 찾을 수 없습니다."
        extra={
          <Button type="primary" onClick={() => navigate(ROUTES.HOME)}>
            홈으로 돌아가기
          </Button>
        }
      />
    </div>
  );
};

export default NotFound;

