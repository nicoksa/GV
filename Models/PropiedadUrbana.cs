namespace GV.Models
{
    public class PropiedadUrbana : Propiedad
    {
        public string Tipo { get; set; }
        public int Ambientes { get; set; }
        public int Dormitorios { get; set; }
        public int Banios { get; set; }
        public int SuperficieTotal { get; set; }
        public int SuperficieCubierta { get; set; }
        public bool Cochera { get; set; }
        public bool Patio { get; set; }
        public bool Aire_acond { get; set; }
        public bool Seguridad{ get; set; }
        public string Direccion { get; set; }
        
    }
}
