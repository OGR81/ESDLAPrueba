using System.Collections.Generic;

namespace ESDLAPrueba.Models
{
    public class ParticipantesViewModel
    {
        public List<Participante> Participantes { get; set; }

        public ParticipantesViewModel()
        {
            Participantes = new List<Participante>();
        }
    }

    public class Participante
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Nick { get; set; } // Asigna una cadena vacía como valor predeterminado si Nick es nulo
        
    }
}



