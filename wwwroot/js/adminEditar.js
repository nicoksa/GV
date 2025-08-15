



// Vista previa de imágenes con selección de principal - Versión unificada para crear/editar
document.addEventListener('DOMContentLoaded', function () {



    const imagenesInput = document.getElementById('imagenesInput');
    const preview = document.getElementById('imagePreview');
    const imagenPrincipalIndex = document.getElementById('imagenPrincipalIndex');

    function wireExistingPrincipalRadios() {
        const radios = document.querySelectorAll('#imagePreview input[name="imagenPrincipalRadio"]');
        radios.forEach(radio => {
            radio.addEventListener('change', function () {
                // Actualiza el hidden
                imagenPrincipalIndex.value = this.value;

                // Marca visualmente la seleccionada
                document.querySelectorAll('#imagePreview .image-container').forEach(el => el.classList.remove('principal-selected'));
                const cont = this.closest('.image-container');
                if (cont) cont.classList.add('principal-selected');
            });
        });
    }



    let allFiles = [];

    // Inicializar con imágenes existentes si las hay
    const existingImages = Array.from(preview.querySelectorAll('.image-container[data-image-id]'));
    const existingCount = existingImages.length;

    wireExistingPrincipalRadios();

    imagenesInput.addEventListener('change', function (event) {
        if (this.files && this.files.length > 0) {
            // Agregar nuevos archivos al array existente
            const newFiles = Array.from(this.files);
            allFiles = [...allFiles, ...newFiles];

            // Actualizar el input de archivos
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
        // Mantener las imágenes existentes
        const existingContainers = Array.from(preview.querySelectorAll('.image-container[data-image-id]'));
        preview.innerHTML = '';
        existingContainers.forEach(container => preview.appendChild(container));

        // Añadir las nuevas imágenes
        allFiles.forEach((file, index) => {
            if (file.type.match('image.*')) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    const imgContainer = document.createElement('div');
                    imgContainer.className = 'image-container';
                    imgContainer.dataset.fileIndex = existingCount + index;

                    const img = document.createElement('img');
                    img.src = e.target.result;

                    const radioContainer = document.createElement('div');
                    radioContainer.className = 'radio-container';

                    const radio = document.createElement('input');
                    radio.type = 'radio';
                    radio.name = 'imagenPrincipalRadio';
                    radio.id = `imagenPrincipal_${existingCount + index}`;
                    radio.value = existingCount + index;

                    // Marcar como principal si coincide con el índice guardado
                    if (imagenPrincipalIndex.value == (existingCount + index).toString()) {
                        radio.checked = true;
                        imgContainer.classList.add('principal-selected');
                    }

                    radio.addEventListener('change', function () {
                        document.querySelectorAll('.image-container').forEach(el => {
                            el.classList.remove('principal-selected');
                        });
                        imgContainer.classList.add('principal-selected');
                        imagenPrincipalIndex.value = this.value;
                    });

                    const label = document.createElement('label');
                    label.htmlFor = `imagenPrincipal_${existingCount + index}`;
                    label.textContent = 'Principal';

                    const deleteBtn = document.createElement('button');
                    deleteBtn.type = 'button';
                    deleteBtn.className = 'btn-delete-image';
                    deleteBtn.innerHTML = '<i class="fas fa-trash"></i>';
                    deleteBtn.addEventListener('click', function () {
                        // Eliminar del array
                        allFiles.splice(index, 1);
                        updateFileInput();
                        renderPreview();

                        // Actualizar selección principal si era la principal
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
                    wireExistingPrincipalRadios();
                };
                
                reader.readAsDataURL(file);
            }
        });
    }

    // Configurar eliminación de imágenes existentes
    document.querySelectorAll('.btn-delete-image[data-image-id]').forEach(btn => {
        btn.addEventListener('click', function () {
            const imageId = this.dataset.imageId;
            if (confirm('¿Eliminar esta imagen permanentemente?')) {
                fetch(`?handler=DeleteImage&imageId=${imageId}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                }).then(response => {
                    if (response.ok) {
                        this.closest('.image-container').remove();
                        updateImageIndices();
                    } else {
                        alert('Error al eliminar la imagen');
                    }
                });
            }
        });
    });

    function updateImageIndices() {
        const containers = Array.from(preview.querySelectorAll('.image-container'));
        let principalIndex = -1;

        containers.forEach((container, index) => {
            // Actualizar índices
            container.dataset.index = index;
            const radio = container.querySelector('input[type="radio"]');
            if (radio) {
                radio.value = index;
                radio.id = `imagenPrincipal_${index}`;
                radio.nextElementSibling.htmlFor = `imagenPrincipal_${index}`;

                if (radio.checked) {
                    principalIndex = index;
                }
            }
        });

        // Actualizar el campo oculto
        if (principalIndex >= 0) {
            imagenPrincipalIndex.value = principalIndex;
        } else if (containers.length > 0) {
            // Seleccionar primera imagen si no hay principal
            const firstRadio = containers[0].querySelector('input[type="radio"]');
            if (firstRadio) {
                firstRadio.checked = true;
                containers[0].classList.add('principal-selected');
                imagenPrincipalIndex.value = 0;
            }
        }
    }


    function setupYouTubePreview() {
    const youtubeInput = document.getElementById('youtubeUrlInput');
    const preview = document.getElementById('videoPreview');

    if (!youtubeInput || !preview) {
        return; // Si no existen en la página, no hace nada
    }

    // Función que actualiza la vista previa
    function updatePreview(url) {
        preview.innerHTML = '';
        if (url) {
            const videoId = extractYouTubeId(url);
            if (videoId) {
                const iframe = document.createElement('iframe');
                iframe.width = '100%';
                iframe.height = '200';
                iframe.src = `https://www.youtube.com/embed/${videoId}`;
                iframe.frameBorder = '0';
                iframe.allow =
                    'accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture';
                iframe.allowFullscreen = true;
                preview.appendChild(iframe);
            }
        }
    }

    function extractYouTubeId(url) {
        const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/;
        const match = url.match(regExp);
        return match && match[2].length === 11 ? match[2] : null;
    }

    // Escuchar cambios mientras escribís
    youtubeInput.addEventListener('input', function () {
        updatePreview(this.value.trim());
    });

    // Mostrar vista previa inicial si ya hay valor
    if (youtubeInput.value.trim() !== '') {
        updatePreview(youtubeInput.value.trim());
    }
}
setupYouTubePreview();
});







