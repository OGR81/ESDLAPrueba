using System;
using System.Collections.Generic;
using Google.Cloud.Firestore; 

namespace MESBG.Models
{
    public class HomeViewModel
    {
        public Evento ProximoEvento { get; set; }
        public List<Evento> EventosPasados { get; set; } 
    }

    [FirestoreData] 
    public class Evento
    {
        public string? Id { get; set; }
        [FirestoreProperty]
        public string Titulo { get; set; }
        [FirestoreProperty]
        public string Descripcion { get; set; }
        [FirestoreProperty]
        public DateTime Fecha { get; set; }
        [FirestoreProperty]
        public DateTime Hora { get; set; }
        [FirestoreProperty]
        public DateTime PeriodoInscripcion { get; set; }
        [FirestoreProperty]
        public string Imagen { get; set; } 
    }
}



