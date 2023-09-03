using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace ESDLAPrueba.Models
{
    public class ParticipantesViewModel
    {
        public List<Participante> Participantes { get; set; }
    }

    [FirestoreData]
    public class Participante
    {
        //[FirestoreProperty]
        public string? Id { get; set; }
        
        [FirestoreProperty]        
        public string? Nombre { get; set; }
        [FirestoreProperty]
        public string? Nick { get; set; }
        [FirestoreProperty]
        public bool bandoSeleccionado { get; set; }
        [FirestoreProperty]
        public bool listaEnviada { get; set; }
        [FirestoreProperty]
        public bool pagoAbonado { get; set; } 
    }



}



