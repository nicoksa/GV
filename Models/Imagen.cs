namespace GV.Models
{
    public class Imagen
    {
        public int Id { get; set; }
        public string Url { get; set; }  // ruta o enlace a la imagen (puede ser local o en la nube)
        public bool EsPrincipal { get; set; }

        // Relación con propiedad
        public int PropiedadId { get; set; }
        public Propiedad Propiedad { get; set; }
    }
}
