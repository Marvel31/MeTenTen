/**
 * 회원가입 페이지
 */

import { useState } from 'react';
import { Card, Form, Input, Button, App } from 'antd';
import { UserOutlined, LockOutlined, MailOutlined } from '@ant-design/icons';
import { Link, useNavigate } from 'react-router-dom';
import { authService } from '@services/AuthService';
import { ROUTES } from '@config/routes';
import {
  validateEmail,
  validatePassword,
  validatePasswordConfirm,
  validateDisplayName,
} from '@utils/validation';
import { SUCCESS_MESSAGES } from '@utils/constants';

interface SignUpFormValues {
  email: string;
  password: string;
  passwordConfirm: string;
  displayName: string;
}

const SignUp: React.FC = () => {
  const { message } = App.useApp();
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (values: SignUpFormValues) => {
    setLoading(true);

    try {
      await authService.signUp({
        email: values.email,
        password: values.password,
        displayName: values.displayName,
      });

      message.success(SUCCESS_MESSAGES.SIGNUP_SUCCESS);
      navigate(ROUTES.HOME);
    } catch (error) {
      if (error instanceof Error) {
        message.error(error.message);
      } else {
        message.error('회원가입에 실패했습니다.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        padding: '24px',
        background: 'var(--background-color)',
      }}
    >
      <Card
        title={
          <div style={{ textAlign: 'center', fontSize: '24px', fontWeight: 600 }}>
            회원가입
          </div>
        }
        style={{ width: '100%', maxWidth: '450px', boxShadow: 'var(--shadow-lg)' }}
      >
        <Form
          form={form}
          name="signup"
          onFinish={handleSubmit}
          layout="vertical"
          autoComplete="off"
        >
          <Form.Item
            name="email"
            label="이메일"
            rules={[
              { required: true, message: '이메일을 입력해주세요.' },
              {
                validator: (_, value) => {
                  const error = validateEmail(value);
                  return error ? Promise.reject(error) : Promise.resolve();
                },
              },
            ]}
          >
            <Input
              prefix={<MailOutlined />}
              placeholder="example@email.com"
              size="large"
              disabled={loading}
            />
          </Form.Item>

          <Form.Item
            name="displayName"
            label="이름"
            rules={[
              { required: true, message: '이름을 입력해주세요.' },
              {
                validator: (_, value) => {
                  const error = validateDisplayName(value);
                  return error ? Promise.reject(error) : Promise.resolve();
                },
              },
            ]}
          >
            <Input
              prefix={<UserOutlined />}
              placeholder="홍길동"
              size="large"
              disabled={loading}
            />
          </Form.Item>

          <Form.Item
            name="password"
            label="비밀번호"
            rules={[
              { required: true, message: '비밀번호를 입력해주세요.' },
              {
                validator: (_, value) => {
                  const error = validatePassword(value);
                  return error ? Promise.reject(error) : Promise.resolve();
                },
              },
            ]}
            hasFeedback
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="최소 6자 이상"
              size="large"
              disabled={loading}
            />
          </Form.Item>

          <Form.Item
            name="passwordConfirm"
            label="비밀번호 확인"
            dependencies={['password']}
            rules={[
              { required: true, message: '비밀번호 확인을 입력해주세요.' },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  const error = validatePasswordConfirm(
                    getFieldValue('password'),
                    value
                  );
                  return error ? Promise.reject(error) : Promise.resolve();
                },
              }),
            ]}
            hasFeedback
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="비밀번호를 다시 입력해주세요"
              size="large"
              disabled={loading}
            />
          </Form.Item>

          <Form.Item style={{ marginBottom: '12px' }}>
            <Button
              type="primary"
              htmlType="submit"
              size="large"
              loading={loading}
              block
            >
              회원가입
            </Button>
          </Form.Item>

          <div style={{ textAlign: 'center', color: 'var(--text-secondary)' }}>
            이미 계정이 있으신가요?{' '}
            <Link to={ROUTES.LOGIN}>
              <Button type="link" style={{ padding: 0 }}>
                로그인
              </Button>
            </Link>
          </div>
        </Form>

        <div
          style={{
            marginTop: '24px',
            padding: '16px',
            background: 'var(--background-gray)',
            borderRadius: '8px',
            fontSize: '12px',
            color: 'var(--text-secondary)',
          }}
        >
          <p style={{ margin: 0 }}>
            <strong>🔐 보안 안내</strong>
          </p>
          <p style={{ margin: '8px 0 0 0' }}>
            모든 데이터는 End-to-End 암호화되어 저장됩니다. 비밀번호를 잊어버리면
            데이터 복구가 불가능하니 안전하게 보관해주세요.
          </p>
        </div>
      </Card>
    </div>
  );
};

export default SignUp;

