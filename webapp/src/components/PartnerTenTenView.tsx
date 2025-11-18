/**
 * 배우자 TenTen 읽기 모달
 */

import { useState, useEffect } from 'react';
import { Modal, Typography, Spin, Empty, App, Card, Space } from 'antd';
import { useAuthStore } from '@stores/authStore';
import { partnerService } from '@services/PartnerService';
import { formatDate } from '@utils/date';
import type { Topic } from '../types/topic';
import { ERROR_MESSAGES } from '@utils/constants';

const { Paragraph } = Typography;

interface PartnerTenTenViewProps {
  open: boolean;
  onClose: () => void;
  topic: Topic;
}

interface DecryptedTenTen {
  content: string;
  createdAt: string;
  updatedAt?: string;
}

const PartnerTenTenView: React.FC<PartnerTenTenViewProps> = ({
  open,
  onClose,
  topic,
}) => {
  const { message } = App.useApp();
  const { user } = useAuthStore();
  const [loading, setLoading] = useState(false);
  const [tentens, setTentens] = useState<DecryptedTenTen[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open && topic) {
      loadPartnerTenTens();
    } else {
      setTentens([]);
      setError(null);
    }
  }, [open, topic]);

  const loadPartnerTenTens = async () => {
    if (!user || !topic) return;

    setLoading(true);
    setError(null);

    try {
      const decryptedTenTens = await partnerService.getPartnerTenTens(
        topic.firebaseKey
      );

      setTentens(
        decryptedTenTens.map((tenten) => ({
          content: tenten.content,
          createdAt: tenten.createdAt,
          updatedAt: tenten.updatedAt,
        }))
      );
    } catch (error) {
      console.error('Load partner tentens error:', error);
      if (error instanceof Error) {
        setError(error.message);
        message.error(error.message);
      } else {
        setError(ERROR_MESSAGES.LOAD_FAILED);
        message.error(ERROR_MESSAGES.LOAD_FAILED);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      title={`${topic.subject} - 배우자 10&10`}
      open={open}
      onCancel={onClose}
      footer={null}
      destroyOnClose
      width={800}
      style={{ top: 20 }}
    >
      {/* 주제 정보 */}
      <div
        style={{
          marginBottom: '24px',
          padding: '12px',
          background: 'var(--background-light)',
          borderRadius: '8px',
        }}
      >
        <div style={{ fontWeight: 'bold', marginBottom: '4px' }}>주제</div>
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

      {/* TenTen 목록 */}
      <Spin spinning={loading}>
        {error ? (
          <Empty
            description={error}
            image={Empty.PRESENTED_IMAGE_SIMPLE}
          />
        ) : tentens.length === 0 ? (
          <Empty
            description={
              loading
                ? '로딩 중...'
                : '배우자가 작성한 10&10이 없습니다.'
            }
          />
        ) : (
          <Space direction="vertical" size="large" style={{ width: '100%' }}>
            {tentens.map((tenten, index) => (
              <Card key={index} size="small">
                <Paragraph
                  style={{
                    whiteSpace: 'pre-wrap',
                    wordBreak: 'break-word',
                    fontSize: '15px',
                    lineHeight: 1.8,
                    margin: 0,
                  }}
                >
                  {tenten.content}
                </Paragraph>
                <div
                  style={{
                    marginTop: '12px',
                    fontSize: '12px',
                    color: 'var(--text-tertiary)',
                  }}
                >
                  작성일:{' '}
                  {formatDate(new Date(tenten.createdAt))}
                  {tenten.updatedAt &&
                    tenten.updatedAt !== tenten.createdAt && (
                      <>
                        {' · '}
                        수정일:{' '}
                        {formatDate(
                          new Date(tenten.updatedAt)
                        )}
                      </>
                    )}
                </div>
              </Card>
            ))}
          </Space>
        )}
      </Spin>
    </Modal>
  );
};

export default PartnerTenTenView;

