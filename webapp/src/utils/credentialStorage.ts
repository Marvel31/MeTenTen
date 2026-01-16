/**
 * 자격증명 암호화 및 스토리지 관리
 * 세션 스토리지(기본) 또는 로컬 스토리지("로그인 유지" 옵션)에 암호화된 자격증명 저장
 */

import { setStorage, getStorage, removeStorage, setSessionStorage, getSessionStorage, removeSessionStorage } from './storage';

interface EncryptedCredentials {
  email: string;
  encryptedPassword: string;
  iv: string;
  deviceId: string;
  expiresAt: string;
}

const CREDENTIAL_KEY = 'metenten_credentials';
const CREDENTIAL_EXPIRY_DAYS = 30;

/**
 * 디바이스 고유 ID 생성 (브라우저 fingerprint)
 * 브라우저, OS, 화면 해상도 등을 조합하여 생성
 */
async function generateDeviceId(): Promise<string> {
  const data = [
    navigator.userAgent,
    navigator.language,
    screen.width.toString(),
    screen.height.toString(),
    screen.colorDepth.toString(),
    new Date().getTimezoneOffset().toString(),
  ].join('|');

  const encoder = new TextEncoder();
  const dataBuffer = encoder.encode(data);
  const hashBuffer = await crypto.subtle.digest('SHA-256', dataBuffer);
  const hashArray = Array.from(new Uint8Array(hashBuffer));
  const hashHex = hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
  
  return hashHex;
}

/**
 * 디바이스 ID로부터 암호화 키 유도
 */
async function deriveKeyFromDeviceId(deviceId: string): Promise<CryptoKey> {
  const encoder = new TextEncoder();
  const keyMaterial = encoder.encode(deviceId);
  
  // SHA-256으로 해시하여 키 생성
  const hashBuffer = await crypto.subtle.digest('SHA-256', keyMaterial);
  
  return await crypto.subtle.importKey(
    'raw',
    hashBuffer,
    { name: 'AES-CBC', length: 256 },
    false,
    ['encrypt', 'decrypt']
  );
}

/**
 * 비밀번호 암호화
 */
async function encryptPassword(password: string, deviceId: string): Promise<{ encrypted: string; iv: string }> {
  const key = await deriveKeyFromDeviceId(deviceId);
  const encoder = new TextEncoder();
  const data = encoder.encode(password);
  
  const iv = crypto.getRandomValues(new Uint8Array(16));
  
  const encrypted = await crypto.subtle.encrypt(
    { name: 'AES-CBC', iv },
    key,
    data
  );
  
  return {
    encrypted: btoa(String.fromCharCode(...new Uint8Array(encrypted))),
    iv: btoa(String.fromCharCode(...iv))
  };
}

/**
 * 비밀번호 복호화
 */
async function decryptPassword(encryptedPassword: string, iv: string, deviceId: string): Promise<string> {
  const key = await deriveKeyFromDeviceId(deviceId);
  
  const encryptedData = Uint8Array.from(atob(encryptedPassword), c => c.charCodeAt(0));
  const ivData = Uint8Array.from(atob(iv), c => c.charCodeAt(0));
  
  const decrypted = await crypto.subtle.decrypt(
    { name: 'AES-CBC', iv: ivData },
    key,
    encryptedData
  );
  
  const decoder = new TextDecoder();
  return decoder.decode(decrypted);
}

/**
 * 자격증명 저장
 * @param email 사용자 이메일
 * @param password 사용자 비밀번호
 * @param rememberMe true면 로컬 스토리지, false면 세션 스토리지
 */
export async function saveCredentials(
  email: string,
  password: string,
  rememberMe: boolean
): Promise<void> {
  try {
    const deviceId = await generateDeviceId();
    const { encrypted, iv } = await encryptPassword(password, deviceId);
    
    const expiresAt = new Date();
    expiresAt.setDate(expiresAt.getDate() + CREDENTIAL_EXPIRY_DAYS);
    
    const credentials: EncryptedCredentials = {
      email,
      encryptedPassword: encrypted,
      iv,
      deviceId,
      expiresAt: expiresAt.toISOString(),
    };
    
    if (rememberMe) {
      setStorage(CREDENTIAL_KEY, credentials);
    } else {
      setSessionStorage(CREDENTIAL_KEY, credentials);
    }
  } catch (error) {
    console.error('Failed to save credentials:', error);
    throw error;
  }
}

/**
 * 저장된 자격증명 로드 및 복호화
 * @returns 자격증명이 있고 유효하면 { email, password }, 없으면 null
 */
export async function loadCredentials(): Promise<{ email: string; password: string } | null> {
  try {
    // 세션 스토리지 먼저 확인
    let credentials = getSessionStorage<EncryptedCredentials>(CREDENTIAL_KEY);
    
    // 세션 스토리지에 없으면 로컬 스토리지 확인
    if (!credentials) {
      credentials = getStorage<EncryptedCredentials>(CREDENTIAL_KEY);
    }
    
    if (!credentials) {
      return null;
    }
    
    // 만료 시간 체크
    const expiresAt = new Date(credentials.expiresAt);
    if (expiresAt < new Date()) {
      await clearCredentials();
      return null;
    }
    
    // 디바이스 ID 확인
    const currentDeviceId = await generateDeviceId();
    if (credentials.deviceId !== currentDeviceId) {
      // 다른 기기에서 접근 시도
      await clearCredentials();
      return null;
    }
    
    // 비밀번호 복호화
    const password = await decryptPassword(
      credentials.encryptedPassword,
      credentials.iv,
      credentials.deviceId
    );
    
    return {
      email: credentials.email,
      password,
    };
  } catch (error) {
    console.error('Failed to load credentials:', error);
    await clearCredentials();
    return null;
  }
}

/**
 * 저장된 자격증명 삭제
 */
export async function clearCredentials(): Promise<void> {
  try {
    removeStorage(CREDENTIAL_KEY);
    removeSessionStorage(CREDENTIAL_KEY);
  } catch (error) {
    console.error('Failed to clear credentials:', error);
  }
}




