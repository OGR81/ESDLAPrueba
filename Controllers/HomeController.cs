using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using ESDLAPrueba.Models;
using ESDLAPrueba.Firebase;
using ESDLAPrueba;
using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore.Internal;

namespace ESDLAPrueba.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public List<Participante> listaParticipantes { get; set; }

        private readonly FirebaseManager _firebaseManager;

        public HomeController(FirebaseManager firebaseManager)
        {
            _firebaseManager = firebaseManager;
            listaParticipantes = new List<Participante>();
        }
        

        public IActionResult Index()
        {
            var modelo = new HomeViewModel();
            return View(modelo);
        }

        [HttpPost]
        public IActionResult Index(IFormFile cargaImagen)
        {
            // Código del método Index actual

            return View();
        }

        public IActionResult Participantes()
        {
            var modelo = new ParticipantesViewModel();
            var lista = ObtenerListaParticipantes();
            lista.Wait();
            modelo.Participantes = lista.Result;
            return View("ParticipantesView", modelo);
        }

        [HttpPost]
        public IActionResult EditarParticipante(Participante participante)
        {
            var participanteExistente = listaParticipantes.Find(p => p.Id == participante.Id);
            if (participanteExistente != null)
            {
                participanteExistente.Nombre = participante.Nombre;
                participanteExistente.Nick = participante.Nick;
            }

            return RedirectToAction("Participantes");
        }

        public async Task<IActionResult> EliminarParticipante(int id)
        {
            Console.WriteLine("Entra en Eliminar el participante con Id: " + id);
            var participanteExistente = listaParticipantes.Find(p => p.Id == id);
            if (participanteExistente != null)
            {
                // Llama a EliminarParticipanteAsync para eliminar el participante en Firebase
                await EliminarParticipanteAsync(id);

                // Obtén la lista actualizada de participantes desde Firebase
                var lista = await ObtenerListaParticipantes();
                listaParticipantes = lista; // Actualiza la lista local en el controlador

                var modelo = new ParticipantesViewModel();
                modelo.Participantes = lista;

                // Devuelve la vista con la lista actualizada
                return View("ParticipantesView", modelo);
            }

            return RedirectToAction("Participantes");
        }




        [HttpPost]
        public IActionResult GuardarParticipante(Participante participante)
        {            
            var participanteExistente = listaParticipantes.Find(p => p.Id == participante.Id);
            if (participanteExistente != null)
            {
                participanteExistente.Nombre = participante.Nombre;
                participanteExistente.Nick = participante.Nick;
            }

            return RedirectToAction("Participantes");
        }

        public async Task <List<Participante>> ObtenerListaParticipantes()
        { 
            return await _firebaseManager.GetParticipantes();
         
        }

        public async Task EliminarParticipanteAsync(int Id)
        {
            Console.WriteLine("Dentro de EliminarParticipanteAsync" + Id);
            await _firebaseManager.DeleteParticipante(Id); 
        }

        public IActionResult Puntuaciones()
        {
            // Aquí puedes obtener las puntuaciones de alguna fuente de datos, como una base de datos

            // Por ejemplo, crearemos una lista ficticia de puntuaciones para demostración
            var puntuaciones = new List<PuntuacionesViewModel>
            {
                new PuntuacionesViewModel { Jugador = "Jugador 1", PuntoPartida = 3, PuntosVictoriaObtenidos = 10, PuntosVictoriaCedidos = 5, LiderAbatido = 0, Ronda = 1 },
                new PuntuacionesViewModel { Jugador = "Jugador 2", PuntoPartida = 1, PuntosVictoriaObtenidos = 8, PuntosVictoriaCedidos = 6, LiderAbatido = 0, Ronda = 1 },
                new PuntuacionesViewModel { Jugador = "Jugador 3", PuntoPartida = 0, PuntosVictoriaObtenidos = 12, PuntosVictoriaCedidos = 3, LiderAbatido = 0, Ronda = 1 }
            };

            // Crea una instancia de PuntuacionViewModel y asigna las puntuaciones obtenidas
            var puntuacionViewModel = new PuntuacionesViewModel
            {
                JugadoresDisponibles = new List<string> { "Jugador 1", "Jugador 2", "Jugador 3" }, // Puedes obtener la lista de jugadores desde alguna fuente de datos
                Puntuaciones = puntuaciones // Asigna la lista de puntuaciones al modelo
            };

            return View("PuntuacionesView", puntuacionViewModel);
        }


        public IActionResult Clasificacion()
        {
            // Obtén las puntuaciones de alguna fuente de datos, como una base de datos
            // Aquí puedes utilizar la misma lógica que tienes en la acción Puntuaciones

            // Por ejemplo, supongamos que tienes la lista de puntuaciones en el modelo PuntuacionesViewModel
            var puntuaciones = new List<PuntuacionesViewModel>
            {
                new PuntuacionesViewModel { Jugador = "Jugador 1", PuntoPartida = 1, PuntosVictoriaObtenidos = 10, PuntosVictoriaCedidos = 5, LiderAbatido = 2, Ronda = 1 },
                new PuntuacionesViewModel { Jugador = "Jugador 2", PuntoPartida = 3, PuntosVictoriaObtenidos = 8, PuntosVictoriaCedidos = 6, LiderAbatido = 3, Ronda = 1 },
                new PuntuacionesViewModel { Jugador = "Jugador 3", PuntoPartida = 0, PuntosVictoriaObtenidos = 12, PuntosVictoriaCedidos = 3, LiderAbatido = 4, Ronda = 1 },
                new PuntuacionesViewModel { Jugador = "Jugador 1", PuntoPartida = 1, PuntosVictoriaObtenidos = 10, PuntosVictoriaCedidos = 5, LiderAbatido = 1, Ronda = 2 },
                new PuntuacionesViewModel { Jugador = "Jugador 2", PuntoPartida = 3, PuntosVictoriaObtenidos = 8, PuntosVictoriaCedidos = 6, LiderAbatido = 5, Ronda = 2 },
                new PuntuacionesViewModel { Jugador = "Jugador 3", PuntoPartida = 3, PuntosVictoriaObtenidos = 12, PuntosVictoriaCedidos = 3, LiderAbatido = 4, Ronda = 2 },
                new PuntuacionesViewModel { Jugador = "Jugador 1", PuntoPartida = 0, PuntosVictoriaObtenidos = 10, PuntosVictoriaCedidos = 5, LiderAbatido = 6, Ronda = 3 },
                new PuntuacionesViewModel { Jugador = "Jugador 2", PuntoPartida = 3, PuntosVictoriaObtenidos = 8, PuntosVictoriaCedidos = 6, LiderAbatido = 3, Ronda = 3 },
                new PuntuacionesViewModel { Jugador = "Jugador 3", PuntoPartida = 1, PuntosVictoriaObtenidos = 12, PuntosVictoriaCedidos = 3, LiderAbatido = 2, Ronda = 3 }
            };


            // Agrupa las puntuaciones por jugador
            var puntuacionesPorJugador = puntuaciones.GroupBy(p => p.Jugador);

            // Calcula los totales por jugador
            var clasificacion = new List<ClasificacionJugadorViewModel>();
            foreach (var grupo in puntuacionesPorJugador)
            {
                var jugador = grupo.Key;
                var totalPtosVictoriaObtenidos = grupo.Sum(p => p.PuntosVictoriaObtenidos);
                var totalPtosVictoriaCedidos = grupo.Sum(p => p.PuntosVictoriaCedidos);
                var totalDifPtosVictoria = totalPtosVictoriaObtenidos - totalPtosVictoriaCedidos;
                var totalLideresAbatidos = grupo.Sum(p => p.LiderAbatido);
                var total = grupo.Sum(p => p.PuntoPartida);

                var clasificacionJugador = new ClasificacionJugadorViewModel
                {
                    Jugador = jugador,
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

    }

}












