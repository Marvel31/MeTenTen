let deferredPrompt;

// PWA 설치 가능 여부 확인
export function canInstall() {
    return deferredPrompt !== null;
}

// PWA 설치
export function installApp() {
    if (deferredPrompt) {
        deferredPrompt.prompt();
        deferredPrompt.userChoice.then((choiceResult) => {
            if (choiceResult.outcome === 'accepted') {
                console.log('User accepted the install prompt');
            } else {
                console.log('User dismissed the install prompt');
            }
            deferredPrompt = null;
        });
    }
}

// beforeinstallprompt 이벤트 리스너
window.addEventListener('beforeinstallprompt', (e) => {
    e.preventDefault();
    deferredPrompt = e;
});

// appinstalled 이벤트 리스너
window.addEventListener('appinstalled', (evt) => {
    console.log('PWA was installed');
    deferredPrompt = null;
});
