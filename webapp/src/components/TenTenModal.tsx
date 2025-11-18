/**
 * TenTen 작성/수정 모달
 */

import { useState, useEffect } from 'react';
import { Modal, Form, Input, Radio, App } from 'antd';
import { tenTenService } from '@services/TenTenService';
import Timer from './Timer';
import type {
  CreateTenTenRequest,
  UpdateTenTenRequest,
  DecryptedTenTen,
} from '../types/tenten';
import type { Topic } from '../types/topic';
import type { EncryptionType } from '../types/common';
import { validateTenTenContent } from '@utils/validation';
import { SUCCESS_MESSAGES } from '@utils/constants';
import { useAuthStore } from '@stores/authStore';

const { TextArea } = Input;

interface TenTenModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
  topic: Topic;
  tenten?: DecryptedTenTen | null;
  userId: string;
}

interface TenTenFormValues {
  content: string;
  encryptionType: EncryptionType;
}

const TenTenModal: React.FC<TenTenModalProps> = ({
  open,
  onClose,
  onSuccess,
  topic,
  tenten,
  userId,
}) => {
  const { message } = App.useApp();
  const { user } = useAuthStore();
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [isMobile, setIsMobile] = useState(false);

  const isEditMode = !!tenten;
  const hasPartner = !!user?.partner;
  const canUseShared = hasPartner && !isEditMode; // 수정 모드에서는 암호화 타입 변경 불가

  // 모바일 감지
  useEffect(() => {
    const checkMobile = () => {
      setIsMobile(window.innerWidth < 768);
    };
    checkMobile();
    window.addEventListener('resize', checkMobile);
    return () => window.removeEventListener('resize', checkMobile);
  }, []);

  // 모달이 열릴 때 폼 초기화
  useEffect(() => {
    if (open) {
      if (tenten) {
        // 수정 모드: 기존 데이터로 초기화
        form.setFieldsValue({
          content: tenten.content,
          encryptionType: tenten.encryptionType,
        });
      } else {
        // 작성 모드: 기본값으로 초기화
        form.setFieldsValue({
          content: '',
          encryptionType: hasPartner ? 'shared' : 'personal', // 배우자가 있으면 shared, 없으면 personal
        });
      }
    }
  }, [open, tenten, form, hasPartner]);

  const handleSubmit = async (values: TenTenFormValues) => {
    setLoading(true);

    try {
      if (isEditMode && tenten) {
        // 수정
        const updateData: UpdateTenTenRequest = {
          content: values.content,
        };

        await tenTenService.updateTenTen(
          userId,
          tenten.firebaseKey,
          updateData
        );
        message.success(SUCCESS_MESSAGES.TENTEN_UPDATED);
      } else {
        // 생성
        // 배우자가 없을 때는 기본값으로 'personal' 사용
        const encryptionType =
          values.encryptionType || (hasPartner ? 'shared' : 'personal');

        const createData: CreateTenTenRequest = {
          content: values.content,
          topicId: topic.firebaseKey,
          encryptionType: encryptionType as EncryptionType,
        };

        await tenTenService.createTenTen(
          userId,
          user?.displayName || '사용자',
          createData
        );
        message.success(SUCCESS_MESSAGES.TENTEN_CREATED);
      }

      form.resetFields();
      onSuccess();
      onClose();
    } catch (error) {
      if (error instanceof Error) {
        message.error(error.message);
      } else {
        message.error(
          isEditMode
            ? '10&10 수정에 실패했습니다.'
            : '10&10 작성에 실패했습니다.'
        );
      }
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    form.resetFields();
    onClose();
  };

  const handleTimerComplete = () => {
    message.info('10분이 지났습니다. 이제 대화를 나누어보세요! 💬');
  };

  return (
    <Modal
      title={
        isEditMode ? '10&10 수정' : `10&10 작성 - ${topic.subject}`
      }
      open={open}
      onOk={() => form.submit()}
      onCancel={handleCancel}
      confirmLoading={loading}
      okText={isEditMode ? '수정' : '저장'}
      cancelText="취소"
      destroyOnClose
      width={isMobile ? '95%' : 800}
      style={{ top: isMobile ? 10 : 20 }}
    >
      <div style={{ marginBottom: '24px' }}>
        {/* 타이머 (작성 모드에서만 표시) */}
        {!isEditMode && (
          <Timer
            initialMinutes={10}
            onComplete={handleTimerComplete}
            autoStart={false}
            showControls={true}
            compact={true}
          />
        )}

        {/* 주제 정보 */}
        <div
          style={{
            marginTop: '16px',
            padding: '12px',
            background: 'var(--background-light)',
            borderRadius: '8px',
          }}
        >
          <div style={{ fontWeight: 'bold', marginBottom: '4px' }}>
            주제
          </div>
          <div style={{ color: 'var(--text-secondary)' }}>{topic.subject}</div>
          <div
            style={{
              marginTop: '4px',
              fontSize: '12px',
              color: 'var(--text-tertiary)',
            }}
          >
            날짜: {topic.topicDate}
          </div>
        </div>
      </div>

      <Form
        form={form}
        name="tenten"
        onFinish={handleSubmit}
        layout="vertical"
        autoComplete="off"
        initialValues={{
          encryptionType: hasPartner ? 'shared' : 'personal',
        }}
      >
        {/* 암호화 타입 선택 (작성 모드에서만) */}
        {!isEditMode && (
          <Form.Item
            name="encryptionType"
            label={canUseShared ? '암호화 타입' : undefined}
            rules={[
              {
                required: true,
                message: '암호화 타입을 선택해주세요.',
              },
            ]}
            style={!canUseShared ? { display: 'none', margin: 0 } : undefined}
            hidden={!canUseShared}
          >
            {canUseShared ? (
              <Radio.Group>
                <Radio value="personal">개인 (나만 볼 수 있음)</Radio>
                <Radio value="shared">공유 (배우자도 볼 수 있음)</Radio>
              </Radio.Group>
            ) : (
              // 배우자가 없을 때는 값이 자동으로 'personal'로 유지됨 (initialValues)
              null
            )}
          </Form.Item>
        )}

        {/* 내용 입력 */}
        <Form.Item
          name="content"
          label="내용"
          rules={[
            { required: true, message: '내용을 입력해주세요.' },
            {
              validator: (_, value) => {
                const error = validateTenTenContent(value);
                return error ? Promise.reject(error) : Promise.resolve();
              },
            },
          ]}
        >
          <TextArea
            placeholder="10분 동안 편지를 작성해보세요. 솔직하고 진심으로..."
            rows={12}
            disabled={loading}
            showCount
            maxLength={10000}
            style={{
              fontSize: '14px',
              lineHeight: 1.6,
              resize: 'vertical',
            }}
          />
        </Form.Item>

        {/* 안내 메시지 */}
        {!isEditMode && (
          <div
            style={{
              marginTop: '8px',
              padding: '12px',
              background: '#f0f5ff',
              borderRadius: '6px',
              fontSize: '12px',
              color: '#595959',
            }}
          >
            <div>💡 <strong>10&10 프로그램 안내</strong></div>
            <div style={{ marginTop: '4px' }}>
              10분간 편지를 작성하고, 10분간 대화를 나누는 시간입니다.
              솔직하고 진심으로 여러분의 마음을 표현해보세요.
            </div>
          </div>
        )}
      </Form>
    </Modal>
  );
};

export default TenTenModal;

