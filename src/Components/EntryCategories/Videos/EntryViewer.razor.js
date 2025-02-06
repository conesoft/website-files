export function registerGlobalKeyboardEvents(assembly, methodName) {
    let serializeEvent = function (e) {
        if (e) {
            return {
                key: e.key,
                code: e.keyCode.toString(),
                location: e.location,
                repeat: e.repeat,
                ctrlKey: e.ctrlKey,
                shiftKey: e.shiftKey,
                altKey: e.altKey,
                metaKey: e.metaKey,
                type: e.type
            };
        }
    };

    window.document.addEventListener('keydown', function (e) {
        DotNet.invokeMethodAsync(assembly, methodName, serializeEvent(e))
    });
}

export function updateMediaSession(title, artist, image) {
    if ("mediaSession" in navigator) {
        navigator.mediaSession.metadata = new MediaMetadata({
            title: title,
            artist: artist,
            artwork: [
                { src: image, sizes: '256x256', type: 'image/png' }
            ]
        });
    }
}