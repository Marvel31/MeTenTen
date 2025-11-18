/**
 * 암호화 서비스
 * Web Crypto API를 사용한 End-to-End 암호화
 * - PBKDF2: 비밀번호 기반 키 유도 (100,000 iterations, SHA-256)
 * - AES-256-CBC: 데이터 암호화/복호화
 */

class EncryptionService {
  private dek: CryptoKey | null = null; // Personal DEK (Data Encryption Key)
  private sharedDek: CryptoKey | null = null; // Shared DEK (배우자와 공유)

  private readonly PBKDF2_ITERATIONS = 100000;
  private readonly KEY_LENGTH = 256; // bits
  private readonly ALGORITHM = 'AES-CBC';

  /**
   * 랜덤 DEK 생성 (256-bit)
   */
  async generateRandomDEK(): Promise<CryptoKey> {
    return await window.crypto.subtle.generateKey(
      {
        name: this.ALGORITHM,
        length: this.KEY_LENGTH,
      },
      true, // extractable
      ['encrypt', 'decrypt']
    );
  }

  /**
   * 비밀번호로부터 KEK(Key Encryption Key) 유도
   * PBKDF2 사용
   */
  private async deriveKEK(
    password: string,
    email: string
  ): Promise<CryptoKey> {
    const encoder = new TextEncoder();
    const passwordBuffer = encoder.encode(password);
    const saltBuffer = encoder.encode(email); // 이메일을 salt로 사용

    // 비밀번호를 key material로 import
    const keyMaterial = await window.crypto.subtle.importKey(
      'raw',
      passwordBuffer,
      'PBKDF2',
      false,
      ['deriveKey']
    );

    // PBKDF2로 KEK 유도
    return await window.crypto.subtle.deriveKey(
      {
        name: 'PBKDF2',
        salt: saltBuffer,
        iterations: this.PBKDF2_ITERATIONS,
        hash: 'SHA-256',
      },
      keyMaterial,
      {
        name: this.ALGORITHM,
        length: this.KEY_LENGTH,
      },
      false, // not extractable
      ['wrapKey', 'unwrapKey']
    );
  }

  /**
   * DEK를 비밀번호로 암호화 (Wrap)
   * @returns Base64 encoded wrapped DEK
   */
  async encryptDEK(
    dek: CryptoKey,
    email: string,
    password: string
  ): Promise<string> {
    const kek = await this.deriveKEK(password, email);
    const iv = window.crypto.getRandomValues(new Uint8Array(16));

    const wrappedKey = await window.crypto.subtle.wrapKey(
      'raw',
      dek,
      kek,
      {
        name: this.ALGORITHM,
        iv: iv,
      }
    );

    // IV + wrapped key를 결합하여 Base64로 인코딩
    const combined = new Uint8Array(iv.length + wrappedKey.byteLength);
    combined.set(iv, 0);
    combined.set(new Uint8Array(wrappedKey), iv.length);

    return this.arrayBufferToBase64(combined);
  }

  /**
   * 암호화된 DEK를 비밀번호로 복호화 (Unwrap)
   * @param encryptedDEK Base64 encoded wrapped DEK
   */
  async decryptDEK(
    encryptedDEK: string,
    email: string,
    password: string
  ): Promise<CryptoKey> {
    const kek = await this.deriveKEK(password, email);
    const combined = this.base64ToArrayBuffer(encryptedDEK);

    // IV와 wrapped key 분리
    const iv = combined.slice(0, 16);
    const wrappedKey = combined.slice(16);

    return await window.crypto.subtle.unwrapKey(
      'raw',
      wrappedKey,
      kek,
      {
        name: this.ALGORITHM,
        iv: iv,
      },
      {
        name: this.ALGORITHM,
        length: this.KEY_LENGTH,
      },
      true, // extractable
      ['encrypt', 'decrypt']
    );
  }

  /**
   * 평문을 DEK로 암호화
   * @param plaintext 평문
   * @param dek 암호화 키 (없으면 현재 설정된 DEK 사용)
   * @returns Base64 encoded ciphertext
   */
  async encrypt(plaintext: string, dek?: CryptoKey): Promise<string> {
    const key = dek || this.dek;
    if (!key) {
      throw new Error('DEK가 설정되지 않았습니다.');
    }

    const encoder = new TextEncoder();
    const data = encoder.encode(plaintext);
    const iv = window.crypto.getRandomValues(new Uint8Array(16));

    const ciphertext = await window.crypto.subtle.encrypt(
      {
        name: this.ALGORITHM,
        iv: iv,
      },
      key,
      data
    );

    // IV + ciphertext를 결합하여 Base64로 인코딩
    const combined = new Uint8Array(iv.length + ciphertext.byteLength);
    combined.set(iv, 0);
    combined.set(new Uint8Array(ciphertext), iv.length);

    return this.arrayBufferToBase64(combined);
  }

