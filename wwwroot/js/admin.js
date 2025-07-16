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

document.getElementById('confirmDeleteBtn').addEventListener('click', function () {
    // Enviar solicitud de eliminación al servidor
    fetch(`?handler=Delete&id=${currentIdToDelete}`, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    })
        .then(response => {
            if (response.ok) {
                window.location.reload();
            } else {
                alert('Error al eliminar el campo');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Error al eliminar el campo');
        });

    const modal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
    modal.hide();
});