document.addEventListener('DOMContentLoaded', function () {
    const tipoSelect = document.getElementById('Propiedad_Tipo');
    const ambientes = document.getElementById('Propiedad_Ambientes');
    const dormitorios = document.getElementById('Propiedad_Dormitorios');
    const banios = document.getElementById('Propiedad_Banios');
    const superficieCubierta = document.getElementById('Propiedad_SuperficieCubierta');

    // Comodidades
    const cochera = document.getElementById('Propiedad_Cochera');
    const patio = document.getElementById('Propiedad_Patio');
    const aireAcond = document.getElementById('Propiedad_AireAcondicionado');
    const seguridad = document.getElementById('Propiedad_Seguridad');
    const pileta = document.getElementById('Propiedad_Pileta');

    function actualizarCampos() {
        const esTerreno = tipoSelect.value === 'Terreno';
        ambientes.disabled = esTerreno;
        dormitorios.disabled = esTerreno;
        banios.disabled = esTerreno;
        superficieCubierta.disabled = esTerreno;

        cochera.disabled = esTerreno;
        patio.disabled = esTerreno;
        aireAcond.disabled = esTerreno;
        seguridad.disabled = esTerreno;
        pileta.disabled = esTerreno;
    }

    tipoSelect.addEventListener('change', actualizarCampos);
    actualizarCampos(); // Ejecuta al cargar la página
});