  /**
   * 암호문을 DEK로 복호화
   * @param ciphertext Base64 encoded ciphertext
   * @param dek 복호화 키 (없으면 현재 설정된 DEK 사용)
   * @returns 평문
   */
  async decrypt(ciphertext: string, dek?: CryptoKey): Promise<string> {
    const key = dek || this.dek;
    if (!key) {
      throw new Error('DEK가 설정되지 않았습니다.');
    }

    const combined = this.base64ToArrayBuffer(ciphertext);

    // IV와 ciphertext 분리
    const iv = combined.slice(0, 16);
    const data = combined.slice(16);

    const plaintext = await window.crypto.subtle.decrypt(
      {
        name: this.ALGORITHM,
        iv: iv,
      },
      key,
      data
    );

    const decoder = new TextDecoder();
    return decoder.decode(plaintext);
  }

  /**
   * Shared DEK로 암호화
   */
  async encryptWithSharedDEK(plaintext: string): Promise<string> {
    if (!this.sharedDek) {
      throw new Error('Shared DEK가 설정되지 않았습니다.');
    }
    return await this.encrypt(plaintext, this.sharedDek);
  }

  /**
   * Shared DEK로 복호화
   */
  async decryptWithSharedDEK(ciphertext: string): Promise<string> {
    if (!this.sharedDek) {
      throw new Error('Shared DEK가 설정되지 않았습니다.');
    }
    return await this.decrypt(ciphertext, this.sharedDek);
  }

  /**
   * 공유 DEK 생성 (배우자 연결 시 사용)
   */
  async generateSharedDEK(): Promise<CryptoKey> {
    return await this.generateRandomDEK();
  }

  /**
   * Personal DEK를 메모리에 설정
   */
  setDEK(dek: CryptoKey): void {
    this.dek = dek;
  }

  /**
   * Shared DEK를 메모리에 설정
   */
  setSharedDEK(sharedDek: CryptoKey): void {
    this.sharedDek = sharedDek;
  }

  /**
   * DEK가 설정되어 있는지 확인
   */
  hasDEK(): boolean {
    return this.dek !== null;
  }

  /**
   * Shared DEK가 설정되어 있는지 확인
   */
  hasSharedDEK(): boolean {
    return this.sharedDek !== null;
  }

  /**
   * 모든 키를 메모리에서 삭제 (로그아웃 시)
   */
  clearKeys(): void {
    this.dek = null;
    this.sharedDek = null;
  }

  /**
   * CryptoKey를 Base64 문자열로 export
   */
  async exportKeyToBase64(key: CryptoKey): Promise<string> {
    const exported = await window.crypto.subtle.exportKey('raw', key);
    return this.arrayBufferToBase64(new Uint8Array(exported));
  }

  /**
   * Base64 문자열을 CryptoKey로 import
   */
  async importKeyFromBase64(base64Key: string): Promise<CryptoKey> {
    const keyData = this.base64ToArrayBuffer(base64Key);
    return await window.crypto.subtle.importKey(
      'raw',
      keyData as BufferSource,
      {
        name: this.ALGORITHM,
        length: this.KEY_LENGTH,
      },
      true,
      ['encrypt', 'decrypt']
    );
  }

  // ========== 유틸리티 함수 ==========

  /**
   * ArrayBuffer를 Base64로 인코딩
   */
  private arrayBufferToBase64(buffer: Uint8Array): string {
    let binary = '';
    const len = buffer.byteLength;
    for (let i = 0; i < len; i++) {
      binary += String.fromCharCode(buffer[i]);
    }
    return btoa(binary);
  }

  /**
   * Base64를 ArrayBuffer로 디코딩
   */
  private base64ToArrayBuffer(base64: string): Uint8Array {
    const binary = atob(base64);
    const len = binary.length;
    const buffer = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
      buffer[i] = binary.charCodeAt(i);
    }
    return buffer;
  }
}

// 싱글톤 인스턴스
export const encryptionService = new EncryptionService();
export default encryptionService;

