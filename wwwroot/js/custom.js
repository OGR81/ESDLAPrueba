$(document).ready(function () {
    var nuevoEvento = undefined;

    // Abre la modal para crear o editar evento al hacer clic en el enlace "Nuevo evento"
    $('.nuevo-evento-link').on('click', (function (e) {

        // Restablece la modal para que sea una nueva creación de evento
        $('#modalEventoTitle').text('Nuevo Evento');
        $('#formEvento')[0].reset();
        document.getElementById("modalEventoTitle").innerHTML = "Nuevo evento";
        $('#modalEvento').modal('show');

        document.addEventListener('DOMContentLoaded', function () {
            const images = document.querySelector('img.img-fluid.selectable-image');

            images.forEach(img => {
                img.addEventListener('click', function () {
                    alert("hola");
                    // Primero, quitamos el borde de todas las imágenes
                    images.forEach(innerImg => {
                        innerImg.style.border = "none";
                    });

                    // Ahora, marcamos la imagen seleccionada
                    img.style.border = "4px solid red";
                });
            });
        });

        nuevoEvento = true;
    }));

    $(".selectable-image").click(function () {
        $(".selectable-image").removeClass("selected");  // Quitamos la clase 'selected' de todas las imágenes
        $(this).addClass("selected");  // Añadimos la clase 'selected' a la imagen clicada
    });

    document.addEventListener('DOMContentLoaded', function () {
        const images = document.querySelector('div.image-container.selectable-image');

        images.forEach(img => {
            img.addEventListener('click', function () {
                // Primero, quitamos el borde de todas las imágenes
                images.forEach(innerImg => {
                    innerImg.style.border = "none";
                });

                // Ahora, marcamos la imagen seleccionada
                img.style.border = "4px solid red";
            });
        });
    });

    $('.modificar-evento-link').on('click', (function (e) {

        var idDocumento = $("#UltimoEvento_Id").val();
        nuevoEvento = false;
        $.ajax({
            url: "/Home/ObtenerEvento",
            data: { idDocumento: idDocumento },
            type: 'get',
            cache: false
        }).done((result) => {
            var fechaEvento = result.evento.fechaEvento.split("/").reverse().join("-");
            var periodoInscripcion = result.evento.periodoInscripcion.split("/").reverse().join("-");
            document.getElementById("modalEventoTitle").innerHTML = "Modificar evento";
            
            $(".modal-body #Titulo").val(result.evento.titulo);
            $(".modal-body #Descripcion").val(result.evento.descripcion);        
            $(".modal-body #FechaEvento").val(fechaEvento);
            $(".modal-body #HoraEvento").val(result.evento.horaEvento);           
            $(".modal-body #PeriodoInscripcion").val(periodoInscripcion);

            const imgElement = document.querySelector(`div.image-container img[src*="images/ImagenesEventos/${result.evento.imagen}"]`)

            if (imgElement)
                imgElement.style.border = "4px solid red";

            $('#modalEvento').modal('show');
        });
    }));

    $('#btnGuardarEvento').click(function () {
        var urlAction = nuevoEvento == true ? "CrearEvento" : "EditarEvento";
        var tipoEvento = nuevoEvento == true ? "crear" : "editar";

        var evento = {
            Id: nuevoEvento == true ? "" : $("#UltimoEvento_Id").val(),
            Titulo: $('#Titulo').val(),
            Imagen: $('#Imagen').val().split('\\').pop(),
            Descripcion: $('#Descripcion').val(),
            FechaEvento: $('#FechaEvento').val(),
            HoraEvento: $('#HoraEvento').val(),
            PeriodoInscripcion: $('#PeriodoInscripcion').val()
        };

        $.ajax({
            url: '/Home/' + urlAction,
            type: 'POST',
            data: evento,
            success: function (response) {
                if (response.success) {

                    Swal.fire({
                        icon: 'success',
                        title: "Evento guardado",
                        text: response.message,
                        showConfirmButton: false,
                        timer: 2000
                    }).then(function () {
                        // Cierra la modal después de guardar
                        $('#modalEvento').modal('hide');
                        location.reload();
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        html: response.message
                    });
                }
                
            },
            error: function (xhr, status, error) {
                console.log(error);
                alert('Hubo un error al ' + tipoEvento + ' el evento.');
                location.reload();
            }
        });
    });

});


