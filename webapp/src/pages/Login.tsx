/**
 * 로그인 페이지
 */

import { useState } from 'react';
import { Card, Form, Input, Button, Checkbox, App } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { Link, useNavigate } from 'react-router-dom';
import { authService } from '@services/AuthService';
import { ROUTES } from '@config/routes';
import { validateEmail, validatePassword } from '@utils/validation';
import { SUCCESS_MESSAGES, STORAGE_KEYS } from '@utils/constants';
import { setStorage, getStorage } from '@utils/storage';

interface LoginFormValues {
  email: string;
  password: string;
  remember?: boolean;
}

const Login: React.FC = () => {
  const { message } = App.useApp();
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  // 저장된 이메일 불러오기
  useState(() => {
    const savedEmail = getStorage<string>(STORAGE_KEYS.USER_EMAIL);
    const rememberMe = getStorage<boolean>(STORAGE_KEYS.REMEMBER_ME);
    
    if (savedEmail && rememberMe) {
      form.setFieldsValue({ email: savedEmail, remember: true });
    }
  });

  const handleSubmit = async (values: LoginFormValues) => {
    setLoading(true);

    try {
      await authService.signIn({
        email: values.email,
        password: values.password,
      });

      // 이메일 저장 여부 처리
      if (values.remember) {
        setStorage(STORAGE_KEYS.USER_EMAIL, values.email);
        setStorage(STORAGE_KEYS.REMEMBER_ME, true);
      } else {
        setStorage(STORAGE_KEYS.USER_EMAIL, null);
        setStorage(STORAGE_KEYS.REMEMBER_ME, false);
      }

      message.success(SUCCESS_MESSAGES.LOGIN_SUCCESS);
      navigate(ROUTES.HOME);
    } catch (error) {
      if (error instanceof Error) {
        message.error(error.message);
      } else {
        message.error('로그인에 실패했습니다.');
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
            로그인
          </div>
        }
        style={{ width: '100%', maxWidth: '450px', boxShadow: 'var(--shadow-lg)' }}
      >
        <div
          style={{
            textAlign: 'center',
            marginBottom: '32px',
            color: 'var(--text-secondary)',
          }}
        >
          <h1 style={{ fontSize: '32px', margin: '0 0 8px 0', color: 'var(--primary-color)' }}>
            MeTenTen
          </h1>
          <p style={{ margin: 0, fontSize: '14px' }}>
            부부 소통을 위한 10&10 프로그램
          </p>
        </div>

        <Form
          form={form}
          name="login"
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
              prefix={<UserOutlined />}
              placeholder="example@email.com"
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
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="비밀번호"
              size="large"
              disabled={loading}
            />
          </Form.Item>

          <Form.Item>
            <Form.Item name="remember" valuePropName="checked" noStyle>
              <Checkbox disabled={loading}>이메일 기억하기</Checkbox>
            </Form.Item>
          </Form.Item>

          <Form.Item style={{ marginBottom: '12px' }}>
            <Button
              type="primary"
              htmlType="submit"
              size="large"
              loading={loading}
              block
            >
              로그인
            </Button>
          </Form.Item>

          <div style={{ textAlign: 'center', color: 'var(--text-secondary)' }}>
            계정이 없으신가요?{' '}
            <Link to={ROUTES.SIGNUP}>
              <Button type="link" style={{ padding: 0 }}>
                회원가입
              </Button>
            </Link>
          </div>
        </Form>
      </Card>
    </div>
  );
};

export default Login;

