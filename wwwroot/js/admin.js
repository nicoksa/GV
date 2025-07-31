// Búsqueda en tiempo real
document.getElementById('searchInput').addEventListener('keyup', function () {
    const value = this.value.toLowerCase();
    document.querySelectorAll('table tbody tr').forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(value) ? '' : 'none';
    });
});

// Manejo del modal de eliminación
let currentIdToDelete = 0;

function confirmDelete(id) {
    currentIdToDelete = id;
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
}

// Configuración del botón de confirmación de eliminación
document.getElementById('confirmDeleteBtn')?.addEventListener('click', function () {
    const form = document.createElement('form');
    form.method = 'post';

    // Determinar la ruta basada en la página actual
    const isGestionCampos = window.location.pathname.includes('GestionCampos');
    form.action = isGestionCampos ? 'GestionCampos?handler=Delete' : 'GestionPropiedades?handler=Delete';

    // Agregar token antiforgery
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    const tokenInput = document.createElement('input');
    tokenInput.type = 'hidden';
    tokenInput.name = '__RequestVerificationToken';
    tokenInput.value = token;

    // Agregar ID
    const idInput = document.createElement('input');
    idInput.type = 'hidden';
    idInput.name = 'id';
    idInput.value = currentIdToDelete;

    form.appendChild(tokenInput);
    form.appendChild(idInput);
    document.body.appendChild(form);
    form.submit();

    const modal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
    modal.hide();
});

