using System.Collections.Generic;

namespace ESDLAPrueba.Models
{
    public class PuntuacionesViewModel
    {
        // Propiedades de la puntuación
        public string? Jugador { get; set; }
        public int PuntoPartida { get; set; }
        public int PuntosVictoriaObtenidos { get; set; }
        public int PuntosVictoriaCedidos { get; set; }
        public int DiferenciaPuntosVictoria => PuntosVictoriaObtenidos - PuntosVictoriaCedidos;
        public int LiderAbatido { get; set; }
        public int Ronda { get; set; }

        // Lista de jugadores disponibles
        public List<string> JugadoresDisponibles { get; set; }

        // Lista de puntuaciones
        public List<PuntuacionesViewModel> Puntuaciones { get; set; }

        // Constructor
        public PuntuacionesViewModel()
        {
            JugadoresDisponibles = new List<string>();
            Puntuaciones = new List<PuntuacionesViewModel>();
        }


    }
}








