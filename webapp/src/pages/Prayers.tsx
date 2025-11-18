/**
 * 기도문 페이지
 */

import { useState, useEffect } from 'react';
import { Card, Collapse, Button, Modal, App, Space, Typography, Tag } from 'antd';
import { CopyOutlined, StarOutlined, StarFilled } from '@ant-design/icons';
import type { Prayer, PrayerCategory } from '../types/prayer';
import { prayerService } from '@services/PrayerService';

const { Panel } = Collapse;
const { Title, Text, Paragraph } = Typography;

export default function Prayers() {
  const { message, modal } = App.useApp();

  const [categories, setCategories] = useState<PrayerCategory[]>([]);
  const [selectedPrayer, setSelectedPrayer] = useState<Prayer | null>(null);
  const [detailModalOpen, setDetailModalOpen] = useState(false);

  // 데이터 로드
  const loadData = async () => {
    try {
      const data = await prayerService.getCategories();
      setCategories(data);
    } catch (error) {
      console.error('Load prayers error:', error);
      message.error('기도문을 불러오는데 실패했습니다.');
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  // 기도문 상세 보기
  const handleViewDetail = async (prayer: Prayer) => {
    setSelectedPrayer(prayer);
    setDetailModalOpen(true);
    // 조회수 증가
    await prayerService.incrementViewCount(prayer.id);
    loadData(); // 조회수 업데이트를 위해 다시 로드
  };

  // 클립보드 복사
  const handleCopyToClipboard = async (prayer: Prayer) => {
    try {
      const text = `${prayer.title}\n\n${prayer.content}`;
      await navigator.clipboard.writeText(text);
      message.success('기도문이 클립보드에 복사되었습니다.');
    } catch (error) {
      console.error('Copy to clipboard error:', error);
      message.error('복사에 실패했습니다.');
    }
  };

  // 즐겨찾기 토글
  const handleToggleFavorite = async (prayer: Prayer) => {
    try {
      await prayerService.toggleFavorite(prayer.id);
      message.success(prayer.isFavorite ? '즐겨찾기에서 제거되었습니다.' : '즐겨찾기에 추가되었습니다.');
      loadData();
      // 상세 모달이 열려있으면 업데이트
      if (selectedPrayer?.id === prayer.id) {
        const updatedPrayer = await prayerService.getPrayerById(prayer.id);
        setSelectedPrayer(updatedPrayer);
      }
    } catch (error) {
      console.error('Toggle favorite error:', error);
      message.error('즐겨찾기 처리에 실패했습니다.');
    }
  };

  // 초기화
  const handleReset = () => {
    modal.confirm({
      title: '기본값으로 초기화',
      content: '모든 기도문 데이터가 초기화됩니다. 계속하시겠습니까?',
      okText: '초기화',
      okType: 'danger',
      cancelText: '취소',
      onOk: async () => {
        try {
          await prayerService.resetToDefault();
          message.success('기본값으로 초기화되었습니다.');
          loadData();
        } catch (error) {
          console.error('Reset prayers error:', error);
          message.error('초기화에 실패했습니다.');
        }
      },
    });
  };

  return (
    <div style={{ padding: '24px' }}>
      <Card>
        <Space direction="vertical" size="large" style={{ width: '100%' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Title level={2} style={{ margin: 0 }}>
              기도문
            </Title>
            <Button onClick={handleReset}>기본값으로 초기화</Button>
          </div>

          <Text type="secondary">
            10&10 시간과 일상에서 사용할 수 있는 다양한 기도문을 제공합니다. 기도문을 클릭하여 전체 내용을 확인하고 복사할 수
            있습니다.
          </Text>

          <Collapse defaultActiveKey={['부부']}>
            {categories.map((category) => (
              <Panel
                header={
                  <span style={{ fontSize: '18px' }}>
                    {category.name} ({category.prayers.length})
                  </span>
                }
                key={category.id}
              >
                <Space direction="vertical" size="middle" style={{ width: '100%' }}>
                  {category.prayers.map((prayer) => (
                    <Card
                      key={prayer.id}
                      size="small"
                      hoverable
                      onClick={() => handleViewDetail(prayer)}
                      style={{ cursor: 'pointer' }}
                    >
                      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                        <div style={{ flex: 1 }}>
                          <Space>
                            <Text strong style={{ fontSize: '16px' }}>
                              {prayer.title}
                            </Text>
                            {prayer.isFavorite && <StarFilled style={{ color: '#faad14' }} />}
                          </Space>
                          <div style={{ marginTop: '8px' }}>
                            <Text type="secondary" style={{ fontSize: '14px' }}>
                              {prayer.content.split('\n')[0]}
                              {prayer.content.split('\n').length > 1 && '...'}
                            </Text>
                          </div>
                          {prayer.tags && prayer.tags.length > 0 && (
                            <div style={{ marginTop: '8px' }}>
                              {prayer.tags.map((tag) => (
                                <Tag key={tag} color="blue" style={{ marginBottom: '4px' }}>
                                  {tag}
                                </Tag>
                              ))}
                            </div>
                          )}
                        </div>
                        <div style={{ marginLeft: '16px' }}>
                          <Text type="secondary" style={{ fontSize: '12px' }}>
                            조회 {prayer.viewCount}
                          </Text>
                        </div>
                      </div>
                    </Card>
                  ))}
                </Space>
              </Panel>
            ))}
          </Collapse>
        </Space>
      </Card>

      {/* 기도문 상세 모달 */}
      <Modal
        title={
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <span>{selectedPrayer?.title}</span>
            {selectedPrayer && (
              <Space>
                <Button
                  type="text"
                  icon={selectedPrayer.isFavorite ? <StarFilled style={{ color: '#faad14' }} /> : <StarOutlined />}
                  onClick={(e) => {
                    e.stopPropagation();
                    handleToggleFavorite(selectedPrayer);
                  }}
                >
                  즐겨찾기
                </Button>
                <Button
                  type="primary"
                  icon={<CopyOutlined />}
                  onClick={(e) => {
                    e.stopPropagation();
                    handleCopyToClipboard(selectedPrayer);
                  }}
                >
                  복사
                </Button>
              </Space>
            )}
          </div>
        }
        open={detailModalOpen}
        onCancel={() => {
          setDetailModalOpen(false);
          setSelectedPrayer(null);
        }}
        footer={null}
        width={600}
      >
        {selectedPrayer && (
          <Space direction="vertical" size="middle" style={{ width: '100%' }}>
            {selectedPrayer.tags && selectedPrayer.tags.length > 0 && (
              <div>
                {selectedPrayer.tags.map((tag) => (
                  <Tag key={tag} color="blue">
                    {tag}
                  </Tag>
                ))}
              </div>
            )}
            <Paragraph style={{ whiteSpace: 'pre-line', fontSize: '16px', lineHeight: '1.8' }}>
              {selectedPrayer.content}
            </Paragraph>
            <div style={{ textAlign: 'right' }}>
              <Text type="secondary" style={{ fontSize: '12px' }}>
                조회 {selectedPrayer.viewCount}회
              </Text>
            </div>
          </Space>
        )}
      </Modal>
    </div>
  );
}
