﻿using System;
using System.Collections.Generic;
using Google.Cloud.Firestore; 

namespace MESBG.Models
{
    public class HomeViewModel
    {
        public Evento UltimoEvento { get; set; }
         
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
        public Timestamp FechaRegistro { get; set; }        
        [FirestoreProperty]
        public string PeriodoInscripcion { get; set; }
        [FirestoreProperty]
        public string Imagen { get; set; }
        [FirestoreProperty]
        public string Hora { get; set; }
        [FirestoreProperty]
        public string Fecha { get; set; }

    }
}



