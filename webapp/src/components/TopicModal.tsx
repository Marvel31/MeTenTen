/**
 * Topic 추가/수정 모달
 */

import { useState, useEffect } from 'react';
import { Modal, Form, Input, DatePicker, App } from 'antd';
import { topicService } from '@services/TopicService';
import type { Topic, CreateTopicRequest, UpdateTopicRequest } from '../types/topic';
import {
  validateTopicSubject,
  validateTopicDate,
} from '@utils/validation';
import { SUCCESS_MESSAGES } from '@utils/constants';
import { formatDate, getCurrentDate } from '@utils/date';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';

interface TopicModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
  topic?: Topic | null;
  userId: string;
}

interface TopicFormValues {
  subject: string;
  topicDate: Dayjs | null;
}

const TopicModal: React.FC<TopicModalProps> = ({
  open,
  onClose,
  onSuccess,
  topic,
  userId,
}) => {
  const { message } = App.useApp();
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const isEditMode = !!topic;

  // 모달이 열릴 때 폼 초기화
  useEffect(() => {
    if (open) {
      if (topic) {
        // 수정 모드: 기존 데이터로 초기화
        form.setFieldsValue({
          subject: topic.subject,
          topicDate: topic.topicDate ? dayjs(topic.topicDate) : dayjs(),
        });
      } else {
        // 추가 모드: 기본값으로 초기화
        form.setFieldsValue({
          subject: '',
          topicDate: dayjs(),
        });
      }
    }
  }, [open, topic, form]);

  const handleSubmit = async (values: TopicFormValues) => {
    setLoading(true);

    try {
      const topicDate = values.topicDate
        ? formatDate(values.topicDate.toDate())
        : getCurrentDate();

      if (isEditMode && topic) {
        // 수정
        const updateData: UpdateTopicRequest = {
          subject: values.subject,
          topicDate: topicDate,
        };

        await topicService.updateTopic(userId, topic.firebaseKey, updateData);
        message.success(SUCCESS_MESSAGES.TOPIC_UPDATED);
      } else {
        // 생성
        const createData: CreateTopicRequest = {
          subject: values.subject,
          topicDate: topicDate,
        };

        await topicService.createTopic(userId, createData);
        message.success(SUCCESS_MESSAGES.TOPIC_CREATED);
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
            ? '주제 수정에 실패했습니다.'
            : '주제 생성에 실패했습니다.'
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

  return (
    <Modal
      title={isEditMode ? '주제 수정' : '새 주제 추가'}
      open={open}
      onOk={() => form.submit()}
      onCancel={handleCancel}
      confirmLoading={loading}
      okText={isEditMode ? '수정' : '생성'}
      cancelText="취소"
      destroyOnClose
      width={600}
    >
      <Form
        form={form}
        name="topic"
        onFinish={handleSubmit}
        layout="vertical"
        autoComplete="off"
      >
        <Form.Item
          name="subject"
          label="주제"
          rules={[
            { required: true, message: '주제를 입력해주세요.' },
            {
              validator: (_, value) => {
                const error = validateTopicSubject(value);
                return error ? Promise.reject(error) : Promise.resolve();
              },
            },
          ]}
        >
          <Input
            placeholder="예: 오늘 하루 동안 가장 감사했던 일"
            size="large"
            disabled={loading}
          />
        </Form.Item>

        <Form.Item
          name="topicDate"
          label="날짜"
          rules={[
            { required: true, message: '날짜를 선택해주세요.' },
            {
              validator: (_, value) => {
                if (!value) {
                  return Promise.reject('날짜를 선택해주세요.');
                }
                const error = validateTopicDate(
                  formatDate(value.toDate())
                );
                return error ? Promise.reject(error) : Promise.resolve();
              },
            },
          ]}
        >
          <DatePicker
            format="YYYY-MM-DD"
            style={{ width: '100%' }}
            size="large"
            disabled={loading}
            placeholder="날짜 선택"
          />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default TopicModal;

