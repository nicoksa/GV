// vista-youtube.js
document.addEventListener('DOMContentLoaded', function () {
    const youtubeInput = document.getElementById('Propiedad_YoutubeUrl');
    const videoPreview = document.getElementById('videoPreview');

    if (youtubeInput && videoPreview) {
        youtubeInput.addEventListener('input', function (event) {
            videoPreview.innerHTML = '';

            const url = this.value.trim();
            if (url) {
                const videoId = extractYouTubeId(url);
                if (videoId) {
                    const iframe = document.createElement('iframe');
                    iframe.width = '100%';
                    iframe.height = '200';
                    iframe.src = `https://www.youtube.com/embed/${videoId}`;
                    iframe.frameBorder = '0';
                    iframe.allow = 'accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture';
                    iframe.allowFullscreen = true;
                    videoPreview.appendChild(iframe);
                }
            }
        });

        // Función para extraer el ID de YouTube
        function extractYouTubeId(url) {
            const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/;
            const match = url.match(regExp);
            return (match && match[2].length === 11) ? match[2] : null;
        }
    }
});