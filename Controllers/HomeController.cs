﻿using Microsoft.AspNetCore.Hosting;
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
using Google.Protobuf.Collections;
using Serilog;
using System.Runtime.Intrinsics.X86;

namespace ESDLAPrueba.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public List<Participante> listaParticipantes = new();
        
        public List<Puntuacion> listaPuntuaciones = new();

        private readonly FirebaseManager _firebaseManager;

        public HomeController(FirebaseManager firebaseManager)
        {
            _firebaseManager = firebaseManager;
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
        
        
        public async Task<IActionResult> Participantes()
        {
            var modelo = new ParticipantesViewModel();
            modelo.Participantes = await ObtenerListaParticipantes(); 
            //List<Participante> lista = await ObtenerListaParticipantes();

            

            return View("ParticipantesView", modelo);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarParticipante(string idParticipante)
        {
            Console.WriteLine($"Entra en Eliminar el participante con id:{idParticipante}" );
            
            // Llama a EliminarParticipanteAsync para eliminar el participante en Firebase
            await _firebaseManager.EliminarDocParticipante("participantes", idParticipante);

            // Obtener la lista actualizada de participantes desde Firebase
            
            var modelo = new ParticipantesViewModel();
            modelo.Participantes = await ObtenerListaParticipantes(); ;


            // Devuelve la vista con la lista actualizada
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

            // Crea un diccionario con los campos a actualizar
            var actualizacion = new Dictionary<string, object>
            {
                { "Nombre", participante?.Nombre },
                { "Nick", participante?.Nick },
                { "Bando", participante.Bando },
                { "PagoAbonado", participante.PagoAbonado },
                { "ListaEnviada", participante.ListaEnviada },

            };

            
            // Llama a FirebaseManager para actualizar el participante en Firebase
            await _firebaseManager.ActualizarDocParticipante("participantes", participante.Id, actualizacion);

            return Json(new { success = true, message = "Jugador modificado" });
        }

        private async Task <List<Participante>> ObtenerListaParticipantes()
        { 
            return await _firebaseManager.GetParticipantes();
         
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

            // Llama a EliminarParticipanteAsync para eliminar el participante en Firebase
            await _firebaseManager.EliminarDocPuntuacion("puntuaciones", idPuntuacion);

            // Obtener la lista actualizada de participantes desde Firebase

            var modelo = new PuntuacionesViewModel();
            modelo.Puntuaciones = await ObtenerListaPuntuaciones(); ;


            // Devuelve la vista con la lista actualizada
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


        /*public IActionResult Clasificacion()
        {
            
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
        }*/





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












