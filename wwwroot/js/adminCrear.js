// Vista previa de imágenes con selección de principal
document.getElementById('imagenesInput').addEventListener('change', function (event) {
    const preview = document.getElementById('imagePreview');
    // No limpiar el preview, mantener las imágenes existentes

    if (this.files.length > 0) {
        // Obtener el número actual de imágenes en el preview
        const currentImageCount = preview.querySelectorAll('.image-container').length;

        for (let i = 0; i < this.files.length; i++) {
            const file = this.files[i];
            if (file.type.match('image.*')) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    const imgContainer = document.createElement('div');
                    imgContainer.className = 'image-container';
                    imgContainer.dataset.fileIndex = currentImageCount + i; // Usar índice único

                    const img = document.createElement('img');
                    img.src = e.target.result;

                    const radioContainer = document.createElement('div');
                    radioContainer.className = 'radio-container';

                    const radio = document.createElement('input');
                    radio.type = 'radio';
                    radio.name = 'imagenPrincipal';
                    radio.id = `imagenPrincipal_${currentImageCount + i}`;
                    radio.value = currentImageCount + i;

                    // Si es la primera imagen, marcarla como principal
                    if (currentImageCount === 0 && i === 0) {
                        radio.checked = true;
                        document.getElementById('imagenPrincipalIndex').value = 0;
                        imgContainer.classList.add('principal-selected');
                    }

                    radio.addEventListener('change', function () {
                        document.querySelectorAll('.image-container').forEach(el => {
                            el.classList.remove('principal-selected');
                        });
                        imgContainer.classList.add('principal-selected');
                        document.getElementById('imagenPrincipalIndex').value = this.value;
                    });

                    const label = document.createElement('label');
                    label.htmlFor = `imagenPrincipal_${currentImageCount + i}`;
                    label.textContent = 'Principal';

                    // Botón para eliminar imagen
                    const deleteBtn = document.createElement('button');
                    deleteBtn.type = 'button';
                    deleteBtn.className = 'btn-delete-image';
                    deleteBtn.innerHTML = '<i class="fas fa-trash"></i>';
                    deleteBtn.addEventListener('click', function () {
                        imgContainer.remove();
                        // Si eliminamos la imagen principal, seleccionar la primera disponible
                        if (radio.checked) {
                            const firstRadio = preview.querySelector('input[type="radio"]');
                            if (firstRadio) {
                                firstRadio.checked = true;
                                firstRadio.dispatchEvent(new Event('change'));
                            } else {
                                document.getElementById('imagenPrincipalIndex').value = '';
                            }
                        }
                    });

                    radioContainer.appendChild(radio);
                    radioContainer.appendChild(label);
                    imgContainer.appendChild(img);
                    imgContainer.appendChild(radioContainer);
                    imgContainer.appendChild(deleteBtn);
                    preview.appendChild(imgContainer);
                }
                reader.readAsDataURL(file);
            }
        }
    }

});