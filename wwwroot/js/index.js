document.addEventListener('DOMContentLoaded', function () {
    // Seleccionar elementos
    const tabs = document.querySelectorAll('.division-tab');
    const forms = document.querySelectorAll('.tab-content');

    // Manejar clic en pestañas
    tabs.forEach(tab => {
        tab.addEventListener('click', function () {
            // Remover clase active de todas las pestañas
            tabs.forEach(t => t.classList.remove('active'));

            // Añadir clase active a la pestaña clickeada
            this.classList.add('active');

            // Ocultar todos los formularios
            forms.forEach(form => form.style.display = 'none');

            // Mostrar el formulario correspondiente
            const tabId = this.getAttribute('data-tab');
            document.getElementById(`form-${tabId}`).style.display = 'block';
        });
    });
});