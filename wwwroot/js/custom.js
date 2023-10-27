$(document).ready(function () {
    // Abre la modal para crear o editar evento al hacer clic en el enlace "Nuevo evento"
    $('.nuevo-evento-link').click(function () {
        // Restablece la modal para que sea una nueva creación de evento
        $('#modalEventoTitle').text('Nuevo Evento');
        $('#formEvento')[0].reset();
        $('#modalEvento').modal('show');
    });

    $('#btnGuardarEvento').click(function () {
        var evento = {
            Id: $('#Id').val(),
            Titulo: $('#Titulo').val(),
            Imagen: $('#Imagen').val(),
            Descripcion: $('#Descripcion').val(),
            Fecha: $('#Fecha').val(),
            Hora: $('#Hora').val(),
            PeriodoInscripcion: $('#PeriodoInscripcion').val()
        };

        $.ajax({
            url: '/Home/CrearEvento',
            type: 'POST',
            data: evento,
            success: function (response) {
                if (response.success) {

                    Swal.fire({
                        icon: 'success',
                        title: "Evento guardado",
                        text: response.message,
                    });

                    $('#modalEvento').modal('hide');

                    // Actualizar el título y otros elementos en la vista principal
                    $('#UltimoEventoId').text(response.data.ultimoEvento.Id);
                    $('#UltimoEventoTitulo').text(response.data.ultimoEvento.Titulo);
                    $('#UltimoEventoImagen').attr('src', response.data.ultimoEvento.Imagen);
                    $('#UltimoEventoDescripcion').text(response.data.ultimoEvento.Descripcion);
                    $('#UltimoEventoFecha').text('Fecha: ' + response.data.ultimoEvento.Fecha);
                    $('#UltimoEventoHora').text('Hora: ' + response.data.ultimoEvento.Hora);
                    $('#UltimoEventoPeriodoInscripcion').text('Fecha límite de inscripción: ' + response.data.ultimoEvento.PeriodoInscripcion);

                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message,
                    });
                }
                //location.reload()
            },
            error: function (xhr, status, error) {
                console.log(error);
                alert('Hubo un error al crear el evento.');
                location.reload();
            }
        });

        // Cierra la modal después de guardar
        $('#modalEvento').modal('hide');
        location.reload();
    });

});


