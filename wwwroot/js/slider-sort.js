new Sortable(document.getElementById('sortable-images'), {
    animation: 150,
    ghostClass: 'sortable-ghost',
    onEnd: function () {
        updateOrderBadges();
    }
});

// Botones para mover arriba/abajo
document.querySelectorAll('.btn-move-up').forEach(btn => {
    btn.addEventListener('click', function () {
        const item = this.closest('.col-md-4');
        const prevItem = item.previousElementSibling;
        if (prevItem) {
            item.parentNode.insertBefore(item, prevItem);
            updateOrderBadges();
        }
    });
});

document.querySelectorAll('.btn-move-down').forEach(btn => {
    btn.addEventListener('click', function () {
        const item = this.closest('.col-md-4');
        const nextItem = item.nextElementSibling;
        if (nextItem) {
            item.parentNode.insertBefore(nextItem, item);
            updateOrderBadges();
        }
    });
});

// Guardar el orden
document.getElementById('save-order-btn').addEventListener('click', function () {
    const imageOrder = Array.from(document.querySelectorAll('#sortable-images .col-md-4'))
        .map((el, index) => ({
            fileName: el.dataset.imageName,
            order: index
        }));

    fetch('?handler=SaveImageOrder', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify(imageOrder)
    }).then(response => {
        if (response.ok) {
            showAlert('success', 'Orden guardado correctamente');
            // Hacer scroll al inicio del contenedor principal
            document.querySelector('.admin-container').scrollIntoView({
                behavior: 'smooth'
            });
        } else {
            showAlert('danger', 'Error al guardar el orden');
        }
    });
});

function showAlert(type, message) {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.role = 'alert';
    alertDiv.innerHTML = `
                    ${message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                `;

    const container = document.querySelector('.card');
    if (container) {
        container.prepend(alertDiv);

        // Auto-ocultar después de 5 segundos
        setTimeout(() => {
            alertDiv.classList.remove('show');
            setTimeout(() => alertDiv.remove(), 300);
        }, 3000);
    }
}
