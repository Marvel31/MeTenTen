/**
 * Ant Design 테마 커스터마이징
 */

import type { ThemeConfig } from 'antd';

export const theme: ThemeConfig = {
  token: {
    // 색상
    colorPrimary: '#FF6B9D', // 핑크 - 사랑과 소통
    colorSuccess: '#52C41A', // 그린 - 성공
    colorWarning: '#FAAD14', // 옐로우 - 경고
    colorError: '#F5222D', // 레드 - 에러
    colorInfo: '#1890FF', // 블루 - 정보

    // 텍스트
    colorText: '#2C3E50', // 다크 그레이
    colorTextSecondary: '#8C8C8C',

    // 배경
    colorBgContainer: '#FFFFFF',
    colorBgLayout: '#FFF5F7', // 연한 핑크

    // 테두리
    colorBorder: '#E8E8E8',
    borderRadius: 8,

    // 폰트
    fontSize: 14,
    fontFamily:
      '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, "Noto Sans", sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji"',
  },
  components: {
    Button: {
      borderRadius: 8,
      controlHeight: 40,
      fontWeight: 500,
    },
    Input: {
      borderRadius: 8,
      controlHeight: 40,
    },
    Card: {
      borderRadius: 12,
    },
    Modal: {
      borderRadius: 12,
    },
  },
};

export default theme;

