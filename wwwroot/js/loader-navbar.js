// Mostrar loader inmediatamente
document.body.classList.add('loading');

// Ocultar loader cuando todo haya cargado
window.addEventListener('load', function () {
    const preloader = document.getElementById('preloader');
    if (preloader) {
        preloader.style.opacity = '0';
        preloader.style.visibility = 'hidden';
        document.body.classList.remove('loading');

        // Eliminar el preloader del DOM después de la animación
        setTimeout(function () {
            preloader.remove();
        }, 500); // Tiempo igual a la duración de la transición
    }
});



document.addEventListener('DOMContentLoaded', function () {
    const navbar = document.querySelector('.index-navbar');

    if (navbar) {
        document.body.classList.add('has-index-navbar');

        window.addEventListener('scroll', function () {
            if (window.scrollY > 100) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        });
    }
});