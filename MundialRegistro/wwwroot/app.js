const estado = {
    partidos: [],
    partidoSeleccionado: null,
    asientoSeleccionado: ''
};

const form = document.getElementById('registroForm');
const fechaJuego = document.getElementById('fechaJuego');
const partidoId = document.getElementById('partidoId');
const asientoInput = document.getElementById('asiento');
const seatMap = document.getElementById('seatMap');
const alerta = document.getElementById('alerta');
const comprobante = document.getElementById('comprobante');
const imprimirBtn = document.getElementById('imprimirBtn');
const nuevoBtn = document.getElementById('nuevoBtn');

const filas = ['A', 'B', 'C', 'D', 'E', 'F'];
const columnas = 12;

iniciar();

async function iniciar() {
    renderSeatMap([]);
    await cargarPartidos();

    fechaJuego.addEventListener('change', manejarCambioFecha);
    partidoId.addEventListener('change', manejarCambioPartido);
    form.addEventListener('submit', registrarPersona);
    imprimirBtn.addEventListener('click', () => window.print());
    nuevoBtn.addEventListener('click', nuevoRegistro);
}

async function cargarPartidos() {
    try {
        const respuesta = await fetch('/api/partidos');
        if (!respuesta.ok) throw new Error('No se pudieron cargar los partidos.');

        estado.partidos = await respuesta.json();
        renderFechas();
    } catch (error) {
        mostrarAlerta(error.message);
    }
}

function renderFechas() {
    const fechasUnicas = [...new Map(estado.partidos.map(p => [p.fecha, p])).values()];

    fechaJuego.innerHTML = '<option value="">Seleccione una fecha</option>';
    fechasUnicas.forEach(partido => {
        const option = document.createElement('option');
        option.value = partido.fecha;
        option.textContent = partido.fechaTexto;
        fechaJuego.appendChild(option);
    });
}

function manejarCambioFecha() {
    const fecha = fechaJuego.value;
    const partidosDelDia = estado.partidos.filter(p => p.fecha === fecha);

    partidoId.innerHTML = '<option value="">Seleccione un partido</option>';
    partidoId.disabled = partidosDelDia.length === 0;
    limpiarSeleccionAsiento();
    renderSeatMap([]);

    partidosDelDia.forEach(partido => {
        const option = document.createElement('option');
        option.value = partido.id;
        option.textContent = `${partido.hora} - ${partido.equipoLocal} vs ${partido.equipoVisitante}`;
        partidoId.appendChild(option);
    });
}

async function manejarCambioPartido() {
    const id = Number(partidoId.value);
    estado.partidoSeleccionado = estado.partidos.find(p => p.id === id) || null;
    limpiarSeleccionAsiento();

    if (!id) {
        renderSeatMap([]);
        return;
    }

    try {
        const respuesta = await fetch(`/api/partidos/${id}/asientos`);
        if (!respuesta.ok) throw new Error('No se pudieron cargar los asientos.');

        const data = await respuesta.json();
        renderSeatMap(data.asientosOcupados || []);
    } catch (error) {
        mostrarAlerta(error.message);
    }
}

function renderSeatMap(asientosOcupados) {
    seatMap.innerHTML = '';
    const ocupados = new Set(asientosOcupados);

    filas.forEach(fila => {
        for (let numero = 1; numero <= columnas; numero += 1) {
            const codigo = `${fila}${numero}`;
            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'seat';
            btn.textContent = codigo;
            btn.disabled = !estado.partidoSeleccionado || ocupados.has(codigo);
            btn.setAttribute('aria-label', `Asiento ${codigo}`);

            if (ocupados.has(codigo)) {
                btn.classList.add('occupied');
                btn.title = 'Asiento ocupado';
            }

            btn.addEventListener('click', () => seleccionarAsiento(codigo, btn));
            seatMap.appendChild(btn);
        }
    });
}

function seleccionarAsiento(codigo, boton) {
    document.querySelectorAll('.seat.selected').forEach(seat => seat.classList.remove('selected'));
    boton.classList.add('selected');
    estado.asientoSeleccionado = codigo;
    asientoInput.value = codigo;
}

function limpiarSeleccionAsiento() {
    estado.asientoSeleccionado = '';
    asientoInput.value = '';
    document.querySelectorAll('.seat.selected').forEach(seat => seat.classList.remove('selected'));
}

async function registrarPersona(event) {
    event.preventDefault();
    ocultarAlerta();

    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    if (!estado.asientoSeleccionado) {
        mostrarAlerta('Debe seleccionar un asiento disponible.');
        return;
    }

    const datos = Object.fromEntries(new FormData(form).entries());
    datos.partidoId = Number(datos.partidoId);

    try {
        const respuesta = await fetch('/api/registros', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(datos)
        });

        const resultado = await respuesta.json();

        if (!respuesta.ok) {
            const mensaje = resultado.errores ? resultado.errores.join(' ') : resultado.mensaje;
            throw new Error(mensaje || 'No se pudo realizar el registro.');
        }

        mostrarComprobante(resultado);
        await manejarCambioPartido();
    } catch (error) {
        mostrarAlerta(error.message);
    }
}

function mostrarComprobante(data) {
    document.getElementById('codigoComprobante').textContent = data.codigo;
    document.getElementById('ticketNombre').textContent = `${data.persona.nombres} ${data.persona.apellidos}`;
    document.getElementById('ticketDocumento').textContent = `${data.persona.tipoDocumento}: ${data.persona.documento}`;
    document.getElementById('ticketPartido').textContent = `${data.partido.equipoLocal} vs ${data.partido.equipoVisitante}`;
    document.getElementById('ticketFecha').textContent = `${data.partido.fecha} - ${data.partido.hora}`;
    document.getElementById('ticketEstadio').textContent = data.partido.estadio;
    document.getElementById('ticketAsiento').textContent = data.asiento;
    document.getElementById('ticketRegistro').textContent = data.fechaRegistro;

    comprobante.hidden = false;
    comprobante.scrollIntoView({ behavior: 'smooth', block: 'start' });
}

function nuevoRegistro() {
    form.reset();
    partidoId.disabled = true;
    partidoId.innerHTML = '<option value="">Seleccione un partido</option>';
    estado.partidoSeleccionado = null;
    limpiarSeleccionAsiento();
    renderSeatMap([]);
    comprobante.hidden = true;
    ocultarAlerta();
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function mostrarAlerta(mensaje) {
    alerta.textContent = mensaje;
    alerta.hidden = false;
}

function ocultarAlerta() {
    alerta.textContent = '';
    alerta.hidden = true;
}
