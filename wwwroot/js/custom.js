$(document).ready(function () {
    $(document).on('click', '.editar-btn', function () {
        var row = $(this).closest('tr');
        row.find('.bando-select, .pago-select, .lista-select').prop('disabled', false);
        row.find('.nombre-input, .nick-input').prop('readonly', false);
        row.addClass('editando');
        row.find('.editar-btn').hide();
        row.find('.guardar-btn').show();
    });

    $(document).on('click', '.eliminar-btn', function () {
        var row = $(this).closest('tr');
        row.remove();
    });

    $(document).on('click', '.guardar-btn', function () {
        var row = $(this).closest('tr');
        row.find('.bando-select, .pago-select, .lista-select').prop('disabled', true);
        row.find('.nombre-input, .nick-input').prop('readonly', true);
        row.removeClass('editando');
        row.find('.editar-btn').show();
        row.find('.guardar-btn').hide();
    });

    $(document).on('click', '.nuevo-jugador-btn', function () {
        var newRowHtml = `
            <tr>
                <td>
                    <input type="text" class="form-control nombre-input editable-input" />
                </td>
                <td>
                    <input type="text" class="form-control nick-input editable-input" />
                </td>
                <td>
                    <select class="form-control bando-select editable-select">
                        <option value="Luz">Luz</option>
                        <option value="Oscuridad">Oscuridad</option>
                    </select>
                </td>
                <td>
                    <select class="form-control pago-select editable-select">
                        <option value="True">Sí</option>
                        <option value="False">No</option>
                    </select>
                </td>
                <td>
                    <select class="form-control lista-select editable-select">
                        <option value="True">Sí</option>
                        <option value="False">No</option>
                    </select>
                </td>
                <td>
                    <button class="btn btn-link editar-btn" title="Editar"><i class="far fa-edit"></i></button>
                    <button class="btn btn-link eliminar-btn" title="Eliminar"><i class="far fa-trash-alt"></i></button>
                    <button class="btn btn-link guardar-btn" title="Guardar" style="display: none;"><i class="far fa-save"></i></button>
                </td>
            </tr>
        `;

        // Agregar la nueva fila al final de la tabla
        $('table tbody').append(newRowHtml);

        // Habilitar la edición en la nueva fila
        $('table tbody tr:last-child').find('.bando-select, .pago-select, .lista-select').prop('disabled', false);
        $('table tbody tr:last-child').find('.nombre-input, .nick-input').prop('readonly', false);
        $('table tbody tr:last-child').addClass('editando');
        $('table tbody tr:last-child').find('.editar-btn').hide();
        $('table tbody tr:last-child').find('.guardar-btn').show();
    });
});

