using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace MESBG.Models
{
    public class PuntuacionesViewModel
    {
        public List<Puntuacion> Puntuaciones { get; set; }
        public List<string> JugadoresDisponibles { get; set; } 
                
    }

    [FirestoreData]
    public class Puntuacion
    {
        // Propiedades de la puntuación
        public string? Id { get; set; }

        [FirestoreProperty]
        public string? Nick { get; set; }

        [FirestoreProperty]
        public int PuntoPartida { get; set; }

        [FirestoreProperty]
        public int PuntosVictoriaObtenidos { get; set; }

        [FirestoreProperty]
        public int PuntosVictoriaCedidos { get; set; }

        [FirestoreProperty]
        public int DiferenciaPuntosVictoria => PuntosVictoriaObtenidos - PuntosVictoriaCedidos;

        [FirestoreProperty]
        public int LiderAbatido { get; set; }

        [FirestoreProperty]
        public int Ronda { get; set; }
    }
}









