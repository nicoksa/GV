using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;

namespace GV.Models
{
    public abstract class Propiedad
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }   
        public string Division { get; set; } // "Campo" o "Urbano"
        public string Ubicacion { get; set; }
        public decimal Precio { get; set; } 
        public DateTime FechaPublicacion { get; set; } = DateTime.Now;

        [DisplayName("Destacada")]
        public bool EsDestacada { get; set; }

        // Relación
        public List<Imagen> Imagenes { get; set; } = new List<Imagen>();
        public List<Video> Videos { get; set; } = new List<Video>();
    }
}
