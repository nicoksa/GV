// Script para el toggle de filtros avanzados
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

// Validación básica de precios
document.querySelector('form').addEventListener('submit', function (e) {
    const minPrice = parseFloat(document.getElementById('min-price').value);
    const maxPrice = parseFloat(document.getElementById('max-price').value);

    if (minPrice && maxPrice && minPrice > maxPrice) {
        e.preventDefault();
        alert('El precio mínimo no puede ser mayor al precio máximo');
    }
});


// Ultimo agregado

document.querySelector('form').addEventListener('submit', toggleFiltersVisibility);
document.querySelectorAll('.site-pagination a').forEach(link => {
    link.addEventListener('click', toggleFiltersVisibility);
});