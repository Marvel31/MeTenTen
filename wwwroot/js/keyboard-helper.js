// 키보드 스크롤 처리 도우미 함수들

// Android 플랫폼 감지
function isAndroid() {
    return /Android/i.test(navigator.userAgent);
}

// 특정 요소로 스크롤하는 함수
window.scrollToElement = function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        // 요소가 보이도록 스크롤
        element.scrollIntoView({
            behavior: 'smooth',
            block: 'center',
            inline: 'nearest'
        });
        
        // 약간의 지연 후 포커스 (스크롤 완료 후)
        setTimeout(() => {
            element.focus();
        }, 300);
        
        return true;
    }
    return false;
};

// Input 요소에 포커스 이벤트 핸들러 설정
window.setupInputFocusHandler = function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.addEventListener('focus', function() {
            if (isAndroid()) {
                // Android에서 키보드가 올라올 때까지 잠시 기다린 후 스크롤
                setTimeout(() => {
                    element.scrollIntoView({
                        behavior: 'smooth',
                        block: 'center',
                        inline: 'nearest'
                    });
                }, 500); // 키보드 애니메이션 시간 고려
            }
        });
        return true;
    }
    return false;
};

// 모든 input/textarea 요소에 자동 포커스 핸들러 설정
window.setupAutoFocusHandlers = function() {
    if (!isAndroid()) {
        return; // Android가 아니면 처리하지 않음
    }
    
    const inputs = document.querySelectorAll('input, textarea');
    inputs.forEach(input => {
        input.addEventListener('focus', function() {
            setTimeout(() => {
                this.scrollIntoView({
                    behavior: 'smooth',
                    block: 'center',
                    inline: 'nearest'
                });
            }, 500);
        });
    });
};

// 모달이 열릴 때 특정 입력 필드로 자동 포커스 및 스크롤
window.focusInputInModal = function(elementId) {
    if (isAndroid()) {
        setTimeout(() => {
            const element = document.getElementById(elementId);
            if (element) {
                element.scrollIntoView({
                    behavior: 'smooth',
                    block: 'center',
                    inline: 'nearest'
                });
                
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
