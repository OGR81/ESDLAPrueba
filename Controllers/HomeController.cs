using Microsoft.AspNetCore.Mvc;
using MESBG.Models;
using MESBG.Firebase;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using System.Globalization;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MESBG.Controllers
{
    public class HomeController : Controller
    {        
        public List<Participante> listaParticipantes = new();
        
        public List<Puntuacion> listaPuntuaciones = new();

        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly FirebaseManager _firebaseManager;

        public HomeController(IWebHostEnvironment hostingEnvironment, FirebaseManager firebaseManager)
        {
            _hostingEnvironment = hostingEnvironment;
            _firebaseManager = firebaseManager;
        }


        public async Task<IActionResult> Index()
        {
            HomeViewModel modelo = new();

            // Obtener el último evento como antes
            modelo.UltimoEvento = await _firebaseManager.GetUltimoEventoAsync();

            return View("Index", modelo);
        }

        [HttpPost]
        public async Task<IActionResult> CrearEvento(Evento evento)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Todos los campos son obligatorios.<br>Debe seleccionar una imagen." });
            }

            DateTime fechaEvento = ConvertDateStringToDateTime(evento.FechaEvento, evento.HoraEvento);
            DateTime fechaPreinscripcion = ConvertDateStringToDateTime(evento.PeriodoInscripcion, "23:59");

            if (fechaPreinscripcion > fechaEvento)
            {
                return Json(new { success = false, message = "La fecha de preinscripción debe ser menor a la del evento." });
            }

            evento.TimeStampFechaEvento = ConvertDateToTimestamp(fechaEvento.ToUniversalTime());
            evento.TimeStampPeriodoInscripcion = ConvertDateToTimestamp(fechaPreinscripcion.ToUniversalTime());

            await _firebaseManager.CrearDocEvento("eventos", evento);

            // Redirige a la acción "Index" con el ID del evento recién creado como parámetro
            return Json(new { success = true, message = "Evento creado" });
        }

        [HttpPost]
        public async Task<IActionResult> EditarEvento(Evento evento)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Todos los campos son obligatorios.<br>Debe seleccionar una imagen."});
            }

            DateTime fechaEvento = ConvertDateStringToDateTime(evento.FechaEvento, evento.HoraEvento);
            DateTime fechaPreinscripcion = ConvertDateStringToDateTime(evento.PeriodoInscripcion, "23:59");

            if(fechaPreinscripcion > fechaEvento)
            {
                return Json(new { success = false, message = "La fecha de preinscripción debe ser menor a la del evento." });
            }

            // Crea un diccionario con los campos a actualizar
            var actualizacion = new Dictionary<string, object>
            {
                { "Titulo", evento.Titulo },
                { "Descripcion", evento.Descripcion },
                { "TimeStampFechaEvento", ConvertDateToTimestamp(fechaEvento.ToUniversalTime()) },
                { "TimeStampPeriodoInscripcion", ConvertDateToTimestamp(fechaPreinscripcion.ToUniversalTime()) },
                { "Imagen", evento.Imagen },
                
            };
                                
            await _firebaseManager.ActualizarDocEvento("eventos", evento.Id, actualizacion);
                                    
            return Json(new { success = true, message = "Evento modificado" });
        }

        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images\\ImagenesEventos");
            var files = await Task.Run(()=> Directory.GetFiles(imagesDirectory));

            var images = files.Select(Path.GetFileName).ToList();

            return Ok(images);
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerEvento(string idDocumento)
        {
            try
            {
                Evento? _evento = await _firebaseManager.GetEventoPorId(idDocumento);
                return Json(new {success=true, evento = _evento });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, evento = ex });
            }
            
        }
        
        
        public async Task<IActionResult> Participantes()
        {
            var modelo = new ParticipantesViewModel();
            modelo.Participantes = await ObtenerListaParticipantes();                       

            return View("ParticipantesView", modelo);
        }


        private async Task<List<Participante>> ObtenerListaParticipantes()
        {
            return await _firebaseManager.GetParticipantes();

        }

        [HttpPost]
        public async Task<IActionResult> EliminarParticipante(string idParticipante)
        {
            Console.WriteLine($"Entra en Eliminar el participante con id:{idParticipante}" );           
        
            await _firebaseManager.EliminarDocParticipante("participantes", idParticipante);                       
            
            var modelo = new ParticipantesViewModel();
            modelo.Participantes = await ObtenerListaParticipantes(); ;

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> CrearParticipante(Participante participante)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Error al crear participante");
            }

            if (await VerificarNick(participante.Nick, participante.Id))
            {
                return Json(new { success = false, message = "El nick ya está en uso" });
            }

            await _firebaseManager.CrearDocParticipante("participantes", participante);
            // Si hay errores de validación, vuelve a mostrar la vista actual
            return Json(new { success = true, message = "Jugador añadido" });
        }

        [HttpPost]
        public async Task<IActionResult> EditarParticipante(Participante participante)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Error al editar participante");
            }

            if (await VerificarNick(participante.Nick, participante.Id))
            {
                return Json(new { success = false, message = "El nick ya está en uso" });
            }

            
            var actualizacion = new Dictionary<string, object>
            {
                { "Nombre", participante.Nombre },
                { "Nick", participante.Nick },
                { "Bando", participante.Bando },
                { "PagoAbonado", participante.PagoAbonado },
                { "ListaEnviada", participante.ListaEnviada },

            };
            
            
            await _firebaseManager.ActualizarDocParticipante("participantes", participante.Id, actualizacion);

            return Json(new { success = true, message = "Jugador modificado" });
        }              

        private async Task<bool> VerificarNick(string nick, string idDocumento)
        {
            listaParticipantes = await _firebaseManager.GetParticipantesBynick(nick);    
            if (string.IsNullOrEmpty(idDocumento))
            {
                return listaParticipantes.Any();
            }
            return listaParticipantes.Where(x=>x.Id!= idDocumento).Any();
        }
                
        
        public async Task<IActionResult> Puntuaciones()
        {
            var modelo = new PuntuacionesViewModel();
            modelo.Puntuaciones = await ObtenerListaPuntuaciones();
                        
            var participantes = await _firebaseManager.GetParticipantes();                        
            modelo.JugadoresDisponibles = participantes.Select(p => p.Nick).ToList();
            
            return View("PuntuacionesView", modelo);
        }
        

        private async Task<List<Puntuacion>> ObtenerListaPuntuaciones()
        {
            return await _firebaseManager.GetPuntuaciones();

        }

        [HttpPost]
        public async Task<IActionResult> EliminarPuntuacion(string idPuntuacion)
        {
            Console.WriteLine($"Entra en Eliminar el participante con id:{idPuntuacion}");
                        
            await _firebaseManager.EliminarDocPuntuacion("puntuaciones", idPuntuacion);

            var modelo = new PuntuacionesViewModel();
            modelo.Puntuaciones = await ObtenerListaPuntuaciones(); ;

            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> CrearPuntuacion(Puntuacion puntuacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Error al introducir la puntuación");
            }
                        
            await _firebaseManager.CrearDocPuntuacion("puntuaciones", puntuacion);
            // Si hay errores de validación, vuelve a mostrar la vista actual
            return Json(new { success = true, message = "Puntuación introducida" });
        }
        
        [HttpPost]
        public async Task<IActionResult> EditarPuntuacion(Puntuacion puntuacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Error al editar la puntuación");
            }
                        
            // Crea un diccionario con los campos a actualizar
            var actualizacion = new Dictionary<string, object>
            {                
                { "Nick", puntuacion?.Nick },
                { "PuntoPartida", puntuacion.PuntoPartida },
                { "PuntosVictoriaObtenidos", puntuacion.PuntosVictoriaObtenidos },
                { "PuntosVictoriaCedidos", puntuacion.PuntosVictoriaCedidos },
                { "DiferenciaPuntosVictoria", puntuacion.DiferenciaPuntosVictoria },
                { "LiderAbatido", puntuacion.LiderAbatido },
                { "Ronda", puntuacion.Ronda },
            };


            // Llama a FirebaseManager para actualizar el participante en Firebase
            await _firebaseManager.ActualizarDocPuntuacion("puntuaciones", puntuacion.Id, actualizacion);

            return Json(new { success = true, message = "Puntuación modificada" });
        }


        public async Task<IActionResult> Clasificacion()
        {
            var puntuaciones = await ObtenerListaPuntuaciones();

            // Agrupa las puntuaciones por nick
            var puntuacionesPorJugador = puntuaciones.GroupBy(p => p.Nick);

            // Calcula los totales por jugador
            var clasificacion = new List<ClasificacionJugador>();
            foreach (var grupo in puntuacionesPorJugador)
            {
                var nick = grupo.Key;
                var totalPtosVictoriaObtenidos = grupo.Sum(p => p.PuntosVictoriaObtenidos);
                var totalPtosVictoriaCedidos = grupo.Sum(p => p.PuntosVictoriaCedidos);
                var totalDifPtosVictoria = totalPtosVictoriaObtenidos - totalPtosVictoriaCedidos;
                var totalLideresAbatidos = grupo.Sum(p => p.LiderAbatido);
                var total = grupo.Sum(p => p.PuntoPartida);

                var clasificacionJugador = new ClasificacionJugador
                {
                    Nick = nick,
                    TotalPtosVictoriaObtenidos = totalPtosVictoriaObtenidos,
                    TotalPtosVictoriaCedidos = totalPtosVictoriaCedidos,
                    TotalDifPtosVictoria = totalDifPtosVictoria,
                    TotalLideresAbatidos = totalLideresAbatidos,
                    Total = total
                };

                clasificacion.Add(clasificacionJugador);
            }

            // Ordena la clasificación por el campo Total de forma descendente
            clasificacion = clasificacion.OrderByDescending(c => c.Total).ToList();

            // Crea una instancia de ClasificacionViewModel y asigna los datos calculados
            var clasificacionViewModel = new ClasificacionViewModel
            {
                Clasificacion = clasificacion
            };

            return View("ClasificacionView", clasificacionViewModel);
        }
        
        public IActionResult Escenarios(string carpetaSeleccionada)
        {
            var viewModel = new EscenariosViewModel();

            // Obtiene la lista de carpetas en wwwroot/images
            var carpetas = Directory.GetDirectories(Path.Combine(_hostingEnvironment.WebRootPath, "images"));
            viewModel.Carpetas = carpetas.Select(Path.GetFileName).ToArray()!;

            // Verifica si se seleccionó una carpeta
            if (!string.IsNullOrEmpty(carpetaSeleccionada))
            {
                // Obtiene la ruta completa de la carpeta seleccionada
                var carpetaSeleccionadaPath = Path.Combine(_hostingEnvironment.WebRootPath, "images", carpetaSeleccionada);

                // Obtiene la lista de archivos de imagen en la carpeta seleccionada
                var imagenes = Directory.GetFiles(carpetaSeleccionadaPath, "*.jpg", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName).ToList()!;

                viewModel.CarpetaSeleccionada = carpetaSeleccionada;
                viewModel.Imagenes = imagenes.Cast<string>().ToList();

            }

            return View("EscenariosView", viewModel);
        }

        private static DateTime ConvertDateStringToDateTime(string date, string hour)
        {
            return DateTime.ParseExact(date + " " + hour, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }

        private static Timestamp ConvertDateToTimestamp(DateTime date)
        {
            return Timestamp.FromDateTime(date.ToUniversalTime());
        }
    }
}












