using System.Collections.Generic;

namespace ESDLAPrueba.Models
{
    public class ClasificacionViewModel
    {
        public List<ClasificacionJugadorViewModel> Clasificacion { get; set; } = new List<ClasificacionJugadorViewModel>();
    }

    public class ClasificacionJugadorViewModel
    {
        public string? Jugador { get; set; }
        public int TotalPtosVictoriaObtenidos { get; set; }
        public int TotalPtosVictoriaCedidos { get; set; }
        public int TotalDifPtosVictoria { get; set; }
        public int TotalLideresAbatidos { get; set; }
        public int Total { get; set; }
    }
}


