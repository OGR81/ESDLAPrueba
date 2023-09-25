using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace MESBG.Models
{
    public class ClasificacionViewModel
    {
        public List<ClasificacionJugador> Clasificacion { get; set; }       

    }

    [FirestoreData]
    public class ClasificacionJugador
    {
        public string? Id { get; set; }

        [FirestoreProperty]
        public string? Nick { get; set; }
        [FirestoreProperty]
        public int TotalPtosVictoriaObtenidos { get; set; }
        [FirestoreProperty]
        public int TotalPtosVictoriaCedidos { get; set; }
        [FirestoreProperty]
        public int TotalDifPtosVictoria { get; set; }
        [FirestoreProperty]
        public int TotalLideresAbatidos { get; set; }
        [FirestoreProperty]
        public int Total { get; set; }
    }
}