$(document).ready(function () {
    $(document).on('click', '.editar-btn', function () {
        var row = $(this).closest('tr');
        row.find('.bando-select, .pago-select, .lista-select').prop('disabled', false);
        row.find('.nombre-input, .nick-input').prop('readonly', false);
        row.addClass('editando');
        row.find('.editar-btn').show();
        row.find('.guardar-btn').hide();
    });

    $(document).ready(function () {
        // Manejar el clic en el botón "Eliminar"
        $('#tablaParticipantes').on('click', '.eliminar-btn', function () {
            var idParticipante = $(this).parents("tr").find("input[name='id']").val(); 

            Swal.fire({
                title: '¿Estás seguro de que quieres eliminar el jugador?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Sí',
                cancelButtonText: 'Cancelar',
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Home/EliminarParticipante',
                        type: 'POST',
                        data: { idParticipante: idParticipante },
                        success: function () {
                            Swal.fire({
                                icon: 'success',
                                title: 'Jugador eliminado',

                            });
                            location.reload();
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'Hubo un error al eliminar el participante',

                            });
                            location.reload();
                        }
                    });
                } else {
                    location.reload();
                }
            });       
        });
                
    });


    // Cuando se hace clic en "Guardar" en una fila de edición
    $(document).on('click', '.guardar-btn', function () {
        var row = $(this).closest('tr');
        // Obtén los valores de los campos editables en esta fila
        var id = row.find('.id-input').val();
        var nombre = row.find('.nombre-input').val();
        var nick = row.find('.nick-input').val();
        var bando = row.find('.bando-select').val();
        var pago = row.find('.pago-select').val();
        var lista = row.find('.lista-select').val();
        row.removeClass('editando');
        row.find('.editar-btn').show();
        row.find('.guardar-btn').hide();

        var url = id == undefined ? "/Home/CrearParticipante" : "/Home/EditarParticipante";

        // Crea un objeto de participante con los valores obtenidos
        var participante = {
            Id: id,
            Nombre: nombre,
            Nick: nick,
            Bando: bando,
            PagoAbonado: pago,
            ListaEnviada: lista
          
        };

        // Envía una solicitud al método AñadirParticipante
        $.ajax({
            url: url,
            type: 'POST',
            data: participante,
            success: function (response) {
                if (response.success) {
                    
                    Swal.fire({
                        icon: 'success',
                        title: "Guardado",
                        text: response.message,
                    });
                    
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message,
                    });
                }
                location.reload()
            },
            error: function (xhr, status, error) {
                console.log(error);
                alert('Hubo un error al agregar el participante.');
                location.reload();
            }
        });

    });

    $(document).on('click', '.emparejamiento-btn', function () {
        // Obtén la lista de jugadores y sus bandos de la tabla
        var jugadores = obtenerListaJugadoresConBando();

        // Verifica que haya al menos dos jugadores para emparejar
        if (jugadores.length < 2) {
            Swal.fire('Error', 'Debe haber al menos dos jugadores para emparejar.', 'error');
            return;
        }

        // Obtén los emparejamientos respetando los bandos
        var emparejamientos = obtenerEmparejamientosConBando(jugadores);

        var modalContent = '<div id="resultadoEmparejamiento">';
        modalContent += '<h2>Emparejamientos</h2>';        
        modalContent += '<ul style="list-style-type: none;">';
        emparejamientos.forEach(function (emparejamiento) {
            modalContent += '<li>' + emparejamiento.Jugador1 + ' vs. ' + emparejamiento.Jugador2 + '</li>';
        });
        modalContent += '</ul>';
        modalContent += '</div>';

        Swal.fire({            
            html: modalContent,
            confirmButtonText: 'Cerrar',
        });
    });

    function obtenerListaJugadoresConBando() {
        var jugadores = [];

        $('#tablaParticipantes tbody tr').each(function () {
            var jugador = {
                Nick: $(this).find('.nick-input').val(),
                Bando: $(this).find('.bando-select').val() === 'true'
            };
            jugadores.push(jugador);
        });

        return jugadores;
    }

    function obtenerEmparejamientosConBando(jugadores) {
        // Separa a los jugadores por bando
        var jugadoresBandoLuz = jugadores.filter(function (jugador) {
            return jugador.Bando === true;
        });

        var jugadoresBandoOscuridad = jugadores.filter(function (jugador) {
            return jugador.Bando === false;
        });

        // Mezcla aleatoriamente los jugadores en cada bando
        var jugadoresMezcladosLuz = shuffleArray(jugadoresBandoLuz);
        var jugadoresMezcladosOscuridad = shuffleArray(jugadoresBandoOscuridad);

        // Crea los emparejamientos alternando entre los bandos
        var emparejamientos = [];

        for (var i = 0; i < Math.min(jugadoresMezcladosLuz.length, jugadoresMezcladosOscuridad.length); i++) {
            var emparejamiento = {
                Jugador1: jugadoresMezcladosLuz[i].Nick,
                Jugador2: jugadoresMezcladosOscuridad[i].Nick
            };
            emparejamientos.push(emparejamiento);
        }

        return emparejamientos;
    }

    function shuffleArray(array) {
        for (var i = array.length - 1; i > 0; i--) {
            var j = Math.floor(Math.random() * (i + 1));
            [array[i], array[j]] = [array[j], array[i]];
        }
        return array;
    }

        
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
                        <option value="True">Luz</option>
                        <option value="False">Oscuridad</option>
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
    $(document).on('input', '.select-puntos-victoria-obtenidos, .select-puntos-victoria-cedidos', function () {
        var row = $(this).closest('tr');
        var puntosObtenidos = parseInt(row.find('.select-puntos-victoria-obtenidos').val());
        var puntosCedidos = parseInt(row.find('.select-puntos-victoria-cedidos').val());
        var diferenciaPuntosVictoria = puntosObtenidos - puntosCedidos;
        row.find('.diferencia-puntos-victoria').text(diferenciaPuntosVictoria);
    });

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

    $(document).on('click', '.editar-btn', function () {
        var row = $(this).closest('tr');
        row.find('.select-nick, .select-punto-partida, .select-puntos-victoria-obtenidos, .select-puntos-victoria-cedidos, .select-lider-abatido, .select-ronda').prop('disabled', false);
        row.addClass('editando');
        row.find('.editar-btn').hide();
        row.find('.guardar-btn').show();
    });

    $(document).ready(function () {        
        $('#tablaPuntuaciones').on('click', '.eliminar-btn', function () {
            var idPuntuacion = $(this).parents("tr").find("input[name='id']").val(); 

            Swal.fire({
                title: '¿Estás seguro de que quieres eliminar la puntuación?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Sí',
                cancelButtonText: 'Cancelar',
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Home/EliminarPuntuacion',
                        type: 'POST',
                        data: { idPuntuacion: idPuntuacion },
                        success: function () {
                            Swal.fire({
                                icon: 'success',
                                title: 'Puntuación eliminada',

                            });
                            location.reload();
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'Hubo un error al eliminar la puntuación',

                            });
                            location.reload();
                        }
                    });
                } else {
                    location.reload();
                }
            });
        });

    });

    $(document).on('click', '.guardar-btn', function () {
        var row = $(this).closest('tr');                
        var id = row.find('.id-input').val();
        var nick = row.find('.select-nick').val();
        var ptoPartida = row.find('.select-punto-partida').val();
        var ptoVicObtenidos = row.find('.select-puntos-victoria-obtenidos').val();
        var ptoVicCedidos = row.find('.select-puntos-victoria-cedidos').val();
        var difPtosVic = row.find('.diferencia-puntos-victoria').val();
        var liderAbatido = row.find('.select-lider-abatido').val();
        var ronda = row.find('.select-ronda').val();
        row.removeClass('editando');
        row.find('.editar-btn').show();
        row.find('.guardar-btn').hide();

        var url = id == undefined ? "/Home/CrearPuntuacion" : "/Home/EditarPuntuacion";

        // Crea un objeto de puntuación con los valores obtenidos
        var puntuacion = {
            Id: id,
            Nick: nick,
            PuntoPartida: ptoPartida,
            PuntosVictoriaObtenidos: ptoVicObtenidos,
            PuntosVictoriaCedidos: ptoVicCedidos,
            DiferenciaPuntosVictoria: difPtosVic,
            LiderAbatido: liderAbatido,
            Ronda: ronda

        };

        // Envía una solicitud al método AñadirPuntuación
        $.ajax({
            url: url, // URL correcta para la operación
            type: 'POST',
            data: puntuacion,
            success: function (response) {
                if (response.success) {

                    Swal.fire({
                        icon: 'success',
                        title: "Puntuación guardada",
                        text: response.message,
                    });

                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message,
                    });
                }
                location.reload()
            },
            error: function (xhr, status, error) {
                console.log(error);
                alert('Hubo un error al agregar la puntuacion.');
                location.reload();
            }
        });

    });    

    $(document).on('click', '.nueva-puntuacion-btn', function () {
        // Obtener la lista de jugadores disponibles desde el elemento oculto
        var jugadoresDisponibles = $('#jugadores-disponibles').text().split(',');
        console.log("dentro");
        var newRowHtml = `
        <tr>
            <td>
                <select class="form-control select-nick editable-select">
                    <option value="">Seleccione un jugador</option>`;

        // Generar opciones para cada jugador disponible
        jugadoresDisponibles.forEach(function (nick) {
            newRowHtml += `
            <option value="${nick}">${nick}</option>`;
        });

        newRowHtml += `
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
        newRow.find('.select-nick, .select-punto-partida, .select-puntos-victoria-obtenidos, .select-puntos-victoria-cedidos, .select-lider-abatido, .select-ronda').prop('disabled', false);
        newRow.addClass('editando');
        newRow.find('.editar-btn').hide();
        newRow.find('.guardar-btn').show();
        
        actualizarOpcionesFiltroRonda();
    });    

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
    
});


















 






























    
    













































