namespace GV.Models
{
    public class PropiedadCampo : Propiedad
    {
        public PropiedadCampo()
        {
            Division = "Campo"; // Esto se ejecutará al crear cualquier instancia
        }
        public string Aptitud { get; set; }
        public int Hectareas { get; set; }
    }
}
