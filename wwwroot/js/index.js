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



document.addEventListener('DOMContentLoaded', function () {
    // Manejar clic en pestañas (código existente)
    const tabs = document.querySelectorAll('.division-tab');
    const forms = document.querySelectorAll('.tab-content');

    tabs.forEach(tab => {
        tab.addEventListener('click', function () {
            tabs.forEach(t => t.classList.remove('active'));
            this.classList.add('active');

            forms.forEach(form => form.style.display = 'none');
            const tabId = this.getAttribute('data-tab');
            document.getElementById(`form-${tabId}`).style.display = 'block';
        });
    });

    // Manejar envío del formulario de Urbano
    const formUrbano = document.getElementById('form-urbano');
    if (formUrbano) {
        formUrbano.addEventListener('submit', function (e) {
            // Validaciones adicionales si son necesarias
        });
    }
});








