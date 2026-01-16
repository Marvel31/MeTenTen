/**
 * 나의 10&10 페이지
 * Topic 관리 및 TenTen 작성
 */

import { useEffect, useState, useMemo, useCallback } from 'react';
import {
  Card,
  Table,
  Button,
  Space,
  Select,
  DatePicker,
  Input,
  App,
  Popconfirm,
  Pagination,
  Typography,
  Empty,
  Spin,
} from 'antd';
import {
  PlusOutlined,
  EditOutlined,
  DeleteOutlined,
  SearchOutlined,
} from '@ant-design/icons';
import { useAuthStore } from '@stores/authStore';
import { useTopicStore } from '@stores/topicStore';
import TopicModal from '@components/TopicModal';
import TenTenModal from '@components/TenTenModal';
import { topicService } from '@services/TopicService';
import { tenTenService } from '@services/TenTenService';
import type { Topic } from '../types/topic';
import type { DecryptedTenTen } from '../types/tenten';
import { formatDate } from '@utils/date';
import { ERROR_MESSAGES } from '@utils/constants';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';

const { Option } = Select;
const { Title } = Typography;
const { MonthPicker } = DatePicker;

const MyTenTens: React.FC = () => {
  const { message } = App.useApp();
  const { user } = useAuthStore();
  const {
    topics,
    loading,
    error,
    filter,
    sortBy,
    sortOrder,
    currentPage,
    pageSize,
    loadTopics,
    refreshTopics,
    setFilter,
    setSortBy,
    setSortOrder,
    setCurrentPage,
    setPageSize,
  } = useTopicStore();

  const [topicModalOpen, setTopicModalOpen] = useState(false);
  const [tentenModalOpen, setTenTenModalOpen] = useState(false);
  const [selectedTopic, setSelectedTopic] = useState<Topic | null>(null);
  const [selectedTenTen, setSelectedTenTen] = useState<DecryptedTenTen | null>(
    null
  );
  const [searchText, setSearchText] = useState('');
  const [dateFilter, setDateFilter] = useState<{
    year: number;
    month: number;
  } | null>(null);
  const [isMobile, setIsMobile] = useState(false);

  const userId = user?.uid || '';

  // 모바일 감지
  useEffect(() => {
    const checkMobile = () => {
      setIsMobile(window.innerWidth < 768);
    };
    checkMobile();
    window.addEventListener('resize', checkMobile);
    return () => window.removeEventListener('resize', checkMobile);
  }, []);

  // 초기 로딩
  useEffect(() => {
    if (userId) {
      loadTopics(userId);
    }
  }, [userId, loadTopics]);

  // 필터 또는 정렬 변경 시 다시 로딩
  useEffect(() => {
    if (userId) {
      loadTopics(userId);
    }
  }, [userId, filter, sortBy, sortOrder, loadTopics]);

  // 검색어로 필터링
  const filteredTopics = topics.filter((topic) => {
    if (!searchText) {
      return true;
    }
    const searchLower = searchText.toLowerCase();
    return topic.subject.toLowerCase().includes(searchLower);
  });

  // 페이지네이션 적용
  const paginatedTopics = filteredTopics.slice(
    (currentPage - 1) * pageSize,
    currentPage * pageSize
  );

  // 총 개수 (검색 필터 적용)
  const displayTotal = filteredTopics.length;

  const handleCreateTopic = () => {
    setSelectedTopic(null);
    setTopicModalOpen(true);
  };

  const handleEditTopic = useCallback((topic: Topic) => {
    setSelectedTopic(topic);
    setTopicModalOpen(true);
  }, []);

  const handleWriteTenTen = useCallback(async (topic: Topic) => {
    // 해당 Topic의 TenTen이 있는지 확인
    const existingTenTens = await tenTenService.getTenTensByTopic(
      userId,
      topic.firebaseKey
    );
    if (existingTenTens.length > 0) {
      // 기존 TenTen이 있으면 수정 모드
      setSelectedTenTen(existingTenTens[0]);
      setSelectedTopic(topic);
      setTenTenModalOpen(true);
    } else {
      // 기존 TenTen이 없으면 작성 모드
      setSelectedTenTen(null);
      setSelectedTopic(topic);
      setTenTenModalOpen(true);
    }
  }, [userId]);

  const handleDelete = useCallback(async (topic: Topic) => {
    try {
      await topicService.deleteTopic(userId, topic.firebaseKey);
      message.success('주제가 삭제되었습니다.');
      refreshTopics(userId);
    } catch (error) {
      message.error(
        error instanceof Error
          ? error.message
          : ERROR_MESSAGES.DELETE_FAILED
      );
    }
  }, [userId, refreshTopics, message]);

  const handleTopicModalSuccess = () => {
    refreshTopics(userId);
  };

  const handleTenTenModalSuccess = () => {
    refreshTopics(userId);
  };

  const handleDateFilterChange = (date: Dayjs | null) => {
    if (date) {
      const year = date.year();
      const month = date.month() + 1;
      setDateFilter({ year, month });
      setFilter({
        year,
        month,
      });
    } else {
      setDateFilter(null);
      setFilter(null);
    }
  };


  const handleSortChange = (value: string) => {
    const [newSortBy, newSortOrder] = value.split('-') as [
      'date' | 'createdAt',
      'asc' | 'desc'
    ];
    setSortBy(newSortBy);
    setSortOrder(newSortOrder);
  };

  const columns = useMemo(() => [
    {
      title: '날짜',
      dataIndex: 'topicDate',
      key: 'topicDate',
      width: 120,
      render: (date: string) => formatDate(date),
    },
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
          onClick={() => handleWriteTenTen(record)}
        >
          {text}
        </Typography.Text>
      ),
    },
    {
      title: '작업',
      key: 'actions',
      width: isMobile ? 100 : 120,
      render: (_: unknown, record: Topic) => (
        <Space size="small" wrap>
          <Button
            type="link"
            icon={!isMobile ? <EditOutlined /> : undefined}
            onClick={() => handleEditTopic(record)}
            size="small"
            style={{ padding: isMobile ? '4px 8px' : undefined }}
          >
            {isMobile ? '✏️' : '수정'}
          </Button>
          <Popconfirm
            title="주제 삭제"
            description="정말 삭제하시겠습니까?"
            onConfirm={() => handleDelete(record)}
            okText="삭제"
            cancelText="취소"
          >
            <Button
              type="link"
              danger
              icon={!isMobile ? <DeleteOutlined /> : undefined}
              size="small"
              style={{ padding: isMobile ? '4px 8px' : undefined }}
            >
              {isMobile ? '🗑️' : '삭제'}
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ], [isMobile, handleWriteTenTen, handleEditTopic, handleDelete]);

  if (!userId) {
    return (
      <div style={{ padding: '24px', textAlign: 'center' }}>
        <Empty description="로그인이 필요합니다." />
      </div>
    );
  }

  return (
    <div style={{ padding: '24px', maxWidth: '1400px', margin: '0 auto' }}>
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: '24px',
        }}
      >
        <Title level={2} style={{ margin: 0 }}>
          나의 10&10
        </Title>
        <Space size="middle">
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={handleCreateTopic}
            size="large"
          >
            새 주제 추가
          </Button>
        </Space>
      </div>

      {/* 필터 영역 */}
      <Card style={{ marginBottom: '16px' }}>
        <Space wrap size="middle" style={{ width: '100%' }}>
          <Input
            placeholder="주제 검색"
            prefix={<SearchOutlined />}
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)}
            style={{
              width: isMobile ? '100%' : 250,
              maxWidth: '100%',
            }}
            allowClear
          />

          <MonthPicker
            placeholder="월 선택"
            value={
              dateFilter ? dayjs(`${dateFilter.year}-${dateFilter.month}`) : null
            }
            onChange={handleDateFilterChange}
            format="YYYY-MM"
            allowClear
          />

          <Select
            placeholder="정렬"
            value={`${sortBy}-${sortOrder}`}
            onChange={handleSortChange}
            style={{ width: 150 }}
          >
            <Option value="date-desc">날짜 최신순</Option>
            <Option value="date-asc">날짜 오래된순</Option>
            <Option value="createdAt-desc">생성일 최신순</Option>
            <Option value="createdAt-asc">생성일 오래된순</Option>
          </Select>
        </Space>
      </Card>

      {/* 테이블 영역 */}
      <Card>
        {error && (
          <div style={{ marginBottom: '16px', color: 'var(--error)' }}>
            {error}
          </div>
        )}

        <Spin spinning={loading}>
          {paginatedTopics.length === 0 ? (
            <Empty
              description={
                searchText || filter
                  ? '검색 결과가 없습니다.'
                  : '아직 등록된 주제가 없습니다.'
              }
            />
          ) : (
            <>
              <Table
                columns={columns}
                dataSource={paginatedTopics}
                rowKey="firebaseKey"
                pagination={false}
                size="middle"
                scroll={{ x: isMobile ? 'max-content' : undefined }}
              />

              <div
                style={{
                  marginTop: '16px',
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                }}
              >
                <div>
                  총 {displayTotal}개 중{' '}
                  {(currentPage - 1) * pageSize + 1}-
                  {Math.min(currentPage * pageSize, displayTotal)}개 표시
                </div>
                <Pagination
                  current={currentPage}
                  pageSize={pageSize}
                  total={displayTotal}
                  onChange={(page) => setCurrentPage(page)}
                  onShowSizeChange={(_, size) => {
                    setPageSize(size);
                    setCurrentPage(1);
                  }}
                  showSizeChanger
                  showTotal={(total) => `총 ${total}개`}
                  pageSizeOptions={['10', '20', '50', '100']}
                />
              </div>
            </>
          )}
        </Spin>
      </Card>

      {/* Topic Modal */}
      <TopicModal
        open={topicModalOpen}
        onClose={() => {
          setTopicModalOpen(false);
          setSelectedTopic(null);
        }}
        onSuccess={handleTopicModalSuccess}
        topic={selectedTopic}
        userId={userId}
      />

      {/* TenTen Modal */}
      {selectedTopic && (
        <TenTenModal
          open={tentenModalOpen}
          onClose={() => {
            setTenTenModalOpen(false);
            setSelectedTenTen(null);
            setSelectedTopic(null);
          }}
          onSuccess={handleTenTenModalSuccess}
          topic={selectedTopic}
          tenten={selectedTenTen}
          userId={userId}
        />
      )}
    </div>
  );
};

export default MyTenTens;
