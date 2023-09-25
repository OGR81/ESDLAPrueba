namespace MESBG.Models
{
    public class EscenariosViewModel
    {
        public string[] Carpetas { get; set; }
        public List<string> Imagenes { get; set; }
        public string CarpetaSeleccionada { get; set; }

        public EscenariosViewModel()
        {
            Carpetas = new string[0];
            Imagenes = new List<string>();
        }
    }
}










