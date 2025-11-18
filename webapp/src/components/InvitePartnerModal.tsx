/**
 * 배우자 초대 모달
 */

import { useState } from 'react';
import { Modal, Form, Input, App } from 'antd';
import { partnerService } from '@services/PartnerService';
import { validateEmail } from '@utils/validation';

interface InvitePartnerModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

interface InvitePartnerFormValues {
  partnerEmail: string;
  myPassword: string;
}

const InvitePartnerModal: React.FC<InvitePartnerModalProps> = ({
  open,
  onClose,
  onSuccess,
}) => {
  const { message } = App.useApp();
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (values: InvitePartnerFormValues) => {
    setLoading(true);

    try {
      await partnerService.invitePartner(
        values.partnerEmail,
        values.myPassword
      );
      message.success('배우자 초대가 완료되었습니다.');
      form.resetFields();
      onSuccess();
      onClose();
    } catch (error) {
      if (error instanceof Error) {
        message.error(error.message);
      } else {
        message.error('배우자 초대에 실패했습니다.');
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
      title="배우자 초대"
      open={open}
      onOk={() => form.submit()}
      onCancel={handleCancel}
      confirmLoading={loading}
      okText="초대"
      cancelText="취소"
      destroyOnClose
      width={500}
    >
      <div
        style={{
          marginBottom: '16px',
          padding: '12px',
          background: '#f0f5ff',
          borderRadius: '6px',
          fontSize: '13px',
          color: '#595959',
        }}
      >
        <div>
          💡 <strong>배우자 초대 안내</strong>
        </div>
        <div style={{ marginTop: '4px' }}>
          배우자의 이메일 주소를 입력하고 본인 비밀번호를 확인해주세요.
          초대가 완료되면 배우자도 로그인하면 자동으로 연결됩니다.
        </div>
      </div>

      <Form
        form={form}
        name="invitePartner"
        onFinish={handleSubmit}
        layout="vertical"
        autoComplete="off"
      >
        <Form.Item
          name="partnerEmail"
          label="배우자 이메일"
          rules={[
            { required: true, message: '배우자 이메일을 입력해주세요.' },
            {
              validator: (_, value) => {
                if (!value) {
                  return Promise.resolve();
                }
                const error = validateEmail(value);
                if (error) {
                  return Promise.reject(new Error(error));
                }
                return Promise.resolve();
              },
            },
          ]}
        >
          <Input
            placeholder="partner@example.com"
            disabled={loading}
            autoComplete="email"
          />
        </Form.Item>

        <Form.Item
          name="myPassword"
          label="본인 비밀번호 확인"
          rules={[
            { required: true, message: '본인 비밀번호를 입력해주세요.' },
            {
              min: 6,
              message: '비밀번호는 최소 6자 이상이어야 합니다.',
            },
          ]}
        >
          <Input.Password
            placeholder="본인 비밀번호를 입력해주세요"
            disabled={loading}
            autoComplete="current-password"
          />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default InvitePartnerModal;

