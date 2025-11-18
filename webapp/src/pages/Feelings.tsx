/**
 * 느낌 표현 페이지
 */

import { useState, useEffect } from 'react';
import { Card, Collapse, List, Button, Modal, Form, Input, Select, App, Space, Typography } from 'antd';
import { PlusOutlined, DeleteOutlined } from '@ant-design/icons';
import type { FeelingCategoryInfo, FeelingExample, FeelingCategory } from '../types/feeling';
import { feelingExampleService } from '@services/FeelingExampleService';

const { Panel } = Collapse;
const { Title, Text } = Typography;

export default function Feelings() {
  const { message, modal } = App.useApp();
  const [form] = Form.useForm();

  const [categoryList, setCategoryList] = useState<FeelingCategoryInfo[]>([]);
  const [loading, setLoading] = useState(false);
  const [addModalOpen, setAddModalOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<FeelingCategory | undefined>(undefined);

  // 데이터 로드
  const loadData = async () => {
    setLoading(true);
    try {
      const data = await feelingExampleService.getCategoryInfoList();
      setCategoryList(data);
    } catch (error) {
      console.error('Load feeling examples error:', error);
      message.error('감정 예시를 불러오는데 실패했습니다.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  // 예시 추가 모달 열기
  const handleOpenAddModal = (category?: FeelingCategory) => {
    setSelectedCategory(category);
    if (category) {
      form.setFieldsValue({ category });
    }
    setAddModalOpen(true);
  };

  // 예시 추가
  const handleAddExample = async (values: { category: FeelingCategory; subCategory: string; description: string }) => {
    try {
      await feelingExampleService.createExample(values);
      message.success('감정 예시가 추가되었습니다.');
      setAddModalOpen(false);
      form.resetFields();
      loadData();
    } catch (error) {
      console.error('Add feeling example error:', error);
      message.error('감정 예시 추가에 실패했습니다.');
    }
  };

  // 예시 삭제
  const handleDeleteExample = (example: FeelingExample) => {
    if (example.isDefault) {
      message.warning('기본 감정 예시는 삭제할 수 없습니다.');
      return;
    }

    modal.confirm({
      title: '감정 예시 삭제',
      content: `"${example.subCategory}"를 삭제하시겠습니까?`,
      okText: '삭제',
      okType: 'danger',
      cancelText: '취소',
      onOk: async () => {
        try {
          await feelingExampleService.deleteExample(example.id);
          message.success('감정 예시가 삭제되었습니다.');
          loadData();
        } catch (error) {
          console.error('Delete feeling example error:', error);
          message.error('감정 예시 삭제에 실패했습니다.');
        }
      },
    });
  };

  // 초기화
  const handleReset = () => {
    modal.confirm({
      title: '기본값으로 초기화',
      content: '모든 사용자 정의 감정 예시가 삭제되고 기본 예시만 남습니다. 계속하시겠습니까?',
      okText: '초기화',
      okType: 'danger',
      cancelText: '취소',
      onOk: async () => {
        try {
          await feelingExampleService.resetToDefault();
          message.success('기본값으로 초기화되었습니다.');
          loadData();
        } catch (error) {
          console.error('Reset feeling examples error:', error);
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
              느낌 표현
            </Title>
            <Space>
              <Button onClick={() => handleOpenAddModal()}>
                <PlusOutlined /> 예시 추가
              </Button>
              <Button onClick={handleReset}>기본값으로 초기화</Button>
            </Space>
          </div>

          <Text type="secondary">
            10&10을 작성할 때 자신의 감정을 정확하게 표현하는데 도움이 되는 느낌 표현 예시입니다.
          </Text>

          <Collapse defaultActiveKey={['joy']}>
            {categoryList.map((categoryInfo) => (
              <Panel
                header={
                  <span style={{ fontSize: '18px' }}>
                    {categoryInfo.emoji} {categoryInfo.displayName} ({categoryInfo.examples.length})
                  </span>
                }
                key={categoryInfo.category}
                extra={
                  <Button
                    type="link"
                    size="small"
                    icon={<PlusOutlined />}
                    onClick={(e) => {
                      e.stopPropagation();
                      handleOpenAddModal(categoryInfo.category);
                    }}
                  >
                    추가
                  </Button>
                }
              >
                <List
                  loading={loading}
                  dataSource={categoryInfo.examples}
                  renderItem={(example) => (
                    <List.Item
                      actions={
                        !example.isDefault
                          ? [
                              <Button
                                type="text"
                                danger
                                size="small"
                                icon={<DeleteOutlined />}
                                onClick={() => handleDeleteExample(example)}
                              >
                                삭제
                              </Button>,
                            ]
                          : []
                      }
                    >
                      <List.Item.Meta
                        title={
                          <span>
                            <strong>{example.subCategory}</strong>
                            {example.isDefault && (
                              <Text type="secondary" style={{ marginLeft: '8px', fontSize: '12px' }}>
                                (기본)
                              </Text>
                            )}
                          </span>
                        }
                        description={example.description}
                      />
                    </List.Item>
                  )}
                />
              </Panel>
            ))}
          </Collapse>
        </Space>
      </Card>

      {/* 예시 추가 모달 */}
      <Modal
        title="감정 예시 추가"
        open={addModalOpen}
        onCancel={() => {
          setAddModalOpen(false);
          form.resetFields();
          setSelectedCategory(undefined);
        }}
        onOk={() => form.submit()}
        okText="추가"
        cancelText="취소"
      >
        <Form form={form} layout="vertical" onFinish={handleAddExample}>
          <Form.Item
            name="category"
            label="카테고리"
            rules={[{ required: true, message: '카테고리를 선택해주세요.' }]}
          >
            <Select placeholder="카테고리 선택" disabled={!!selectedCategory}>
              <Select.Option value="joy">😊 기쁨</Select.Option>
              <Select.Option value="fear">😰 두려움</Select.Option>
              <Select.Option value="anger">😠 분노</Select.Option>
              <Select.Option value="sadness">😢 슬픔</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            name="subCategory"
            label="감정 이름"
            rules={[{ required: true, message: '감정 이름을 입력해주세요.' }]}
          >
            <Input placeholder="예: 행복한, 불안한" maxLength={20} />
          </Form.Item>

          <Form.Item
            name="description"
            label="설명"
            rules={[{ required: true, message: '설명을 입력해주세요.' }]}
          >
            <Input.TextArea placeholder="감정에 대한 설명" rows={3} maxLength={100} showCount />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
