/**
 * 설정 페이지
 * 배우자 관리, 비밀번호 변경 등
 */

import { useState } from 'react';
import {
  Card,
  Button,
  Space,
  Typography,
  Divider,
  Popconfirm,
  App,
} from 'antd';
import { UserOutlined, DisconnectOutlined, KeyOutlined } from '@ant-design/icons';
import { useAuthStore } from '@stores/authStore';
import { partnerService } from '@services/PartnerService';
import InvitePartnerModal from '@components/InvitePartnerModal';
import ChangePasswordModal from '@components/ChangePasswordModal';
import { formatDate } from '@utils/date';
import { ERROR_MESSAGES } from '@utils/constants';

const { Title, Text, Paragraph } = Typography;

const Settings: React.FC = () => {
  const { message } = App.useApp();
  const { user } = useAuthStore();
  const [loading, setLoading] = useState(false);
  const [inviteModalOpen, setInviteModalOpen] = useState(false);
  const [passwordModalOpen, setPasswordModalOpen] = useState(false);

  const hasPartner = !!user?.partner;

  const handleDisconnectPartner = async () => {
    setLoading(true);
    try {
      await partnerService.disconnectPartner();
      message.success('배우자 연결이 해제되었습니다.');
    } catch (error) {
      if (error instanceof Error) {
        message.error(error.message);
      } else {
        message.error(ERROR_MESSAGES.DELETE_FAILED);
      }
    } finally {
      setLoading(false);
    }
  };

  // 배우자 초대 성공 핸들러 (필요시 추가 처리)

  return (
    <div style={{ padding: '24px', maxWidth: '800px', margin: '0 auto' }}>
      <Title level={2}>설정</Title>

      {/* 배우자 연결 섹션 */}
      <Card
        title={
          <Space>
            <UserOutlined />
            <span>배우자 연결</span>
          </Space>
        }
        style={{ marginBottom: '24px' }}
      >
        {!hasPartner ? (
          <div>
            <Paragraph>
              배우자를 초대하여 10&10을 공유할 수 있습니다. 배우자와 연결하면
              '공유' 타입의 10&10을 서로 읽을 수 있습니다.
            </Paragraph>
            <Button
              type="primary"
              onClick={() => setInviteModalOpen(true)}
              icon={<UserOutlined />}
            >
              배우자 초대
            </Button>
          </div>
        ) : (
          <div>
            <Space direction="vertical" style={{ width: '100%' }}>
              <div>
                <Text strong>배우자 이름:</Text>{' '}
                <Text>{user?.partner?.partnerDisplayName}</Text>
              </div>
              <div>
                <Text strong>배우자 이메일:</Text>{' '}
                <Text>{user?.partner?.partnerEmail}</Text>
              </div>
              <div>
                <Text strong>연결일:</Text>{' '}
                <Text>
                  {user?.partner?.connectedAt
                    ? formatDate(new Date(user.partner.connectedAt), 'yyyy-MM-dd')
                    : '-'}
                </Text>
              </div>
            </Space>
            <Divider />
            <Popconfirm
              title="배우자 연결 해제"
              description="정말 배우자 연결을 해제하시겠습니까? 연결 해제 후 다시 연결하려면 새로 초대해야 합니다."
              onConfirm={handleDisconnectPartner}
              okText="해제"
              cancelText="취소"
            >
              <Button
                danger
                loading={loading}
                icon={<DisconnectOutlined />}
              >
                배우자 연결 해제
              </Button>
            </Popconfirm>
          </div>
        )}
      </Card>

      {/* 비밀번호 변경 섹션 */}
      <Card
        title={
          <Space>
            <KeyOutlined />
            <span>계정 설정</span>
          </Space>
        }
      >
        <Space direction="vertical" style={{ width: '100%' }}>
          <div>
            <Text strong>이메일:</Text> <Text>{user?.email}</Text>
          </div>
          <div>
            <Text strong>이름:</Text> <Text>{user?.displayName}</Text>
          </div>
          <Divider />
          <Button
            icon={<KeyOutlined />}
            onClick={() => setPasswordModalOpen(true)}
          >
            비밀번호 변경
          </Button>
        </Space>
      </Card>

      {/* 모달들 */}
      <InvitePartnerModal
        open={inviteModalOpen}
        onClose={() => setInviteModalOpen(false)}
        onSuccess={() => {
          window.location.reload();
        }}
      />

      <ChangePasswordModal
        open={passwordModalOpen}
        onClose={() => setPasswordModalOpen(false)}
      />
    </div>
  );
};

export default Settings;
