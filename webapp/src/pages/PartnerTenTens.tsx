/**
 * 배우자 10&10 페이지
 * 배우자가 작성한 10&10 조회
 */

import { useEffect, useState } from 'react';
import {
  Card,
  Table,
  Space,
  Typography,
  Empty,
  Spin,
  App,
} from 'antd';
import { useAuthStore } from '@stores/authStore';
import { partnerService } from '@services/PartnerService';
import { topicService } from '@services/TopicService';
import PartnerTenTenView from '@components/PartnerTenTenView';
import { formatDate } from '@utils/date';
import type { Topic } from '../types/topic';
import { ERROR_MESSAGES } from '@utils/constants';

const { Title, Text } = Typography;

const PartnerTenTens: React.FC = () => {
  App.useApp();
  const { user } = useAuthStore();
  const [loading, setLoading] = useState(false);
  const [topics, setTopics] = useState<Topic[]>([]);
  const [selectedTopic, setSelectedTopic] = useState<Topic | null>(null);
  const [viewModalOpen, setViewModalOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const hasPartner = !!user?.partner;

  useEffect(() => {
    if (user && hasPartner) {
      loadPartnerTopics();
    }
  }, [user, hasPartner]);

  const loadPartnerTopics = async () => {
    if (!user?.partner) return;

    setLoading(true);
    setError(null);

    try {
      // 배우자가 작성 완료한 Topic ID 목록 조회
      const topicIds = await partnerService.getPartnerCompletedTopics();

      if (topicIds.length === 0) {
        setTopics([]);
        return;
      }

      // 각 Topic ID로 Topic 정보 조회
      const partnerUserId = user.partner!.partnerId;
      const topicPromises = topicIds.map(async (topicId) => {
        try {
          return await topicService.getTopicById(partnerUserId, topicId);
        } catch (error) {
          console.error(`Failed to load topic ${topicId}:`, error);
          return null;
        }
      });

      const loadedTopics = (await Promise.all(topicPromises)).filter(
        (topic): topic is Topic => topic !== null
      );

      setTopics(loadedTopics);
    } catch (error) {
      console.error('Load partner topics error:', error);
      if (error instanceof Error) {
        setError(error.message);
      } else {
        setError(ERROR_MESSAGES.LOAD_FAILED);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleViewTenTen = (topic: Topic) => {
    setSelectedTopic(topic);
    setViewModalOpen(true);
  };

  const handleViewClose = () => {
    setViewModalOpen(false);
    setSelectedTopic(null);
    // 모달 닫을 때 목록 새로고침 (선택적)
    // loadPartnerTopics();
  };

  if (!user) {
    return (
      <div style={{ padding: '24px', textAlign: 'center' }}>
        <Empty description="로그인이 필요합니다." />
      </div>
    );
  }

  if (!hasPartner) {
    return (
      <div style={{ padding: '24px' }}>
        <Title level={2}>배우자 10&10</Title>
        <Card>
          <Empty
            description="배우자가 연결되지 않았습니다."
            image={Empty.PRESENTED_IMAGE_SIMPLE}
          >
            <Text type="secondary">
              설정 페이지에서 배우자를 초대하여 연결할 수 있습니다.
            </Text>
          </Empty>
        </Card>
      </div>
    );
  }

  const columns = [
    {
      title: '주제',
      dataIndex: 'subject',
      key: 'subject',
      ellipsis: true,
      render: (text: string, record: Topic) => (
        <Typography.Text
          strong
          style={{
            color: '#1890ff',
            cursor: 'pointer',
          }}
          onClick={() => handleViewTenTen(record)}
        >
          {text}
        </Typography.Text>
      ),
    },
    {
      title: '날짜',
      dataIndex: 'topicDate',
      key: 'topicDate',
      width: 120,
      render: (date: string) => formatDate(new Date(date)),
    },
  ];

  return (
    <div style={{ padding: '24px', maxWidth: '1200px', margin: '0 auto' }}>
      <Title level={2}>배우자 10&10</Title>

      {/* 배우자 정보 */}
      {user.partner && (
        <Card style={{ marginBottom: '24px' }}>
          <Space direction="vertical" size="small">
            <div>
              <Text strong>배우자:</Text> <Text>{user.partner.partnerDisplayName}</Text>
            </div>
            <div>
              <Text strong>이메일:</Text> <Text>{user.partner.partnerEmail}</Text>
            </div>
          </Space>
        </Card>
      )}

      {/* Topic 목록 */}
      <Card>
        {error && (
          <div style={{ marginBottom: '16px', color: 'var(--error)' }}>
            {error}
          </div>
        )}

        <Spin spinning={loading}>
          {topics.length === 0 ? (
            <Empty
              description={
                loading
                  ? '로딩 중...'
                  : '배우자가 작성한 10&10이 없습니다.'
              }
            />
          ) : (
            <Table
              columns={columns}
              dataSource={topics}
              rowKey="firebaseKey"
              pagination={{
                pageSize: 10,
                showSizeChanger: true,
                showTotal: (total) => `총 ${total}개`,
              }}
            />
          )}
        </Spin>
      </Card>

      {/* 배우자 TenTen 읽기 모달 */}
      {selectedTopic && (
        <PartnerTenTenView
          open={viewModalOpen}
          onClose={handleViewClose}
          topic={selectedTopic}
        />
      )}
    </div>
  );
};

export default PartnerTenTens;
