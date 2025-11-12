/**
 * 라우트 경로 상수 정의
 */

export const ROUTES = {
  HOME: '/',
  LOGIN: '/login',
  SIGNUP: '/signup',
  MY_TENTENS: '/my-tentens',
  PARTNER_TENTENS: '/partner-tentens',
  FEELINGS: '/feelings',
  PRAYERS: '/prayers',
  SETTINGS: '/settings',
  NOT_FOUND: '*',
} as const;

export type RouteKey = keyof typeof ROUTES;
export type RoutePath = (typeof ROUTES)[RouteKey];

export default ROUTES;

