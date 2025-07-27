// Función para extraer el ID de YouTube
function extractYouTubeId(url) {
    const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|&v=)([^#&?]*).*/;
    const match = url.match(regExp);
    return (match && match[2].length === 11) ? match[2] : null;
}

// Función para actualizar todos los índices y la imagen principal
function updateImageIndices() {
    const containers = document.querySelectorAll('.image-container');
    let principalIndex = -1;

    containers.forEach((container, index) => {
        // Actualizar el índice en el elemento
        container.dataset.index = index;

        // Actualizar el radio button
        const radio = container.querySelector('input[type="radio"]');
        if (radio) {
            radio.value = index;
            radio.id = `imagenPrincipal_${index}`;

            // Actualizar label asociado
            const label = container.querySelector('label');
            if (label) {
                label.htmlFor = `imagenPrincipal_${index}`;
            }

            // Verificar si es la imagen principal
            if (radio.checked) {
                principalIndex = index;
            }
        }
    });

    // Actualizar el valor del campo oculto
    if (principalIndex >= 0) {
        document.getElementById('imagenPrincipalIndex').value = principalIndex;
    } else if (containers.length > 0) {
        // Si no hay principal seleccionada pero hay imágenes, seleccionar la primera
        const firstRadio = containers[0].querySelector('input[type="radio"]');
        if (firstRadio) {
            firstRadio.checked = true;
            containers[0].classList.add('principal-selected');
            document.getElementById('imagenPrincipalIndex').value = 0;
        }
    }
}

// Configurar el manejador de eventos para la carga de imágenes
function setupImageUpload(existingImagesCount) {
    const imagenesInput = document.getElementById('imagenesInput');
    if (!imagenesInput) return;

    imagenesInput.addEventListener('change', function (event) {
        const preview = document.getElementById('imagePreview');
        const files = this.files;

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            if (!file.type.match('image.*')) continue;

            const reader = new FileReader();
            reader.onload = function (e) {
                const index = existingImagesCount + i;
                const imgContainer = document.createElement('div');
                imgContainer.className = 'image-container';
                imgContainer.dataset.index = index.toString();
                imgContainer.dataset.isNew = "true";

                const img = document.createElement('img');
                img.src = e.target.result;

                const radioContainer = document.createElement('div');
                radioContainer.className = 'radio-container';

                const radio = document.createElement('input');
                radio.type = 'radio';
                radio.name = 'imagenPrincipalRadio';
                radio.id = `imagenPrincipal_${index}`;
                radio.value = index.toString();

                const label = document.createElement('label');
                label.htmlFor = `imagenPrincipal_${index}`;
                label.textContent = 'Principal';

                // Botón de eliminar para nuevas imágenes
                const deleteBtn = document.createElement('button');
                deleteBtn.type = 'button';
                deleteBtn.className = 'btn-delete-image';
                deleteBtn.innerHTML = '<i class="fas fa-trash"></i>';
                deleteBtn.onclick = function () {
                    if (confirm('¿Eliminar esta imagen?')) {
                        imgContainer.remove();
                        updateImageIndices();
                    }
                };

                radioContainer.appendChild(radio);
                radioContainer.appendChild(label);
                imgContainer.appendChild(img);
                imgContainer.appendChild(radioContainer);
                imgContainer.appendChild(deleteBtn);
                preview.appendChild(imgContainer);

                // Si es la primera imagen, marcarla como principal
                if (existingImagesCount === 0 && i === 0) {
                    radio.checked = true;
                    imgContainer.classList.add('principal-selected');
                    document.getElementById('imagenPrincipalIndex').value = 0;
                }
            };
            reader.readAsDataURL(file);
        }

        updateImageIndices();
    });
}

// Configurar el manejador de eventos para eliminar imágenes
function setupImageDeletion(propiedadId, token, tipoPropiedad) {
    document.querySelectorAll('.btn-delete-image').forEach(btn => {
        btn.addEventListener('click', function () {
            const imageId = this.dataset.imageId;
            const container = this.closest('.image-container');
            const wasPrincipal = container.querySelector('input[type="radio"]')?.checked;

            if (confirm('¿Eliminar esta imagen?')) {
                const endpoint = tipoPropiedad === 'campo'
                    ? `/Admin/EditarCampo/${propiedadId}?handler=DeleteImage&imageId=${imageId}`
                    : `/Admin/EditarPropiedad/${propiedadId}?handler=DeleteImage&imageId=${imageId}`;

                fetch(endpoint, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token,
                        'Content-Type': 'application/x-www-form-urlencoded'
                    }
                }).then(response => {
                    if (response.ok) {
                        container.remove();
                        // Si era la principal, seleccionar la primera imagen restante
                        if (wasPrincipal) {
                            const firstRadio = document.querySelector('input[type="radio"]');
                            if (firstRadio) {
                                firstRadio.checked = true;
                                firstRadio.closest('.image-container').classList.add('principal-selected');
                            }
                        }
                        updateImageIndices();
                    } else {
                        alert('Error al eliminar la imagen');
                    }
                });
            }
        });
    });
}

// Configurar el manejador de eventos para cambiar la imagen principal
function setupPrincipalImageSelection() {
    document.addEventListener('change', function (e) {
        if (e.target && e.target.name === 'imagenPrincipalRadio') {
            document.querySelectorAll('.image-container').forEach(el => {
                el.classList.remove('principal-selected');
            });
            e.target.closest('.image-container').classList.add('principal-selected');
            document.getElementById('imagenPrincipalIndex').value = e.target.value;
        }
    });
}

// Configurar el manejador de eventos para la vista previa de YouTube
function setupYouTubePreview() {
    const youtubeUrlInput = document.getElementById('youtubeUrlInput');
    if (!youtubeUrlInput) return;

    youtubeUrlInput.addEventListener('input', function (event) {
        const preview = document.getElementById('videoPreview');
        preview.innerHTML = '';

        const url = this.value.trim();
        if (url) {
            const videoId = extractYouTubeId(url);
            if (videoId) {
                const iframe = document.createElement('iframe');
                iframe.width = '100%';
                iframe.height = '200';
                iframe.src = `https://www.youtube.com/embed/${videoId}`;
                iframe.frameBorder = '0';
                iframe.allow = 'accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture';
                iframe.allowFullscreen = true;
                preview.appendChild(iframe);
            }
        }
    });
}

// Inicializar todos los componentes
function initAdminEditar(existingImagesCount, propiedadId, token, tipoPropiedad) {
    document.addEventListener('DOMContentLoaded', function () {
        updateImageIndices();
        setupImageUpload(existingImagesCount);
        setupImageDeletion(propiedadId, token, tipoPropiedad);
        setupPrincipalImageSelection();
        setupYouTubePreview();
    });
}