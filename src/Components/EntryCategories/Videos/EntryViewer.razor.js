export function main(args) {
    function formatted(duration) {
        let hours = Math.floor(duration / 3600)
        let minutes = Math.floor(duration / 60 - (hours * 60))
        let seconds = Math.floor(duration - (minutes * 60 + hours * 3600))

        return hours.toLocaleString(undefined, { minimumIntegerDigits: 2 }) + ":" + minutes.toLocaleString(undefined, { minimumIntegerDigits: 2 }) + ":" + seconds.toLocaleString(undefined, { minimumIntegerDigits: 2 })
    }

    function toggleFullscreen() {
        let fullscreen = document.fullscreenElement !== null
        if (fullscreen) {
            document.exitFullscreen()
        } else {
            document.documentElement.requestFullscreen()
        }
    }

    function togglePlayPause() {
        let playing = video.paused == false
        if (playing) {
            video.pause()
        } else {
            video.play()
        }
    }

    function hideIdleMouseCursor() {
        // https://stackoverflow.com/a/4483383/1528847

        var mouseTimer = null
        var cursorVisible = true

        function disappearCursor() {
            mouseTimer = null
            document.body.style.cursor = "none"
            cursorVisible = false
        }

        document.onmousemove = function () {
            if (mouseTimer) {
                window.clearTimeout(mouseTimer)
            }
            if (!cursorVisible) {
                document.body.style.cursor = "default"
                cursorVisible = true
            }
            mouseTimer = window.setTimeout(disappearCursor, 2000)
        };
        mouseTimer = window.setTimeout(disappearCursor, 2000)
    }

    function toggleMuting() {
        video.muted = !video.muted
    }

    function stateHasChanged() {
        let fullscreen = document.fullscreenElement !== null
        enterfullscreen.style.visibility = fullscreen ? 'hidden' : 'visible'
        exitfullscreen.style.visibility = fullscreen ? 'visible' : 'hidden'

        let muted = video.muted
        mute.style.visibility = muted ? 'hidden' : 'visible'
        unmute.style.visibility = muted ? 'visible' : 'hidden'

        let playing = video.paused == false
        video.classList.toggle('playing', playing)
        //play.style.visibility = playing ? 'hidden' : 'visible'
        //pause.style.visibility = playing ? 'visible' : 'hidden'

        let ratio = video.duration > 0 ? video.currentTime / video.duration : 0
        progress.value = ratio * 100
        range.value = ratio * 100

        lefttext.innerText = formatted(0)
        middletext.innerText = formatted(video.currentTime)
        righttext.innerText = formatted(video.duration)
    }

    document.onfullscreenchange = stateHasChanged
    video.onvolumechange = stateHasChanged
    video.onplay = stateHasChanged
    video.onended = stateHasChanged
    video.onpause = stateHasChanged
    video.ontimeupdate = stateHasChanged

    enterfullscreen.onclick = () => document.documentElement.requestFullscreen()
    exitfullscreen.onclick = () => document.exitFullscreen()
    forward.onclick = () => video.currentTime += 10
    backward.onclick = () => video.currentTime -= 10
    mute.onclick = () => video.muted = true
    unmute.onclick = () => video.muted = false
    play.onclick = () => togglePlayPause()
    //pause.onclick = () => video.pause()
    range.oninput = () => video.currentTime = video.duration * range.value / range.max

    document.onkeydown = event => {
        if (event.key === 'f') {
            toggleFullscreen()
        }
        if (event.key === ' ') {
            togglePlayPause()
        }
        if (event.key === 'm') {
            toggleMuting()
        }
        if (event.key === 'ArrowRight') {
            video.currentTime += 10
        }
        if (event.key === 'ArrowLeft') {
            video.currentTime -= 10
        }
    }

    document.documentElement.ondblclick = () => toggleFullscreen()

    stateHasChanged()

    hideIdleMouseCursor()

    if ("mediaSession" in navigator) {
        navigator.mediaSession.metadata = new MediaMetadata({
            title: args.media.title,
            artist: args.media.artist,
            artwork: [
                { src: args.media.image, sizes: '256x256', type: 'image/png' }
            ]
        });
    }
}