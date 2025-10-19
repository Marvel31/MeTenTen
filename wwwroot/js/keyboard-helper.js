// 키보드 스크롤 처리 도우미 함수들

// Android 플랫폼 감지
function isAndroid() {
    return /Android/i.test(navigator.userAgent);
}

// Visual Viewport API 지원 여부 확인
function supportsVisualViewport() {
    return window.visualViewport !== undefined;
}

// 키보드 높이 감지
function getKeyboardHeight() {
    if (supportsVisualViewport()) {
        return window.innerHeight - window.visualViewport.height;
    }
    return 0;
}

// 특정 요소로 스크롤하는 함수 (개선된 버전)
window.scrollToElement = function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        // 키보드 높이를 고려한 스크롤 위치 계산
        const keyboardHeight = getKeyboardHeight();
        const elementRect = element.getBoundingClientRect();
        const viewportHeight = window.innerHeight;
        const availableHeight = viewportHeight - keyboardHeight;
        
        // 요소가 키보드에 가려지는지 확인
        if (elementRect.bottom > availableHeight) {
            // 요소를 키보드 위쪽으로 스크롤
            const scrollAmount = elementRect.bottom - availableHeight + 20; // 20px 여백
            window.scrollBy({
                top: scrollAmount,
                behavior: 'smooth'
            });
        }
        
        // 약간의 지연 후 포커스 (스크롤 완료 후)
        setTimeout(() => {
            element.focus();
        }, 300);
        
        return true;
    }
    return false;
};

// Input 요소에 포커스 이벤트 핸들러 설정 (개선된 버전)
window.setupInputFocusHandler = function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.addEventListener('focus', function() {
            if (isAndroid()) {
                // Visual Viewport 변경 감지
                if (supportsVisualViewport()) {
                    const handleViewportChange = () => {
                        const keyboardHeight = getKeyboardHeight();
                        if (keyboardHeight > 0) {
                            const elementRect = element.getBoundingClientRect();
                            const availableHeight = window.innerHeight - keyboardHeight;
                            
                            if (elementRect.bottom > availableHeight) {
                                const scrollAmount = elementRect.bottom - availableHeight + 20;
                                window.scrollBy({
                                    top: scrollAmount,
                                    behavior: 'smooth'
                                });
                            }
                        }
                    };
                    
                    window.visualViewport.addEventListener('resize', handleViewportChange);
                    // 초기 호출
                    setTimeout(handleViewportChange, 300);
                } else {
                    // Visual Viewport API가 없는 경우 기존 방식
                    setTimeout(() => {
                        const keyboardHeight = 300; // 예상 키보드 높이
                        const elementRect = element.getBoundingClientRect();
                        const availableHeight = window.innerHeight - keyboardHeight;
                        
                        if (elementRect.bottom > availableHeight) {
                            const scrollAmount = elementRect.bottom - availableHeight + 20;
                            window.scrollBy({
                                top: scrollAmount,
                                behavior: 'smooth'
                            });
                        }
                    }, 600); // 더 긴 대기 시간
                }
            }
        });
        return true;
    }
    return false;
};

// 모든 input/textarea 요소에 자동 포커스 핸들러 설정 (개선된 버전)
window.setupAutoFocusHandlers = function() {
    if (!isAndroid()) {
        return; // Android가 아니면 처리하지 않음
    }
    
    const inputs = document.querySelectorAll('input, textarea');
    inputs.forEach(input => {
        input.addEventListener('focus', function() {
            if (supportsVisualViewport()) {
                // Visual Viewport API 사용
                const handleViewportChange = () => {
                    const keyboardHeight = getKeyboardHeight();
                    if (keyboardHeight > 0) {
                        const elementRect = this.getBoundingClientRect();
                        const availableHeight = window.innerHeight - keyboardHeight;
                        
                        if (elementRect.bottom > availableHeight) {
                            const scrollAmount = elementRect.bottom - availableHeight + 20;
                            window.scrollBy({
                                top: scrollAmount,
                                behavior: 'smooth'
                            });
                        }
                    }
                };
                
                window.visualViewport.addEventListener('resize', handleViewportChange);
                setTimeout(handleViewportChange, 300);
            } else {
                // 기존 방식
                setTimeout(() => {
                    const keyboardHeight = 300;
                    const elementRect = this.getBoundingClientRect();
                    const availableHeight = window.innerHeight - keyboardHeight;
                    
                    if (elementRect.bottom > availableHeight) {
                        const scrollAmount = elementRect.bottom - availableHeight + 20;
                        window.scrollBy({
                            top: scrollAmount,
                            behavior: 'smooth'
                        });
                    }
                }, 600);
            }
        });
    });
};

// 모달이 열릴 때 특정 입력 필드로 자동 포커스 및 스크롤 (개선된 버전)
window.focusInputInModal = function(elementId) {
    if (isAndroid()) {
        setTimeout(() => {
            const element = document.getElementById(elementId);
            if (element) {
                // 키보드 높이를 고려한 스크롤
                const keyboardHeight = getKeyboardHeight() || 300;
                const elementRect = element.getBoundingClientRect();
                const availableHeight = window.innerHeight - keyboardHeight;
                
                if (elementRect.bottom > availableHeight) {
                    const scrollAmount = elementRect.bottom - availableHeight + 20;
                    window.scrollBy({
                        top: scrollAmount,
                        behavior: 'smooth'
                    });
                } else {
                    element.scrollIntoView({
                        behavior: 'smooth',
                        block: 'center',
                        inline: 'nearest'
                    });
                }
                
                setTimeout(() => {
                    element.focus();
                }, 300);
            }
        }, 100); // 모달 애니메이션 완료 후 실행
    }
};

// 페이지 로드 시 자동 설정
document.addEventListener('DOMContentLoaded', function() {
    if (isAndroid()) {
        // 페이지가 완전히 로드된 후 자동 핸들러 설정
        setTimeout(() => {
            window.setupAutoFocusHandlers();
        }, 1000);
    }
});

console.log('Keyboard helper loaded for platform:', isAndroid() ? 'Android' : 'Other');
