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