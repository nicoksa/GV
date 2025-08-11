
// Vista previa de imágenes con selección de principal - Versión mejorada
document.addEventListener('DOMContentLoaded', function () {
    const imagenesInput = document.getElementById('imagenesInput');
    const preview = document.getElementById('imagePreview');
    const imagenPrincipalIndex = document.getElementById('imagenPrincipalIndex');
    let allFiles = []; // Almacena todos los archivos seleccionados

    imagenesInput.addEventListener('change', function (event) {
        if (this.files && this.files.length > 0) {
            // Agregar nuevos archivos al array existente
            const newFiles = Array.from(this.files);
            allFiles = [...allFiles, ...newFiles];

            // Actualizar el input de archivos con todos los seleccionados
            updateFileInput();

            // Regenerar la vista previa
            renderPreview();
        }
    });

    function updateFileInput() {
        const dataTransfer = new DataTransfer();
        allFiles.forEach(file => dataTransfer.items.add(file));
        imagenesInput.files = dataTransfer.files;
    }

    function renderPreview() {
        preview.innerHTML = '';

        allFiles.forEach((file, index) => {
            if (file.type.match('image.*')) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    const imgContainer = document.createElement('div');
                    imgContainer.className = 'image-container';
                    imgContainer.dataset.fileIndex = index;

                    const img = document.createElement('img');
                    img.src = e.target.result;

                    const radioContainer = document.createElement('div');
                    radioContainer.className = 'radio-container';

                    const radio = document.createElement('input');
                    radio.type = 'radio';
                    radio.name = 'imagenPrincipal';
                    radio.id = `imagenPrincipal_${index}`;
                    radio.value = index;

                    // Marcar como principal si es el índice guardado o la primera imagen
                    if (index == imagenPrincipalIndex.value || (index === 0 && !imagenPrincipalIndex.value)) {
                        radio.checked = true;
                        imgContainer.classList.add('principal-selected');
                        imagenPrincipalIndex.value = index;
                    }

                    radio.addEventListener('change', function () {
                        document.querySelectorAll('.image-container').forEach(el => {
                            el.classList.remove('principal-selected');
                        });
                        imgContainer.classList.add('principal-selected');
                        imagenPrincipalIndex.value = this.value;
                    });

                    const label = document.createElement('label');
                    label.htmlFor = `imagenPrincipal_${index}`;
                    label.textContent = 'Principal';

                    // Botón para eliminar imagen (manteniendo tus estilos)
                    const deleteBtn = document.createElement('button');
                    deleteBtn.type = 'button';
                    deleteBtn.className = 'btn-delete-image';
                    deleteBtn.innerHTML = '<i class="fas fa-trash"></i>';
                    deleteBtn.addEventListener('click', function () {
                        // Eliminar el archivo del array
                        allFiles.splice(index, 1);

                        // Actualizar el input de archivos
                        updateFileInput();

                        // Regenerar la vista previa
                        renderPreview();

                        // Si eliminamos la imagen principal, seleccionar la primera disponible
                        if (radio.checked) {
                            const firstRadio = preview.querySelector('input[type="radio"]');
                            if (firstRadio) {
                                firstRadio.checked = true;
                                firstRadio.dispatchEvent(new Event('change'));
                            } else {
                                imagenPrincipalIndex.value = '';
                            }
                        }
                    });

                    radioContainer.appendChild(radio);
                    radioContainer.appendChild(label);
                    imgContainer.appendChild(img);
                    imgContainer.appendChild(radioContainer);
                    imgContainer.appendChild(deleteBtn);
                    preview.appendChild(imgContainer);
                };
                reader.readAsDataURL(file);
            }
        });
    }
});



