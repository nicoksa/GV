
/*
// Script para el toggle de filtros avanzados - Versión corregida
document.querySelector('.toggle-advanced-filters').addEventListener('click', function () {
    const icon = this.querySelector('i');
    icon.classList.toggle('fa-chevron-down');
    icon.classList.toggle('fa-chevron-up');

    const textSpan = this.querySelector('.filter-toggle-text');
    if (icon.classList.contains('fa-chevron-up')) {
        textSpan.textContent = 'Menos opciones';
    } else {
        textSpan.textContent = 'Más opciones de búsqueda';
    }
});
*/

// Validación básica de precios
document.querySelector('form').addEventListener('submit', function (e) {
    const minPrice = parseFloat(document.getElementById('min-price').value);
    const maxPrice = parseFloat(document.getElementById('max-price').value);

    if (minPrice && maxPrice && minPrice > maxPrice) {
        e.preventDefault();
        alert('El precio mínimo no puede ser mayor al precio máximo');
    }
});

// Función para verificar si hay valores seleccionados
function hasSelectedFilters() {
    const filterInputs = document.querySelectorAll('.advanced-filters input, .advanced-filters select');
    return Array.from(filterInputs).some(input => {
        if (input.type === 'checkbox' || input.type === 'radio') {
            return input.checked;
        }
        return input.value && input.value !== '';
    });
}

/*
// Función para mostrar/ocultar filtros y actualizar el texto del toggle
function toggleFiltersVisibility() {
    const advancedFilters = document.querySelector('.advanced-filters');
    const toggleButton = document.querySelector('.toggle-advanced-filters');

    if (hasSelectedFilters()) {
        new bootstrap.Collapse(advancedFilters, { toggle: true }).show();
        // Actualizar texto del toggle cuando se muestran los filtros
        const icon = toggleButton.querySelector('i');
        const textSpan = toggleButton.querySelector('.filter-toggle-text');
        icon.classList.replace('fa-chevron-down', 'fa-chevron-up');
        textSpan.textContent = 'Menos opciones';
    }
}
*/

/*
// Configurar eventos
document.addEventListener('DOMContentLoaded', function () {
    // Mostrar filtros si hay valores seleccionados al cargar
    toggleFiltersVisibility();

    // Configurar el botón toggle para cerrar cuando no hay filtros
    const toggleButton = document.querySelector('.toggle-advanced-filters');
    if (toggleButton) {
        toggleButton.addEventListener('click', function () {
            if (!hasSelectedFilters()) {
                setTimeout(() => {
                    const advancedFilters = document.querySelector('.advanced-filters');
                    if (!advancedFilters.classList.contains('show')) {
                        new bootstrap.Collapse(advancedFilters, { toggle: false }).hide();
                    }
                }, 10);
            }
        });
    }
});

// Mantener filtros visibles al enviar formulario o paginar
document.querySelector('form').addEventListener('submit', toggleFiltersVisibility);
document.querySelectorAll('.site-pagination a').forEach(link => {
    link.addEventListener('click', toggleFiltersVisibility);
});
*/