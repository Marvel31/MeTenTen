/**
 * 10분 카운트다운 타이머 컴포넌트
 */

import { useState, useEffect, useRef, useCallback } from 'react';
import { Button, Space, Typography, App } from 'antd';
import {
  PlayCircleOutlined,
  PauseCircleOutlined,
  ReloadOutlined,
  SoundOutlined,
} from '@ant-design/icons';

const { Title } = Typography;

interface TimerProps {
  initialMinutes?: number; // 기본값: 10분
  onComplete?: () => void; // 타이머 완료 시 호출
  autoStart?: boolean; // 자동 시작 여부
  showControls?: boolean; // 컨트롤 버튼 표시 여부
  compact?: boolean; // 작은 버전 (모달 내에서 사용)
}

const Timer: React.FC<TimerProps> = ({
  initialMinutes = 10,
  onComplete,
  autoStart = false,
  showControls = true,
  compact = false,
}) => {
  const { message } = App.useApp();
  const [seconds, setSeconds] = useState(initialMinutes * 60);
  const [isRunning, setIsRunning] = useState(autoStart);
  const [isCompleted, setIsCompleted] = useState(false);
  const intervalRef = useRef<number | null>(null);

  // 타이머 시작/일시정지
  const toggleTimer = useCallback(() => {
    setIsRunning((prev) => !prev);
  }, []);

  // 타이머 재시작
  const resetTimer = useCallback(() => {
    setSeconds(initialMinutes * 60);
    setIsRunning(false);
    setIsCompleted(false);
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
  }, [initialMinutes]);

  // 시간 포맷 (MM:SS)
  const formatTime = useCallback((totalSeconds: number): string => {
    const minutes = Math.floor(totalSeconds / 60);
    const remainingSeconds = totalSeconds % 60;
    return `${String(minutes).padStart(2, '0')}:${String(
      remainingSeconds
    ).padStart(2, '0')}`;
  }, []);

  // 타이머 로직
  useEffect(() => {
    if (isRunning && seconds > 0) {
      intervalRef.current = window.setInterval(() => {
        setSeconds((prev) => {
          if (prev <= 1) {
            setIsRunning(false);
            setIsCompleted(true);
            if (onComplete) {
              onComplete();
            }
            message.success({
              content: '⏰ 10분이 지났습니다!',
              duration: 5,
              icon: <SoundOutlined style={{ color: '#52c41a' }} />,
            });
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    } else {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
    }

    // cleanup
    return () => {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
    };
  }, [isRunning, seconds, onComplete, message]);

  // 컴포넌트 언마운트 시 정리
  useEffect(() => {
    return () => {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
      }
    };
  }, []);

  const displayTime = formatTime(seconds);
  const progress = ((initialMinutes * 60 - seconds) / (initialMinutes * 60)) * 100;

  return (
    <div
      style={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        gap: compact ? '8px' : '16px',
        padding: compact ? '12px' : '24px',
        borderRadius: '12px',
        background: 'var(--background-white)',
        border: `2px solid ${isCompleted ? '#52c41a' : isRunning ? '#1890ff' : '#d9d9d9'}`,
      }}
    >
      {/* 타이머 표시 */}
      <Title
        level={1}
        style={{
          margin: 0,
          fontSize: compact ? '32px' : '64px',
          fontFamily: 'monospace',
          color: isCompleted
            ? '#52c41a'
            : isRunning
            ? '#1890ff'
            : '#595959',
          transition: 'color 0.3s',
        }}
      >
        {displayTime}
      </Title>

      {/* 진행률 표시 */}
      <div
        style={{
          width: '100%',
          height: compact ? '4px' : '8px',
          background: '#f0f0f0',
          borderRadius: '4px',
          overflow: 'hidden',
        }}
      >
        <div
          style={{
            width: `${progress}%`,
            height: '100%',
            background: isCompleted
              ? '#52c41a'
              : isRunning
              ? '#1890ff'
              : '#d9d9d9',
            transition: 'all 0.3s',
          }}
        />
      </div>

      {/* 상태 표시 */}
      {isCompleted ? (
        <div
          style={{
            color: '#52c41a',
            fontWeight: 'bold',
            fontSize: compact ? '12px' : '14px',
          }}
        >
          완료! 🎉
        </div>
      ) : (
        <div
          style={{
            color: isRunning ? '#1890ff' : '#8c8c8c',
            fontSize: compact ? '12px' : '14px',
          }}
        >
          {isRunning ? '작성 중...' : '대기 중'}
        </div>
      )}

      {/* 컨트롤 버튼 */}
      {showControls && (
        <Space size={compact ? 'small' : 'middle'}>
          {!isCompleted && (
            <Button
              type="primary"
              icon={
                isRunning ? (
                  <PauseCircleOutlined />
                ) : (
                  <PlayCircleOutlined />
                )
              }
              onClick={toggleTimer}
              size={compact ? 'small' : 'large'}
              style={{ minWidth: compact ? '80px' : '120px' }}
            >
              {isRunning ? '일시정지' : '시작'}
            </Button>
          )}

          <Button
            icon={<ReloadOutlined />}
            onClick={resetTimer}
            size={compact ? 'small' : 'large'}
            disabled={seconds === initialMinutes * 60 && !isRunning}
          >
            재시작
          </Button>
        </Space>
      )}
    </div>
  );
};

export default Timer;