$(document).ready(function () {
    function actualizarDiferenciaPuntosVictoria(row) {
        var puntosObtenidos = parseInt(row.find('.select-puntos-victoria-obtenidos').val());
        var puntosCedidos = parseInt(row.find('.select-puntos-victoria-cedidos').val());
        var diferenciaPuntosVictoria = puntosObtenidos - puntosCedidos;
        row.find('.diferencia-puntos-victoria').text(diferenciaPuntosVictoria);
    }

    function actualizarOpcionesFiltroRonda() {
        var rondas = obtenerRondasDisponibles();
        var filtroRonda = $('#filtro-ronda');

        var rondaSeleccionada = filtroRonda.val() || ''; // Guardamos el valor seleccionado antes de actualizar las opciones

        filtroRonda.empty();
        filtroRonda.append('<option value="">Todas las Rondas</option>');

        rondas.forEach(function (ronda) {
            filtroRonda.append('<option value="' + ronda + '">' + ronda + '</option>');
        });

        filtroRonda.val(rondaSeleccionada); // Restauramos el valor seleccionado
    }

    actualizarOpcionesFiltroRonda();

    $(document).on('change', '.select-ronda', function () {
        actualizarOpcionesFiltroRonda();
        filtrarPorRonda($('#filtro-ronda').val()); // Filtrar al cambiar la selección del filtro
    });

    $('#filtro-ronda').on('change', function () {
        filtrarPorRonda($(this).val()); // Filtrar al cambiar la selección del filtro
    });

    $(document).on('input', '.select-puntos-victoria-obtenidos, .select-puntos-victoria-cedidos', function () {
        var row = $(this).closest('tr');
        actualizarDiferenciaPuntosVictoria(row);
    });

    $(document).on('click', '.editar-btn', function () {
        var row = $(this).closest('tr');
        row.find('.select-jugador, .select-punto-partida, .select-puntos-victoria-obtenidos, .select-puntos-victoria-cedidos, .select-lider-abatido, .select-ronda').prop('disabled', false);
        row.addClass('editando');
        row.find('.editar-btn').hide();
        row.find('.guardar-btn').show();
    });

    $(document).on('click', '.eliminar-btn', function () {
        var row = $(this).closest('tr');
        row.remove();
        actualizarOpcionesFiltroRonda();
        filtrarPorRonda($('#filtro-ronda').val()); // Filtrar al eliminar una fila
    });

    $(document).on('click', '.guardar-btn', function () {
        var row = $(this).closest('tr');
        row.find('.select-jugador, .select-punto-partida, .select-puntos-victoria-obtenidos, .select-puntos-victoria-cedidos, .select-lider-abatido, .select-ronda').prop('disabled', true);
        row.removeClass('editando');
        row.find('.editar-btn').show();
        row.find('.guardar-btn').hide();
    });

    $(document).on('click', '.nueva-puntuacion-btn', function () {
        var jugadoresDisponibles = obtenerJugadoresDisponibles();
        var newRowHtml = `
      <tr>
        <td>
          <select class="form-control select-jugador editable-select">
            ${generarOpcionesSelect(jugadoresDisponibles)}
          </select>
        </td>
        <td>
          <select class="form-control select-punto-partida editable-select">
            <option value="3">3</option>
            <option value="1">1</option>
            <option value="0">0</option>
          </select>
        </td>
        <td>
          <select class="form-control select-puntos-victoria-obtenidos editable-select">
            ${generarOpcionesSelect(0, 100)}
          </select>
        </td>
        <td>
          <select class="form-control select-puntos-victoria-cedidos editable-select">
            ${generarOpcionesSelect(0, 100)}
          </select>
        </td>
        <td>
          <span class="diferencia-puntos-victoria"></span>
        </td>
        <td>
          <select class="form-control select-lider-abatido editable-select">
            ${generarOpcionesSelect(0, 100)}
          </select>
        </td>
        <td>
          <select class="form-control select-ronda editable-select">
            ${generarOpcionesSelect(0, 100)}
          </select>
        </td>
        <td>
          <button class="btn btn-link editar-btn" title="Editar"><i class="far fa-edit"></i></button>
          <button class="btn btn-link eliminar-btn" title="Eliminar"><i class="far fa-trash-alt"></i></button>
          <button class="btn btn-link guardar-btn" title="Guardar" style="display: none;"><i class="far fa-save"></i></button>
        </td>
      </tr>
    `;

        // Agregar la nueva fila al final de la tabla
        $('table tbody').append(newRowHtml);

        // Habilitar la edición en la nueva fila
        var newRow = $('table tbody tr:last-child');
        newRow.find('.select-jugador, .select-punto-partida, .select-puntos-victoria-obtenidos, .select-puntos-victoria-cedidos, .select-lider-abatido, .select-ronda').prop('disabled', false);
        newRow.addClass('editando');
        newRow.find('.editar-btn').hide();
        newRow.find('.guardar-btn').show();

        // Asignar evento 'input' a los nuevos elementos
        newRow.find('.select-puntos-victoria-obtenidos, .select-puntos-victoria-cedidos').on('input', function () {
            actualizarDiferenciaPuntosVictoria(newRow);
        });

        actualizarOpcionesFiltroRonda();
    });

    function obtenerJugadoresDisponibles() {
        var jugadores = [];
        $('.select-jugador').each(function () {
            var jugadorSeleccionado = $(this).val();
            if (jugadorSeleccionado !== '') {
                jugadores.push(jugadorSeleccionado);
            }
        });
        return jugadores;
    }

    function obtenerRondasDisponibles() {
        var rondas = new Set();
        $('.select-ronda').each(function () {
            var rondaSeleccionada = $(this).val();
            if (rondaSeleccionada !== '') {
                rondas.add(rondaSeleccionada);
            }
        });
        return Array.from(rondas);
    }

    function generarOpcionesSelect(inicio, fin) {
        var opciones = '';
        for (var i = inicio; i <= fin; i++) {
            opciones += `<option value="${i}">${i}</option>`;
        }
        return opciones;
    }

    function filtrarPorRonda(rondaSeleccionada) {
        if (rondaSeleccionada !== '') {
            $('table tbody tr').each(function () {
                var ronda = $(this).find('.select-ronda').val();
                if (ronda === rondaSeleccionada) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });
        } else {
            $('table tbody tr').show();
        }
    }


    // Calcular la diferencia de puntos de victoria al cargar la página
    actualizarDiferenciaPuntosVictoria($('table tbody tr'));
});

















 






























    
    













































