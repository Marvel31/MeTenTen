/**
 * 비밀번호 변경 모달
 */

import { useState } from 'react';
import { Modal, Form, Input, App } from 'antd';
import { LockOutlined } from '@ant-design/icons';
import { authService } from '@services/AuthService';
import {
  validatePassword,
  validatePasswordConfirm,
} from '@utils/validation';
import { SUCCESS_MESSAGES } from '@utils/constants';

interface ChangePasswordModalProps {
  open: boolean;
  onClose: () => void;
}

interface ChangePasswordFormValues {
  currentPassword: string;
  newPassword: string;
  newPasswordConfirm: string;
}

const ChangePasswordModal: React.FC<ChangePasswordModalProps> = ({
  open,
  onClose,
}) => {
  const { message } = App.useApp();
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (values: ChangePasswordFormValues) => {
    setLoading(true);

    try {
      await authService.changePassword({
        currentPassword: values.currentPassword,
        newPassword: values.newPassword,
      });

      message.success(SUCCESS_MESSAGES.PASSWORD_CHANGED);
      form.resetFields();
      onClose();
    } catch (error) {
      if (error instanceof Error) {
        message.error(error.message);
      } else {
        message.error('비밀번호 변경에 실패했습니다.');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    form.resetFields();
    onClose();
  };

  return (
    <Modal
      title="비밀번호 변경"
      open={open}
      onOk={() => form.submit()}
      onCancel={handleCancel}
      confirmLoading={loading}
      okText="변경"
      cancelText="취소"
      destroyOnClose
    >
      <Form
        form={form}
        name="changePassword"
        onFinish={handleSubmit}
        layout="vertical"
        autoComplete="off"
      >
        <Form.Item
          name="currentPassword"
          label="현재 비밀번호"
          rules={[
            { required: true, message: '현재 비밀번호를 입력해주세요.' },
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
            placeholder="현재 비밀번호"
            disabled={loading}
          />
        </Form.Item>

        <Form.Item
          name="newPassword"
          label="새 비밀번호"
          rules={[
            { required: true, message: '새 비밀번호를 입력해주세요.' },
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
            placeholder="새 비밀번호 (최소 6자)"
            disabled={loading}
          />
        </Form.Item>

        <Form.Item
          name="newPasswordConfirm"
          label="새 비밀번호 확인"
          dependencies={['newPassword']}
          rules={[
            { required: true, message: '새 비밀번호 확인을 입력해주세요.' },
            ({ getFieldValue }) => ({
              validator(_, value) {
                const error = validatePasswordConfirm(
                  getFieldValue('newPassword'),
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
            placeholder="새 비밀번호 확인"
            disabled={loading}
          />
        </Form.Item>

        <div
          style={{
            padding: '12px',
            background: 'var(--background-gray)',
            borderRadius: '8px',
            fontSize: '12px',
            color: 'var(--text-secondary)',
          }}
        >
          <p style={{ margin: 0 }}>
            <strong>⚠️ 주의사항</strong>
          </p>
          <p style={{ margin: '8px 0 0 0' }}>
            비밀번호 변경 시 모든 암호화된 데이터는 새 비밀번호로 자동 재암호화됩니다.
            기존 데이터는 그대로 유지됩니다.
          </p>
        </div>
      </Form>
    </Modal>
  );
};

export default ChangePasswordModal;

