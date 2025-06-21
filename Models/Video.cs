namespace GV.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Url { get; set; } // Usar YouTube o archivo .mp4

        public int PropiedadId { get; set; }
        public Propiedad Propiedad { get; set; }
    }
}