$(document).ready(function () {
    $(document).on('click', '.editar-participante-btn', function () {
        var row = $(this).closest('tr');
        row.find('.bando-select, .pago-select, .lista-select').prop('disabled', false);
        row.find('.nombre-input, .nick-input').prop('readonly', false);
        row.addClass('editando');
        row.find('.editar-participante-btn').hide();
        row.find('.guardar-participante-btn').show();
    });

    $(document).ready(function () {
        // Manejar el clic en el botón "Eliminar"
        $('#tablaParticipantes').on('click', '.eliminar-participante-btn', function () {
            var idParticipante = $(this).parents("tr").find("input[name='id']").val(); 

            Swal.fire({
                title: '¿Estás seguro de que quieres eliminar el jugador?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Sí',
                cancelButtonText: 'Cancelar',
            }).then((result) => {
                if (result.isConfirmed && idParticipante != undefined) {
                    $.ajax({
                        url: '/Home/EliminarParticipante',
                        type: 'POST',
                        data: { idParticipante: idParticipante },
                        success: function () {
                            Swal.fire({
                                icon: 'success',
                                title: 'Jugador eliminado',
                                showConfirmButton: false,
                                timer: 2000
                            }).then(function () {
                                location.reload();
                            });

                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'Hubo un error al eliminar el participante'
                            });
                        }
                    });
                } else {
                    location.reload();
                }
            });       
        });
                
    });


    // Cuando se hace clic en "Guardar" en una fila de edición
    $(document).on('click', '.guardar-participante-btn', function () {
        var row = $(this).closest('tr');
        // Obtén los valores de los campos editables en esta fila
        var id = row.find('.id-input').val();
        var nombre = row.find('.nombre-input').val();
        var nick = row.find('.nick-input').val();
        var bando = row.find('.bando-select').val();
        var pago = row.find('.pago-select').val();
        var lista = row.find('.lista-select').val();
        row.removeClass('editando');
        row.find('.editar-participante-btn').show();
        row.find('.guardar-participante-btn').hide();

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
                        showConfirmButton: false,
                        timer: 2000
                    }).then(function () {
                        location.reload();
                    });
                    
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message
                    });
                }
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
                    <button class="btn btn-link editar-participante-btn" title="Editar"><i class="far fa-edit"></i></button>
                    <button class="btn btn-link guardar-participante-btn" title="Guardar" style="display: none;"><i class="far fa-save"></i></button>
                    <button class="btn btn-link eliminar-participante-btn" title="Eliminar"><i class="far fa-trash-alt"></i></button>
                </td>
            </tr>
        `;

        // Agregar la nueva fila al final de la tabla
        $('table tbody').append(newRowHtml);

        // Habilitar la edición en la nueva fila
        $('table tbody tr:last-child').find('.bando-select, .pago-select, .lista-select').prop('disabled', false);
        $('table tbody tr:last-child').find('.nombre-input, .nick-input').prop('readonly', false);
        $('table tbody tr:last-child').addClass('editando');
        $('table tbody tr:last-child').find('.editar-participante-btn').hide();
        $('table tbody tr:last-child').find('.guardar-participante-btn').show();
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

    $(document).on('click', '.editar-puntuacion-btn', function () {
        var row = $(this).closest('tr');
        row.find('.select-nick, .select-punto-partida, .select-puntos-victoria-obtenidos, .select-puntos-victoria-cedidos, .select-lider-abatido, .select-ronda').prop('disabled', false);
        row.addClass('editando');
        row.find('.editar-puntuacion-btn').hide();
        row.find('.guardar-puntuacion-btn').show();
    });

    $(document).ready(function () {        
        $('#tablaPuntuaciones').on('click', '.eliminar-puntuacion-btn', function () {
            var idPuntuacion = $(this).parents("tr").find("input[name='id']").val(); 

            Swal.fire({
                title: '¿Estás seguro de que quieres eliminar la puntuación?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Sí',
                cancelButtonText: 'Cancelar',
            }).then((result) => {
                if (result.isConfirmed && idPuntuacion != undefined) {
                    $.ajax({
                        url: '/Home/EliminarPuntuacion',
                        type: 'POST',
                        data: { idPuntuacion: idPuntuacion },
                        success: function () {
                            Swal.fire({
                                icon: 'success',
                                title: 'Puntuación eliminada',
                                showConfirmButton: false,
                                timer: 2000

                            }).then(function() {
                                location.reload();
                            });
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'Hubo un error al eliminar la puntuación'
                            })
                        }
                    });
                } else {
                    location.reload();
                }
            });
        });

    });

    $(document).on('click', '.guardar-puntuacion-btn', function () {
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
        row.find('.editar-puntuacion-btn').show();
        row.find('.guardar-puntuacion-btn').hide();

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
                        showConfirmButton: false,
                        timer: 2000
                    }).then(function () {
                        location.reload();
                    });

                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message
                    });
                }
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
                <button class="btn btn-link editar-puntuacion-btn" title="Editar"><i class="far fa-edit"></i></button>
                <button class="btn btn-link guardar-puntuacion-btn" title="Guardar" style="display: none;"><i class="far fa-save"></i></button>
                <button class="btn btn-link eliminar-puntuacion-btn" title="Eliminar"><i class="far fa-trash-alt"></i></button>
                
            </td>
        </tr>
    `;

        // Agregar la nueva fila al final de la tabla
        $('table tbody').append(newRowHtml);

        // Habilitar la edición en la nueva fila
        var newRow = $('table tbody tr:last-child');
        newRow.find('.select-nick, .select-punto-partida, .select-puntos-victoria-obtenidos, .select-puntos-victoria-cedidos, .select-lider-abatido, .select-ronda').prop('disabled', false);
        newRow.addClass('editando');
        newRow.find('.editar-puntuacion-btn').hide();
        newRow.find('.guardar-puntuacion-btn').show();
        
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

    

    function selectImage() {
         
    }
});


















 






























    
    













































